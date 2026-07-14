using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
