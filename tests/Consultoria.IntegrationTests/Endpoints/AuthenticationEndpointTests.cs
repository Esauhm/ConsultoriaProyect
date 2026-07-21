using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Consultoria.IntegrationTests.Endpoints
{
    public sealed class AuthenticationEndpointTests
    : IClassFixture<ConsultoriaWebApplicationFactory>
    {
        private readonly ConsultoriaWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AuthenticationEndpointTests(
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
        public async Task Login_ConCredencialesValidas_DebeRetornarToken()
        {
            // Act
            string token =
                await ObtenerTokenAdminAsync();

            // Assert
            Assert.False(
                string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task ObtenerConsultores_ConTokenValido_DebeRetornarOk()
        {
            // Arrange
            string token =
                await ObtenerTokenAdminAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            // Act
            using HttpResponseMessage response =
                await _client.GetAsync(
                    "/api/v1/consultores");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        private async Task<string> ObtenerTokenAdminAsync()
        {
            var request = new
            {
                email = "admin@demo.com",
                password = _factory.AdminPassword
            };

            using HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    "/api/v1/auth/login",
                    request);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.OK,
                $"El login debía retornar 200 OK, pero retornó " +
                $"{(int)response.StatusCode} {response.StatusCode}." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            using JsonDocument jsonDocument =
                JsonDocument.Parse(responseBody);

            JsonElement root =
                jsonDocument.RootElement;

            bool contieneData =
                root.TryGetProperty(
                    "data",
                    out JsonElement data);

            Assert.True(
                contieneData,
                $"La respuesta del login no contiene la propiedad 'data'." +
                Environment.NewLine +
                $"Respuesta: {responseBody}");

            bool contieneToken =
                data.TryGetProperty(
                    "token",
                    out JsonElement tokenElement);

            Assert.True(
                contieneToken,
                $"La respuesta del login no contiene la propiedad 'data.token'." +
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
