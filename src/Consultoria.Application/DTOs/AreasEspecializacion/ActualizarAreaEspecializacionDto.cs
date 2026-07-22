using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.AreasEspecializacion
{
    public sealed class ActualizarAreaEspecializacionDto
    {
        public string Nombre { get; init; } = string.Empty;

        public byte[] RowVersion { get; init; } =
             Array.Empty<byte>();
    }
}
