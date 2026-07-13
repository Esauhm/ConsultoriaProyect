using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Domain.Entities
{
    public class Consultor
    {
        public int ConsultorId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public int AreaEspecializacionId { get; private set; }
        public decimal TarifaHora { get; private set; }
        public string EmailCorporativo { get; private set; } = string.Empty;
        public bool Activo { get; private set; }
        public DateTime FechaIngreso { get; private set; }

        private Consultor()
        {
        }

        public Consultor(
            string nombre,
            int areaEspecializacionId,
            decimal tarifaHora,
            string emailCorporativo)
        {
            CambiarNombre(nombre);
            CambiarAreaEspecializacion(areaEspecializacionId);
            CambiarTarifaHora(tarifaHora);
            CambiarEmailCorporativo(emailCorporativo);

            Activo = true;
            FechaIngreso = DateTime.UtcNow;
        }

        public void Actualizar(
            string nombre,
            int areaEspecializacionId,
            decimal tarifaHora,
            string emailCorporativo)
        {
            CambiarNombre(nombre);
            CambiarAreaEspecializacion(areaEspecializacionId);
            CambiarTarifaHora(tarifaHora);
            CambiarEmailCorporativo(emailCorporativo);
        }

        public void CambiarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException(
                    "El nombre del consultor es obligatorio.",
                    nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void CambiarAreaEspecializacion(int areaEspecializacionId)
        {
            if (areaEspecializacionId <= 0)
                throw new ArgumentException(
                    "El área de especialización es obligatoria.",
                    nameof(areaEspecializacionId));

            AreaEspecializacionId = areaEspecializacionId;
        }

        public void CambiarTarifaHora(decimal tarifaHora)
        {
            if (tarifaHora is < 30 or > 200)
                throw new ArgumentOutOfRangeException(
                    nameof(tarifaHora),
                    "La tarifa por hora debe estar entre $30 y $200.");

            TarifaHora = tarifaHora;
        }

        public void CambiarEmailCorporativo(string emailCorporativo)
        {
            if (string.IsNullOrWhiteSpace(emailCorporativo))
                throw new ArgumentException(
                    "El correo corporativo es obligatorio.",
                    nameof(emailCorporativo));

            EmailCorporativo = emailCorporativo
                .Trim()
                .ToLowerInvariant();
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public static Consultor Reconstruir(
            int consultorId,
            string nombre,
            int areaEspecializacionId,
            decimal tarifaHora,
            string emailCorporativo,
            bool activo,
            DateTime fechaIngreso)
        {
            var consultor = new Consultor
            {
                ConsultorId = consultorId,
                Nombre = nombre,
                AreaEspecializacionId = areaEspecializacionId,
                TarifaHora = tarifaHora,
                EmailCorporativo = emailCorporativo,
                Activo = activo,
                FechaIngreso = fechaIngreso
            };

            return consultor;
        }
    }
}