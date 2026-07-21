using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Consultoria.IntegrationTests.Endpoints
{
    public sealed class AuthorizationEndpointTests
    : IClassFixture<ConsultoriaWebApplicationFactory>
    {
        private readonly ConsultoriaWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AuthorizationEndpointTests(
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
        public async Task CrearConsultor_ComoUsuario_DebeRetornarForbidden()
        {
            // Arrange
            string token =
                await ObtenerTokenUsuarioAsync();

            var request = new
            {
                nombre = "Consultor de prueba",
                areaEspecializacionId = 1,
                tarifaHora = 50,
                emailCorporativo =
                    "consultor.prueba@consultoria.com"
            };

            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "/api/v1/consultores")
                {
                    Content =
                        JsonContent.Create(request)
                };

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            // Act
            using HttpResponseMessage response =
                await _client.SendAsync(httpRequest);

            // Assert
            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        private async Task<string> ObtenerTokenUsuarioAsync()
        {
            var loginRequest = new
            {
                email = "usuario@demo.com",
                password = _factory.UserPassword
            };

            using HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    "/api/v1/auth/login",
                    loginRequest);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.OK,
                $"El login del usuario debía retornar 200 OK, " +
                $"pero retornó {(int)response.StatusCode} " +
                $"{response.StatusCode}." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            using JsonDocument jsonDocument =
                JsonDocument.Parse(responseBody);

            JsonElement root =
                jsonDocument.RootElement;

            Assert.True(
                root.TryGetProperty(
                    "data",
                    out JsonElement data),
                $"La respuesta no contiene la propiedad 'data'." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            Assert.True(
                data.TryGetProperty(
                    "token",
                    out JsonElement tokenElement),
                $"La respuesta no contiene 'data.token'." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            string? token =
                tokenElement.GetString();

            Assert.False(
                string.IsNullOrWhiteSpace(token));

            return token!;
        }
    }
}
