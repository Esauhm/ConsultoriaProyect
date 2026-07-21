using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Consultoria.IntegrationTests.Endpoints
{
    public sealed class SecurityEndpointTests
     : IClassFixture<ConsultoriaWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SecurityEndpointTests(
            ConsultoriaWebApplicationFactory factory)
        {
            /*
             * Usamos HTTPS para evitar que
             * UseHttpsRedirection retorne un 307.
             */
            _client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress =
                        new Uri("https://localhost"),

                    AllowAutoRedirect = false
                });
        }

        [Fact]
        public async Task ObtenerConsultores_SinToken_DebeRetornarUnauthorized()
        {
            // Act
            using HttpResponseMessage response =
                await _client.GetAsync(
                    "/api/v1/consultores");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task Login_ConCredencialesInvalidas_DebeRetornarUnauthorized()
        {
            // Arrange
            var request = new
            {
                email = "usuario.inexistente@demo.com",
                password = "Password.Incorrecta.123"
            };

            // Act
            using HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    "/api/v1/auth/login",
                    request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }
    }
}
