using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
