using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface ICacheService
    {
        T? Obtener<T>(string clave);

        void Guardar<T>(
            string clave,
            T valor,
            TimeSpan duracion);

        void Eliminar(string clave);
    }
}
