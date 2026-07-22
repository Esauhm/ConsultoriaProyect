using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Consultoria.IntegrationTests.Endpoints;

public sealed class ConcurrencyEndpointTests
    : IClassFixture<ConsultoriaWebApplicationFactory>
{
    private const string AdminEmail = "admin@demo.com";

    private readonly ConsultoriaWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ConcurrencyEndpointTests(
        ConsultoriaWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
    }

    [Fact]
    public async Task ActualizarArea_DebeRetornarConflict_CuandoRowVersionEstaDesactualizado()
    {
        // Arrange: iniciar sesión como administrador.
        string token = await ObtenerTokenAdminAsync();

    _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

    string identificador =
        Guid.NewGuid()
            .ToString("N")[..8];

    string nombreOriginal =
        $"Área concurrencia {identificador}";

        // Crear un área para esta prueba.
        using HttpResponseMessage crearResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/areas-especializacion",
                new
                {
    nombre = nombreOriginal
});

        Assert.True(
            crearResponse.IsSuccessStatusCode,
            await crearResponse.Content.ReadAsStringAsync());

        using JsonDocument crearJson =
            await LeerJsonAsync(crearResponse);

int areaId =
    ObtenerPropiedadRecursiva(
        crearJson.RootElement,
        "areaEspecializacionId")
    .GetInt32();

        // Consultar el área para obtener la versión vigente.
        using HttpResponseMessage obtenerResponse =
            await _client.GetAsync(
                $"/api/v1/areas-especializacion/{areaId}");

Assert.Equal(
    HttpStatusCode.OK,
    obtenerResponse.StatusCode);

        using JsonDocument obtenerJson =
            await LeerJsonAsync(obtenerResponse);

string rowVersionBase64 =
    ObtenerPropiedadRecursiva(
        obtenerJson.RootElement,
        "rowVersion")
    .GetString()
    ?? throw new InvalidOperationException(
        "La API no devolvió RowVersion.");

byte[] rowVersionOriginal =
    Convert.FromBase64String(
        rowVersionBase64);

// Act 1: primera actualización con la versión vigente.
string primerNombre =
    $"Primera actualización {identificador}";

        using HttpResponseMessage primeraActualizacion =
            await _client.PutAsJsonAsync(
                $"/api/v1/areas-especializacion/{areaId}",
                new
                {
    nombre = primerNombre,
                    rowVersion = rowVersionOriginal
});

        // Assert 1: la primera actualización debe funcionar.
        Assert.Equal(
            HttpStatusCode.OK,
            primeraActualizacion.StatusCode);

        // Act 2: reutilizar deliberadamente el mismo
        // RowVersion anterior.
        string segundoNombre =
            $"Segunda actualización {identificador}";

        using HttpResponseMessage segundaActualizacion =
            await _client.PutAsJsonAsync(
                $"/api/v1/areas-especializacion/{areaId}",
                new
                {
    nombre = segundoNombre,
                    rowVersion = rowVersionOriginal
});

        string contenidoConflicto =
            await segundaActualizacion.Content
                .ReadAsStringAsync();

// Assert 2: la versión antigua debe producir conflicto.
Assert.Equal(
    HttpStatusCode.Conflict,
    segundaActualizacion.StatusCode);

        Assert.Contains(
            "concurrencia",
            contenidoConflicto,
            StringComparison.OrdinalIgnoreCase);

        // Confirmar que el segundo cambio no se guardó.
        using HttpResponseMessage verificarResponse =
            await _client.GetAsync(
                $"/api/v1/areas-especializacion/{areaId}");

Assert.Equal(
    HttpStatusCode.OK,
    verificarResponse.StatusCode);

        using JsonDocument verificarJson =
            await LeerJsonAsync(verificarResponse);

string nombreGuardado =
    ObtenerPropiedadRecursiva(
        verificarJson.RootElement,
        "nombre")
    .GetString()
    ?? string.Empty;

Assert.Equal(
    primerNombre,
    nombreGuardado);

        Assert.NotEqual(
            segundoNombre,
            nombreGuardado);
    }

    private async Task<string> ObtenerTokenAdminAsync()
{
    using HttpResponseMessage response =
        await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email = AdminEmail,
                password = _factory.AdminPassword
            });

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    using JsonDocument json =
        await LeerJsonAsync(response);

    return ObtenerPropiedadRecursiva(
            json.RootElement,
            "token")
        .GetString()
        ?? throw new InvalidOperationException(
            "La respuesta de autenticación no contiene el token.");
}

private static async Task<JsonDocument> LeerJsonAsync(
    HttpResponseMessage response)
{
    string contenido =
        await response.Content.ReadAsStringAsync();

    return JsonDocument.Parse(contenido);
}

private static JsonElement ObtenerPropiedadRecursiva(
    JsonElement elemento,
    string nombrePropiedad)
{
    if (elemento.ValueKind == JsonValueKind.Object)
    {
        foreach (JsonProperty propiedad
                 in elemento.EnumerateObject())
        {
            if (string.Equals(
                propiedad.Name,
                nombrePropiedad,
                StringComparison.OrdinalIgnoreCase))
            {
                return propiedad.Value;
            }

            if (propiedad.Value.ValueKind is
                JsonValueKind.Object or
                JsonValueKind.Array)
            {
                try
                {
                    return ObtenerPropiedadRecursiva(
                        propiedad.Value,
                        nombrePropiedad);
                }
                catch (InvalidOperationException)
                {
                    // Continuar buscando.
                }
            }
        }
    }

    if (elemento.ValueKind == JsonValueKind.Array)
    {
        foreach (JsonElement item
                 in elemento.EnumerateArray())
        {
            try
            {
                return ObtenerPropiedadRecursiva(
                    item,
                    nombrePropiedad);
            }
            catch (InvalidOperationException)
            {
                // Continuar buscando.
            }
        }
    }

    throw new InvalidOperationException(
        $"No se encontró la propiedad " +
        $"'{nombrePropiedad}' en la respuesta JSON.");
}
}