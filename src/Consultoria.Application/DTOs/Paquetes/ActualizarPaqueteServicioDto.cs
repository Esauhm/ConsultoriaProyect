using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Paquetes
{
    public sealed class ActualizarPaqueteServicioDto
    {
        public string Nombre { get; init; } = string.Empty;

        public int AreaEspecializacionId { get; init; }

        public int ConsultorId { get; init; }

        public int DuracionHoras { get; init; }

        public decimal Costo { get; init; }

        public string? Descripcion { get; init; }
    }
}
