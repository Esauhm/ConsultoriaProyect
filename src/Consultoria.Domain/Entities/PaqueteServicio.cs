using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Domain.Entities
{
    public class PaqueteServicio
    {
        public int PaqueteId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public int AreaEspecializacionId { get; private set; }
        public int ConsultorId { get; private set; }
        public int DuracionHoras { get; private set; }
        public decimal Costo { get; private set; }
        public string? Descripcion { get; private set; }
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
            decimal costo,
            string? descripcion)
        {
            CambiarNombre(nombre);
            CambiarAreaEspecializacion(areaEspecializacionId);
            CambiarConsultor(consultorId);
            CambiarDuracion(duracionHoras);
            CambiarCosto(costo);

            Descripcion = NormalizarDescripcion(descripcion);
            Activo = true;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Actualizar(
            string nombre,
            int areaEspecializacionId,
            int consultorId,
            int duracionHoras,
            decimal costo,
            string? descripcion)
        {
            CambiarNombre(nombre);
            CambiarAreaEspecializacion(areaEspecializacionId);
            CambiarConsultor(consultorId);
            CambiarDuracion(duracionHoras);
            CambiarCosto(costo);

            Descripcion = NormalizarDescripcion(descripcion);
        }

        public void CambiarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException(
                    "El nombre del paquete es obligatorio.",
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

        public void CambiarConsultor(int consultorId)
        {
            if (consultorId <= 0)
                throw new ArgumentException(
                    "El consultor es obligatorio.",
                    nameof(consultorId));

            ConsultorId = consultorId;
        }

        public void CambiarDuracion(int duracionHoras)
        {
            if (duracionHoras <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(duracionHoras),
                    "La duración debe ser mayor que cero.");

            DuracionHoras = duracionHoras;
        }

        public void CambiarCosto(decimal costo)
        {
            if (costo <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(costo),
                    "El costo debe ser mayor que cero.");

            Costo = costo;
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
            decimal costo,
            string? descripcion,
            bool activo,
            DateTime fechaCreacion)
        {
            var paquete = new PaqueteServicio
            {
                PaqueteId = paqueteId,
                Nombre = nombre,
                AreaEspecializacionId = areaEspecializacionId,
                ConsultorId = consultorId,
                DuracionHoras = duracionHoras,
                Costo = costo,
                Descripcion = descripcion,
                Activo = activo,
                FechaCreacion = fechaCreacion
            };

            return paquete;
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                ? null
                : descripcion.Trim();
        }
    }
}
