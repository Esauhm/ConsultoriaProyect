using Consultoria.Domain.Entities;
using Consultoria.Infrastructure.Authentication;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace Consultoria.Infrastructure.Persistence.Seeding
{
    public static class DatabaseSeeder
    {
        private const string AdminEmail = "admin@demo.com";
        private const string UserEmail = "usuario@demo.com";

        private static readonly string[] AreasIniciales =
        [
            "Tecnología",
        "Finanzas",
        "Recursos Humanos",
        "Marketing",
        "Auditoría"
        ];

        public static void Seed(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword)
        {
            ArgumentNullException.ThrowIfNull(context);

            SeedAreas(context);
            SeedUsuarios(
                context,
                adminPassword,
                userPassword);

            context.SaveChanges();
        }

        public static async Task SeedAsync(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            await SeedAreasAsync(
                context,
                cancellationToken);

            await SeedUsuariosAsync(
                context,
                adminPassword,
                userPassword,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        private static void SeedAreas(
            ConsultoriaDbContext context)
        {
            HashSet<string> areasExistentes = context
                .AreasEspecializacion
                .AsNoTracking()
                .Select(area => area.Nombre)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (string nombreArea in AreasIniciales)
            {
                if (!areasExistentes.Contains(nombreArea))
                {
                    context.AreasEspecializacion.Add(
                        new AreaEspecializacion(nombreArea));
                }
            }
        }

        private static async Task SeedAreasAsync(
            ConsultoriaDbContext context,
            CancellationToken cancellationToken)
        {
            List<string> nombresExistentes = await context
                .AreasEspecializacion
                .AsNoTracking()
                .Select(area => area.Nombre)
                .ToListAsync(cancellationToken);

            var areasExistentes = nombresExistentes.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

            foreach (string nombreArea in AreasIniciales)
            {
                if (!areasExistentes.Contains(nombreArea))
                {
                    await context.AreasEspecializacion.AddAsync(
                        new AreaEspecializacion(nombreArea),
                        cancellationToken);
                }
            }
        }

        private static void SeedUsuarios(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword)
        {
            ValidarPasswords(adminPassword, userPassword);

            var passwordHasher = new PasswordHasher();

            bool existeAdmin = context.Usuarios
                .AsNoTracking()
                .Any(usuario => usuario.Email == AdminEmail);

            if (!existeAdmin)
            {
                context.Usuarios.Add(
                    new Usuario(
                        "Administrador",
                        AdminEmail,
                        passwordHasher.GenerarHash(adminPassword),
                        "Admin"));
            }

            bool existeUsuario = context.Usuarios
                .AsNoTracking()
                .Any(usuario => usuario.Email == UserEmail);

            if (!existeUsuario)
            {
                context.Usuarios.Add(
                    new Usuario(
                        "Usuario Demo",
                        UserEmail,
                        passwordHasher.GenerarHash(userPassword),
                        "User"));
            }
        }

        private static async Task SeedUsuariosAsync(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword,
            CancellationToken cancellationToken)
        {
            ValidarPasswords(adminPassword, userPassword);

            var passwordHasher = new PasswordHasher();

            bool existeAdmin = await context.Usuarios
                .AsNoTracking()
                .AnyAsync(
                    usuario => usuario.Email == AdminEmail,
                    cancellationToken);

            if (!existeAdmin)
            {
                await context.Usuarios.AddAsync(
                    new Usuario(
                        "Administrador",
                        AdminEmail,
                        passwordHasher.GenerarHash(adminPassword),
                        "Admin"),
                    cancellationToken);
            }

            bool existeUsuario = await context.Usuarios
                .AsNoTracking()
                .AnyAsync(
                    usuario => usuario.Email == UserEmail,
                    cancellationToken);

            if (!existeUsuario)
            {
                await context.Usuarios.AddAsync(
                    new Usuario(
                        "Usuario Demo",
                        UserEmail,
                        passwordHasher.GenerarHash(userPassword),
                        "User"),
                    cancellationToken);
            }
        }

        private static void ValidarPasswords(
            string adminPassword,
            string userPassword)
        {
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "No se configuró 'SeedUsers:AdminPassword'.");
            }

            if (string.IsNullOrWhiteSpace(userPassword))
            {
                throw new InvalidOperationException(
                    "No se configuró 'SeedUsers:UserPassword'.");
            }
        }
    }
}
