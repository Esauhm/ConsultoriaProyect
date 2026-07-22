using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Paquetes
{
    public sealed class PaqueteServicioDto
    {
        public int PaqueteId { get; init; }

        public string Nombre { get; init; } = string.Empty;

        public int AreaEspecializacionId { get; init; }

        public string AreaEspecializacion { get; init; } = string.Empty;

        public int ConsultorId { get; init; }

        public string Consultor { get; init; } = string.Empty;

        public int DuracionHoras { get; init; }

        public decimal TarifaHoraAplicada { get; init; }

        public decimal Costo { get; init; }

        public string? Descripcion { get; init; }

        public bool Activo { get; init; }

        public DateTime FechaCreacion { get; init; }

        public byte[] RowVersion { get; init; } =
            Array.Empty<byte>();
    }
}
