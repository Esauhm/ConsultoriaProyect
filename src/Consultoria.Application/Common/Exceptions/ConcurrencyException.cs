using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Exceptions
{
    public sealed class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        public ConcurrencyException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
