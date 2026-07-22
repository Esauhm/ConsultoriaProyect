using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.AreasEspecializacion
{
    public sealed class AreaEspecializacionDto
    {
        public int AreaEspecializacionId { get; init; }

        public string Nombre { get; init; } = string.Empty;

        public bool Activo { get; init; }

        public byte[] RowVersion { get; init; } =
        Array.Empty<byte>();
    }
}
