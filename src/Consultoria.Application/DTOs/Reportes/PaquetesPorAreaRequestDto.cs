using Consultoria.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Reportes
{
    public sealed class PaquetesPorAreaRequestDto : PaginationRequest
    {
        public int? AreaEspecializacionId { get; init; }

        public string? NombrePaquete { get; init; }

        public bool? Activo { get; init; }
    }
}
