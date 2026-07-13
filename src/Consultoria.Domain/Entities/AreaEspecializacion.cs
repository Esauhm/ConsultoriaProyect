using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Domain.Entities
{
    public class AreaEspecializacion
    {
        public int AreaEspecializacionId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private AreaEspecializacion()
        {
        }

        public AreaEspecializacion(string nombre)
        {
            CambiarNombre(nombre);
            Activo = true;
        }

        public void CambiarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException(
                    "El nombre del área es obligatorio.",
                    nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public static AreaEspecializacion Reconstruir(
            int areaEspecializacionId,
            string nombre,
            bool activo)
        {
            return new AreaEspecializacion
            {
                AreaEspecializacionId = areaEspecializacionId,
                Nombre = nombre,
                Activo = activo
            };
        }
    }
}
