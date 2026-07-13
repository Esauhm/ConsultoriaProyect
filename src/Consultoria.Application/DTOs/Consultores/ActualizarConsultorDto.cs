using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Consultores
{
    public sealed class ActualizarConsultorDto
    {
        public string Nombre { get; init; } = string.Empty;

        public int AreaEspecializacionId { get; init; }

        public decimal TarifaHora { get; init; }

        public string EmailCorporativo { get; init; } = string.Empty;
    }
}
