using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Consultores
{
    public sealed class ConsultorDto
    {
        public int ConsultorId { get; init; }

        public string Nombre { get; init; } = string.Empty;

        public int AreaEspecializacionId { get; init; }

        public string AreaEspecializacion { get; init; } = string.Empty;

        public decimal TarifaHora { get; init; }

        public string EmailCorporativo { get; init; } = string.Empty;

        public bool Activo { get; init; }

        public DateTime FechaIngreso { get; init; }

        public byte[] RowVersion { get; init; } =
            Array.Empty<byte>();
    }
}
