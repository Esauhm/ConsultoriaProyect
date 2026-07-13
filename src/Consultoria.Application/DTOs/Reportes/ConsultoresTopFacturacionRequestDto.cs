using Consultoria.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Reportes
{
    public sealed class ConsultoresTopFacturacionRequestDto : PaginationRequest
    {
        public int? AreaEspecializacionId { get; init; }

        public string? NombreConsultor { get; init; }

        public bool? Activo { get; init; }
    }
}
