using Consultoria.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Consultoria.IntegrationTests.Endpoints
{
    public sealed class HealthEndpointTests
    : IClassFixture<ConsultoriaWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthEndpointTests(
            ConsultoriaWebApplicationFactory factory)
        {
            _client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost"),
                    AllowAutoRedirect = false
                });
        }

        [Theory]
        [InlineData("/health/live")]
        [InlineData("/health/ready")]
        public async Task HealthEndpoint_DebeRetornarHealthy(
            string endpoint)
        {
            // Act
            using HttpResponseMessage response =
                await _client.GetAsync(endpoint);

            string contenido =
                await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            Assert.Equal(
                "Healthy",
                contenido);
        }
    }
}
