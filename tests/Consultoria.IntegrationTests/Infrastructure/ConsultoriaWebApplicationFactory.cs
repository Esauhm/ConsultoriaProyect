using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;
using Consultoria.Infrastructure.Persistence.Context;

namespace Consultoria.IntegrationTests.Infrastructure
{
    public sealed class ConsultoriaWebApplicationFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private const string MsSqlImage =
            "mcr.microsoft.com/mssql/server:2022-latest";

        private const string TestAdminPassword =
            "Admin.Test.123!";

        private const string TestUserPassword =
            "User.Test.123!";

        private const string TestJwtKey =
            "Consultoria-Integration-Tests-Key-2026-Only-For-Automated-Testing";

        private readonly MsSqlContainer _sqlServerContainer =
            new MsSqlBuilder(MsSqlImage)
                .WithName(
                    $"consultoria-integration-sql-{Guid.NewGuid():N}")
                .Build();

        /*
         * Estas propiedades permitirán que las pruebas
         * de login reutilicen las credenciales sin duplicarlas.
         */
        public string AdminPassword =>
            TestAdminPassword;

        public string UserPassword =>
            TestUserPassword;

        public string ConnectionString =>
            _sqlServerContainer.GetConnectionString();

        /*
         * xUnit inicia el contenedor antes de crear
         * los clientes HTTP y ejecutar las pruebas.
         */
        public Task InitializeAsync()
        {
            return _sqlServerContainer.StartAsync();
        }

        /*
         * Aquí solamente reemplazamos servicios.
         *
         * No colocamos ConfigureAppConfiguration porque
         * sería demasiado tarde para AddInfrastructure.
         */
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                /*
                 * Eliminar el DbContext registrado originalmente
                 * por Consultoria.Infrastructure.
                 */
                services.RemoveAll<
                    DbContextOptions<ConsultoriaDbContext>>();

                services.RemoveAll<ConsultoriaDbContext>();

                /*
                 * Registrar el DbContext conectado al
                 * SQL Server temporal de Testcontainers.
                 */
                services.AddDbContext<ConsultoriaDbContext>(
                    options =>
                    {
                        options.UseSqlServer(
                            _sqlServerContainer.GetConnectionString(),
                            sqlServerOptions =>
                            {
                                sqlServerOptions.MigrationsAssembly(
                                    typeof(ConsultoriaDbContext)
                                        .Assembly
                                        .GetName()
                                        .Name);
                            });
                    });
            });
        }

        /*
         * CreateHost se ejecuta antes del arranque completo
         * de Program.cs.
         *
         * Por eso aquí colocamos los valores que
         * AddInfrastructure necesita inmediatamente.
         */
        protected override IHost CreateHost(
            IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureHostConfiguration(
                configurationBuilder =>
                {
                    Dictionary<string, string?> testConfiguration =
                        new()
                        {
                            /*
                             * AddInfrastructure necesita una conexión
                             * durante su registro inicial.
                             */
                            ["ConnectionStrings:DefaultConnection"] =
                                _sqlServerContainer.GetConnectionString(),

                            /*
                             * Contraseñas falsas para los usuarios
                             * semilla de la base temporal.
                             */
                            ["SeedUsers:AdminPassword"] =
                                TestAdminPassword,

                            ["SeedUsers:UserPassword"] =
                                TestUserPassword,

                            /*
                             * Configuración JWT exclusiva para Testing.
                             */
                            ["Jwt:Key"] =
                                TestJwtKey,

                            ["Jwt:Issuer"] =
                                "Consultoria.IntegrationTests",

                            ["Jwt:Audience"] =
                                "Consultoria.IntegrationTests",

                        };

                    configurationBuilder.AddInMemoryCollection(
                        testConfiguration);
                });

            /*
             * Aquí se ejecuta Program.cs y se construye
             * la aplicación de pruebas.
             */
            IHost host =
                base.CreateHost(builder);

            /*
             * Aplicar las migraciones solamente sobre
             * la base temporal.
             */
            using IServiceScope scope =
                host.Services.CreateScope();

            ConsultoriaDbContext dbContext =
                scope.ServiceProvider
                    .GetRequiredService<ConsultoriaDbContext>();

            dbContext.Database.Migrate();

            return host;
        }

        /*
         * xUnit v2 requiere Task.
         */
        async Task IAsyncLifetime.DisposeAsync()
        {
            await base.DisposeAsync();

            await _sqlServerContainer.DisposeAsync();
        }
    }
}
