using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Consultoria.IntegrationTests.Endpoints
{
    public sealed class MainBusinessFlowEndpointTests
        : IClassFixture<ConsultoriaWebApplicationFactory>
    {
        private const string LoginRoute =
            "/api/v1/auth/login";

        private const string AreasRoute =
            "/api/v1/areas-especializacion";

        private const string ConsultoresRoute =
            "/api/v1/consultores";

        private const string PaquetesRoute =
            "/api/v1/paquetes";

        private readonly ConsultoriaWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public MainBusinessFlowEndpointTests(
            ConsultoriaWebApplicationFactory factory)
        {
            _factory = factory;

            _client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress =
                        new Uri("https://localhost"),

                    AllowAutoRedirect = false
                });
        }

        [Fact]
        public async Task Admin_DebeCrearAreaConsultorYPaquete_ConCostoCalculado()
        {
            // Arrange: autenticación del administrador
            string token =
                await ObtenerTokenAdminAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            string identificador =
                Guid.NewGuid()
                    .ToString("N")[..8];

            const decimal tarifaHora = 75;
            const int duracionHoras = 8;
            const decimal costoEsperado =
                tarifaHora * duracionHoras;

            // -------------------------------------------------
            // 1. Crear área
            // -------------------------------------------------

            var areaRequest = new
            {
                nombre =
                    $"Arquitectura de Software {identificador}"
            };

            using HttpResponseMessage areaResponse =
                await _client.PostAsJsonAsync(
                    AreasRoute,
                    areaRequest);

            JsonElement areaData =
                await ObtenerDataAsync(
                    areaResponse,
                    HttpStatusCode.Created,
                    "crear el área");

            int areaEspecializacionId =
                areaData
                    .GetProperty("areaEspecializacionId")
                    .GetInt32();

            Assert.True(areaEspecializacionId > 0);

            // -------------------------------------------------
            // 2. Crear consultor
            // -------------------------------------------------

            var consultorRequest = new
            {
                nombre =
                    $"Consultor Integración {identificador}",

                areaEspecializacionId,

                tarifaHora,

                emailCorporativo =
                    $"integracion.{identificador}@consultoria.com"
            };

            using HttpResponseMessage consultorResponse =
                await _client.PostAsJsonAsync(
                    ConsultoresRoute,
                    consultorRequest);

            JsonElement consultorData =
                await ObtenerDataAsync(
                    consultorResponse,
                    HttpStatusCode.Created,
                    "crear el consultor");

            int consultorId =
                consultorData
                    .GetProperty("consultorId")
                    .GetInt32();

            Assert.True(consultorId > 0);

            Assert.Equal(
                areaEspecializacionId,
                consultorData
                    .GetProperty("areaEspecializacionId")
                    .GetInt32());

            Assert.Equal(
                tarifaHora,
                consultorData
                    .GetProperty("tarifaHora")
                    .GetDecimal());

            // -------------------------------------------------
            // 3. Crear paquete
            // -------------------------------------------------

            var paqueteRequest = new
            {
                nombre =
                    $"Paquete Arquitectura {identificador}",

                consultorId,

                duracionHoras,

                descripcion =
                    "Paquete creado mediante una prueba de integración."
            };

            using HttpResponseMessage paqueteResponse =
                await _client.PostAsJsonAsync(
                    PaquetesRoute,
                    paqueteRequest);

            JsonElement paqueteData =
                await ObtenerDataAsync(
                    paqueteResponse,
                    HttpStatusCode.Created,
                    "crear el paquete");

            // -------------------------------------------------
            // 4. Verificar reglas de negocio
            // -------------------------------------------------

            Assert.True(
                paqueteData
                    .GetProperty("paqueteId")
                    .GetInt32() > 0);

            Assert.Equal(
                consultorId,
                paqueteData
                    .GetProperty("consultorId")
                    .GetInt32());

            Assert.Equal(
                areaEspecializacionId,
                paqueteData
                    .GetProperty("areaEspecializacionId")
                    .GetInt32());

            Assert.Equal(
                duracionHoras,
                paqueteData
                    .GetProperty("duracionHoras")
                    .GetInt32());

            Assert.Equal(
                tarifaHora,
                paqueteData
                    .GetProperty("tarifaHoraAplicada")
                    .GetDecimal());

            Assert.Equal(
                costoEsperado,
                paqueteData
                    .GetProperty("costo")
                    .GetDecimal());

            Assert.True(
                paqueteData
                    .GetProperty("activo")
                    .GetBoolean());
        }

        private async Task<string> ObtenerTokenAdminAsync()
        {
            var loginRequest = new
            {
                email = "admin@demo.com",
                password = _factory.AdminPassword
            };

            using HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    LoginRoute,
                    loginRequest);

            JsonElement data =
                await ObtenerDataAsync(
                    response,
                    HttpStatusCode.OK,
                    "iniciar sesión como administrador");

            string? token =
                data
                    .GetProperty("token")
                    .GetString();

            Assert.False(
                string.IsNullOrWhiteSpace(token));

            return token!;
        }

        private static async Task<JsonElement> ObtenerDataAsync(
            HttpResponseMessage response,
            HttpStatusCode statusEsperado,
            string operacion)
        {
            string responseBody =
                await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == statusEsperado,
                $"La operación '{operacion}' debía retornar " +
                $"{(int)statusEsperado} {statusEsperado}, pero retornó " +
                $"{(int)response.StatusCode} {response.StatusCode}." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            using JsonDocument document =
                JsonDocument.Parse(responseBody);

            Assert.True(
                document.RootElement.TryGetProperty(
                    "data",
                    out JsonElement data),
                $"La respuesta de '{operacion}' no contiene " +
                $"la propiedad 'data'." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            /*
             * Clone permite devolver el elemento aunque
             * JsonDocument se destruya al terminar el método.
             */
            return data.Clone();
        }
    }
}
