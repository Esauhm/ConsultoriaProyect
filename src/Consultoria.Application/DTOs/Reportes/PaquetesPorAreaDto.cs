using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Reportes
{
    public sealed class PaquetesPorAreaDto
    {
        public int AreaEspecializacionId { get; init; }

        public string AreaEspecializacion { get; init; } = string.Empty;

        public int CantidadPaquetes { get; init; }

        public int TotalHoras { get; init; }

        public decimal CostoTotal { get; init; }

        public decimal CostoPromedio { get; init; }
    }
}
