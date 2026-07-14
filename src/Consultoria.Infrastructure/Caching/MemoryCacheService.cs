using Consultoria.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Caching
{
    public sealed class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Obtener<T>(string clave)
        {
            ValidarClave(clave);

            return _memoryCache.TryGetValue(
                clave,
                out T? valor)
                    ? valor
                    : default;
        }

        public void Guardar<T>(
            string clave,
            T valor,
            TimeSpan duracion)
        {
            ValidarClave(clave);

            ArgumentNullException.ThrowIfNull(valor);

            if (duracion <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(duracion),
                    "La duración del caché debe ser mayor que cero.");
            }

            var opciones = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duracion
            };

            _memoryCache.Set(
                clave,
                valor,
                opciones);
        }

        public void Eliminar(string clave)
        {
            ValidarClave(clave);

            _memoryCache.Remove(clave);
        }

        private static void ValidarClave(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ArgumentException(
                    "La clave del caché es obligatoria.",
                    nameof(clave));
            }
        }
    }
}
