using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string entityName, object id)
            : base($"{entityName} con identificador '{id}' no fue encontrado.")
        {
        }
    }
}
