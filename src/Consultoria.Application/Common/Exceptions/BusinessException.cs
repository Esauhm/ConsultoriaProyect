using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Exceptions
{
    public sealed class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {
        }
    }
}
