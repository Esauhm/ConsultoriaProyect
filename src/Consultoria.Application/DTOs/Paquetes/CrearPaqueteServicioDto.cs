using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Paquetes
{
    public sealed class CrearPaqueteServicioDto
    {
        public string Nombre { get; init; } = string.Empty;

        public int ConsultorId { get; init; }

        public int DuracionHoras { get; init; }

        public string Descripcion { get; init; } = string.Empty;
    }
}
