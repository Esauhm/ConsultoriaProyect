using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Reportes
{
    public sealed class ConsultorTopFacturacionDto
    {
        public int ConsultorId { get; init; }

        public string NombreConsultor { get; init; } = string.Empty;

        public string AreaEspecializacion { get; init; } = string.Empty;

        public decimal TarifaHora { get; init; }

        public int CantidadPaquetes { get; init; }

        public int TotalHoras { get; init; }

        public decimal TotalFacturado { get; init; }
    }
}
