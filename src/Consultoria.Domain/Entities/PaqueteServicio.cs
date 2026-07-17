using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Domain.Entities
{
    public sealed class PaqueteServicio
    {
        public int PaqueteId { get; private set; }

        public string Nombre { get; private set; } = string.Empty;

        public int AreaEspecializacionId { get; private set; }

        public int ConsultorId { get; private set; }

        public int DuracionHoras { get; private set; }

        public decimal TarifaHoraAplicada { get; private set; }

        public decimal Costo { get; private set; }

        public string Descripcion { get; private set; } = string.Empty;

        public bool Activo { get; private set; }

        public DateTime FechaCreacion { get; private set; }

        private PaqueteServicio()
        {
        }

        public PaqueteServicio(
            string nombre,
            int areaEspecializacionId,
            int consultorId,
            int duracionHoras,
            decimal tarifaHoraAplicada,
            string descripcion)
        {
            ValidarDatos(
                nombre,
                areaEspecializacionId,
                consultorId,
                duracionHoras,
                tarifaHoraAplicada,
                descripcion);

            Nombre = nombre.Trim();
            AreaEspecializacionId = areaEspecializacionId;
            ConsultorId = consultorId;
            DuracionHoras = duracionHoras;
            TarifaHoraAplicada = tarifaHoraAplicada;
            Costo = CalcularCosto(
                duracionHoras,
                tarifaHoraAplicada);
            Descripcion = descripcion.Trim();
            Activo = true;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Actualizar(
            string nombre,
            int areaEspecializacionId,
            int consultorId,
            int duracionHoras,
            decimal tarifaHoraAplicada,
            string descripcion)
        {
            ValidarDatos(
                nombre,
                areaEspecializacionId,
                consultorId,
                duracionHoras,
                tarifaHoraAplicada,
                descripcion);

            Nombre = nombre.Trim();
            AreaEspecializacionId = areaEspecializacionId;
            ConsultorId = consultorId;
            DuracionHoras = duracionHoras;
            TarifaHoraAplicada = tarifaHoraAplicada;
            Costo = CalcularCosto(
                duracionHoras,
                tarifaHoraAplicada);
            Descripcion = descripcion.Trim();
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public static PaqueteServicio Reconstruir(
            int paqueteId,
            string nombre,
            int areaEspecializacionId,
            int consultorId,
            int duracionHoras,
            decimal tarifaHoraAplicada,
            decimal costo,
            string descripcion,
            bool activo,
            DateTime fechaCreacion)
        {
            return new PaqueteServicio
            {
                PaqueteId = paqueteId,
                Nombre = nombre,
                AreaEspecializacionId = areaEspecializacionId,
                ConsultorId = consultorId,
                DuracionHoras = duracionHoras,
                TarifaHoraAplicada = tarifaHoraAplicada,
                Costo = costo,
                Descripcion = descripcion,
                Activo = activo,
                FechaCreacion = fechaCreacion
            };
        }

        private static decimal CalcularCosto(
            int duracionHoras,
            decimal tarifaHoraAplicada)
        {
            return duracionHoras * tarifaHoraAplicada;
        }

        private static void ValidarDatos(
            string nombre,
            int areaEspecializacionId,
            int consultorId,
            int duracionHoras,
            decimal tarifaHoraAplicada,
            string descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "El nombre del paquete es obligatorio.",
                    nameof(nombre));
            }

            if (areaEspecializacionId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(areaEspecializacionId),
                    "El área de especialización debe ser válida.");
            }

            if (consultorId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(consultorId),
                    "El consultor debe ser válido.");
            }

            if (duracionHoras <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(duracionHoras),
                    "La duración debe ser mayor que cero.");
            }

            if (tarifaHoraAplicada <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tarifaHoraAplicada),
                    "La tarifa por hora debe ser mayor que cero.");
            }

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException(
                    "La descripción del paquete es obligatoria.",
                    nameof(descripcion));
            }
        }
    }
}
