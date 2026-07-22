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

        private static readonly ConsultorSeed[] ConsultoresIniciales =
        [
            new(
            "Ana Martínez",
            "Tecnología",
            45m,
            "ana.martinez@demo.com"),

        new(
            "Carlos López",
            "Finanzas",
            50m,
            "carlos.lopez@demo.com"),

        new(
            "Laura Hernández",
            "Recursos Humanos",
            40m,
            "laura.hernandez@demo.com"),

        new(
            "Miguel Rivera",
            "Marketing",
            42m,
            "miguel.rivera@demo.com"),

        new(
            "Sofía Gómez",
            "Auditoría",
            55m,
            "sofia.gomez@demo.com")
        ];

        private static readonly PaqueteSeed[] PaquetesIniciales =
        [
            new(
            "Desarrollo de API",
            "ana.martinez@demo.com",
            20,
            "Diseño y desarrollo de una API empresarial."),

        new(
            "Arquitectura de Software",
            "ana.martinez@demo.com",
            12,
            "Evaluación y diseño de arquitectura de software."),

        new(
            "Análisis Financiero",
            "carlos.lopez@demo.com",
            10,
            "Análisis de costos, ingresos y rentabilidad."),

        new(
            "Optimización de Recursos Humanos",
            "laura.hernandez@demo.com",
            8,
            "Evaluación y mejora de procesos internos."),

        new(
            "Estrategia de Marketing",
            "miguel.rivera@demo.com",
            15,
            "Diseño de estrategia comercial y posicionamiento."),

        new(
            "Auditoría de Procesos",
            "sofia.gomez@demo.com",
            18,
            "Revisión de controles y procesos operativos.")
        ];

        public static void Seed(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Primero se crean las áreas porque los consultores
            // necesitan AreaEspecializacionId.
            SeedAreas(context);
            context.SaveChanges();

            // Luego se crean los consultores porque los paquetes
            // necesitan ConsultorId.
            SeedConsultores(context);
            context.SaveChanges();

            // Finalmente se crean paquetes y usuarios.
            SeedPaquetes(context);

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

            await context.SaveChangesAsync(
                cancellationToken);

            await SeedConsultoresAsync(
                context,
                cancellationToken);

            await context.SaveChangesAsync(
                cancellationToken);

            await SeedPaquetesAsync(
                context,
                cancellationToken);

            await SeedUsuariosAsync(
                context,
                adminPassword,
                userPassword,
                cancellationToken);

            await context.SaveChangesAsync(
                cancellationToken);
        }

        // =========================================================
        // ÁREAS DE ESPECIALIZACIÓN
        // =========================================================

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
                if (areasExistentes.Contains(nombreArea))
                {
                    continue;
                }

                context.AreasEspecializacion.Add(
                    new AreaEspecializacion(nombreArea));

                areasExistentes.Add(nombreArea);
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

            HashSet<string> areasExistentes =
                nombresExistentes.ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            foreach (string nombreArea in AreasIniciales)
            {
                if (areasExistentes.Contains(nombreArea))
                {
                    continue;
                }

                await context.AreasEspecializacion.AddAsync(
                    new AreaEspecializacion(nombreArea),
                    cancellationToken);

                areasExistentes.Add(nombreArea);
            }
        }

        // =========================================================
        // CONSULTORES
        // =========================================================

        private static void SeedConsultores(
            ConsultoriaDbContext context)
        {
            List<AreaEspecializacion> listaAreas = context
                .AreasEspecializacion
                .AsNoTracking()
                .ToList();

            Dictionary<string, int> areas =
                listaAreas.ToDictionary(
                    area => area.Nombre,
                    area => area.AreaEspecializacionId,
                    StringComparer.OrdinalIgnoreCase);

            HashSet<string> emailsExistentes = context
                .Consultores
                .AsNoTracking()
                .Select(consultor =>
                    consultor.EmailCorporativo)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            foreach (ConsultorSeed datos in ConsultoresIniciales)
            {
                if (emailsExistentes.Contains(
                    datos.EmailCorporativo))
                {
                    continue;
                }

                if (!areas.TryGetValue(
                    datos.AreaNombre,
                    out int areaEspecializacionId))
                {
                    throw new InvalidOperationException(
                        $"No se encontró el área " +
                        $"'{datos.AreaNombre}' para crear " +
                        $"el consultor '{datos.Nombre}'.");
                }

                var consultor = new Consultor(
                    datos.Nombre,
                    areaEspecializacionId,
                    datos.TarifaHora,
                    datos.EmailCorporativo);

                context.Consultores.Add(consultor);

                emailsExistentes.Add(
                    datos.EmailCorporativo);
            }
        }

        private static async Task SeedConsultoresAsync(
            ConsultoriaDbContext context,
            CancellationToken cancellationToken)
        {
            List<AreaEspecializacion> listaAreas = await context
                .AreasEspecializacion
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            Dictionary<string, int> areas =
                listaAreas.ToDictionary(
                    area => area.Nombre,
                    area => area.AreaEspecializacionId,
                    StringComparer.OrdinalIgnoreCase);

            List<string> emails = await context
                .Consultores
                .AsNoTracking()
                .Select(consultor =>
                    consultor.EmailCorporativo)
                .ToListAsync(cancellationToken);

            HashSet<string> emailsExistentes =
                emails.ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            foreach (ConsultorSeed datos in ConsultoresIniciales)
            {
                if (emailsExistentes.Contains(
                    datos.EmailCorporativo))
                {
                    continue;
                }

                if (!areas.TryGetValue(
                    datos.AreaNombre,
                    out int areaEspecializacionId))
                {
                    throw new InvalidOperationException(
                        $"No se encontró el área " +
                        $"'{datos.AreaNombre}' para crear " +
                        $"el consultor '{datos.Nombre}'.");
                }

                var consultor = new Consultor(
                    datos.Nombre,
                    areaEspecializacionId,
                    datos.TarifaHora,
                    datos.EmailCorporativo);

                await context.Consultores.AddAsync(
                    consultor,
                    cancellationToken);

                emailsExistentes.Add(
                    datos.EmailCorporativo);
            }
        }

        // =========================================================
        // PAQUETES DE SERVICIO
        // =========================================================

        private static void SeedPaquetes(
            ConsultoriaDbContext context)
        {
            List<Consultor> listaConsultores = context
                .Consultores
                .AsNoTracking()
                .ToList();

            Dictionary<string, Consultor> consultores =
                listaConsultores.ToDictionary(
                    consultor =>
                        consultor.EmailCorporativo,
                    StringComparer.OrdinalIgnoreCase);

            HashSet<string> paquetesExistentes = context
                .PaquetesServicio
                .AsNoTracking()
                .Select(paquete => paquete.Nombre)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            foreach (PaqueteSeed datos in PaquetesIniciales)
            {
                if (paquetesExistentes.Contains(
                    datos.Nombre))
                {
                    continue;
                }

                if (!consultores.TryGetValue(
                    datos.ConsultorEmail,
                    out Consultor? consultor))
                {
                    throw new InvalidOperationException(
                        $"No se encontró el consultor " +
                        $"'{datos.ConsultorEmail}' para crear " +
                        $"el paquete '{datos.Nombre}'.");
                }

                var paquete = new PaqueteServicio(
                    datos.Nombre,
                    consultor.AreaEspecializacionId,
                    consultor.ConsultorId,
                    datos.DuracionHoras,
                    consultor.TarifaHora,
                    datos.Descripcion);

                context.PaquetesServicio.Add(paquete);

                paquetesExistentes.Add(
                    datos.Nombre);
            }
        }

        private static async Task SeedPaquetesAsync(
            ConsultoriaDbContext context,
            CancellationToken cancellationToken)
        {
            List<Consultor> listaConsultores = await context
                .Consultores
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            Dictionary<string, Consultor> consultores =
                listaConsultores.ToDictionary(
                    consultor =>
                        consultor.EmailCorporativo,
                    StringComparer.OrdinalIgnoreCase);

            List<string> nombresPaquetes = await context
                .PaquetesServicio
                .AsNoTracking()
                .Select(paquete => paquete.Nombre)
                .ToListAsync(cancellationToken);

            HashSet<string> paquetesExistentes =
                nombresPaquetes.ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            foreach (PaqueteSeed datos in PaquetesIniciales)
            {
                if (paquetesExistentes.Contains(
                    datos.Nombre))
                {
                    continue;
                }

                if (!consultores.TryGetValue(
                    datos.ConsultorEmail,
                    out Consultor? consultor))
                {
                    throw new InvalidOperationException(
                        $"No se encontró el consultor " +
                        $"'{datos.ConsultorEmail}' para crear " +
                        $"el paquete '{datos.Nombre}'.");
                }

                var paquete = new PaqueteServicio(
                    datos.Nombre,
                    consultor.AreaEspecializacionId,
                    consultor.ConsultorId,
                    datos.DuracionHoras,
                    consultor.TarifaHora,
                    datos.Descripcion);

                await context.PaquetesServicio.AddAsync(
                    paquete,
                    cancellationToken);

                paquetesExistentes.Add(
                    datos.Nombre);
            }
        }

        // =========================================================
        // USUARIOS
        // =========================================================

        private static void SeedUsuarios(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword)
        {
            ValidarPasswords(
                adminPassword,
                userPassword);

            var passwordHasher = new PasswordHasher();

            bool existeAdmin = context
                .Usuarios
                .AsNoTracking()
                .Any(usuario =>
                    usuario.Email == AdminEmail);

            if (!existeAdmin)
            {
                context.Usuarios.Add(
                    new Usuario(
                        "Administrador",
                        AdminEmail,
                        passwordHasher.GenerarHash(
                            adminPassword),
                        "Admin"));
            }

            bool existeUsuario = context
                .Usuarios
                .AsNoTracking()
                .Any(usuario =>
                    usuario.Email == UserEmail);

            if (!existeUsuario)
            {
                context.Usuarios.Add(
                    new Usuario(
                        "Usuario Demo",
                        UserEmail,
                        passwordHasher.GenerarHash(
                            userPassword),
                        "User"));
            }
        }

        private static async Task SeedUsuariosAsync(
            ConsultoriaDbContext context,
            string adminPassword,
            string userPassword,
            CancellationToken cancellationToken)
        {
            ValidarPasswords(
                adminPassword,
                userPassword);

            var passwordHasher = new PasswordHasher();

            bool existeAdmin = await context
                .Usuarios
                .AsNoTracking()
                .AnyAsync(
                    usuario =>
                        usuario.Email == AdminEmail,
                    cancellationToken);

            if (!existeAdmin)
            {
                await context.Usuarios.AddAsync(
                    new Usuario(
                        "Administrador",
                        AdminEmail,
                        passwordHasher.GenerarHash(
                            adminPassword),
                        "Admin"),
                    cancellationToken);
            }

            bool existeUsuario = await context
                .Usuarios
                .AsNoTracking()
                .AnyAsync(
                    usuario =>
                        usuario.Email == UserEmail,
                    cancellationToken);

            if (!existeUsuario)
            {
                await context.Usuarios.AddAsync(
                    new Usuario(
                        "Usuario Demo",
                        UserEmail,
                        passwordHasher.GenerarHash(
                            userPassword),
                        "User"),
                    cancellationToken);
            }
        }

        // =========================================================
        // VALIDACIONES
        // =========================================================

        private static void ValidarPasswords(
            string adminPassword,
            string userPassword)
        {
            if (string.IsNullOrWhiteSpace(
                adminPassword))
            {
                throw new InvalidOperationException(
                    "No se configuró " +
                    "'SeedUsers:AdminPassword'.");
            }

            if (string.IsNullOrWhiteSpace(
                userPassword))
            {
                throw new InvalidOperationException(
                    "No se configuró " +
                    "'SeedUsers:UserPassword'.");
            }
        }

        // =========================================================
        // MODELOS INTERNOS DEL SEEDER
        // =========================================================

        private sealed record ConsultorSeed(
            string Nombre,
            string AreaNombre,
            decimal TarifaHora,
            string EmailCorporativo);

        private sealed record PaqueteSeed(
            string Nombre,
            string ConsultorEmail,
            int DuracionHoras,
            string Descripcion);
    }
}
