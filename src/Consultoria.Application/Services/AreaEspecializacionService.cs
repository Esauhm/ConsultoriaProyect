using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.AreasEspecializacion;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace Consultoria.Application.Services
{
    public sealed class AreaEspecializacionService
    : IAreaEspecializacionService
    {
        private readonly IAreaEspecializacionRepository _areaRepository;
        private readonly ILogger<AreaEspecializacionService> _logger;

        public AreaEspecializacionService(
            IAreaEspecializacionRepository areaRepository,
            ILogger<AreaEspecializacionService> logger)
        {
            _areaRepository = areaRepository;
            _logger = logger;
        }

        public async Task<AreaEspecializacionDto> CrearAsync(
            CrearAreaEspecializacionDto request,
            CancellationToken cancellationToken = default)
        {
            string nombre = NormalizarNombre(request.Nombre);

            bool nombreDuplicado =
                await _areaRepository.ExisteNombreAsync(
                    nombre,
                    cancellationToken: cancellationToken);

            if (nombreDuplicado)
            {
                throw new ConflictException(
                    $"Ya existe un área de especialización con el nombre '{nombre}'.");
            }

            var area = new AreaEspecializacion(nombre);

            int areaId = await _areaRepository.CrearAsync(
                area,
                cancellationToken);

            _logger.LogInformation(
                "Área de especialización creada. AreaEspecializacionId: {AreaEspecializacionId}, Nombre: {Nombre}",
                areaId,
                nombre);

            AreaEspecializacionDto? resultado =
                await _areaRepository.ObtenerPorIdAsync(
                    areaId,
                    cancellationToken);

            return resultado
                ?? throw new InvalidOperationException(
                    "El área fue creada, pero no pudo recuperarse.");
        }

        public async Task<AreaEspecializacionDto> ObtenerPorIdAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            AreaEspecializacionDto? area =
                await _areaRepository.ObtenerPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken);

            return area
                ?? throw new NotFoundException(
                    $"No se encontró el área de especialización con identificador {areaEspecializacionId}.");
        }

        public async Task<IReadOnlyCollection<AreaEspecializacionDto>>
            ObtenerTodasAsync(
                CancellationToken cancellationToken = default)
        {
            return await _areaRepository.ObtenerTodasAsync(
                cancellationToken);
        }

        public async Task<AreaEspecializacionDto> ActualizarAsync(
            int areaEspecializacionId,
            ActualizarAreaEspecializacionDto request,
            CancellationToken cancellationToken = default)
        {
            AreaEspecializacion? area =
                await _areaRepository.ObtenerEntidadPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken);

            if (area is null)
            {
                throw new NotFoundException(
                    $"No se encontró el área de especialización con identificador {areaEspecializacionId}.");
            }

            string nombre = NormalizarNombre(request.Nombre);

            bool nombreDuplicado =
                await _areaRepository.ExisteNombreAsync(
                    nombre,
                    areaEspecializacionId,
                    cancellationToken);

            if (nombreDuplicado)
            {
                throw new ConflictException(
                    $"Ya existe otra área de especialización con el nombre '{nombre}'.");
            }

            area.ActualizarNombre(nombre);

            await _areaRepository.ActualizarAsync(
                area,
                cancellationToken);

            _logger.LogInformation(
                "Área de especialización actualizada. AreaEspecializacionId: {AreaEspecializacionId}, Nombre: {Nombre}",
                areaEspecializacionId,
                nombre);

            AreaEspecializacionDto? resultado =
                await _areaRepository.ObtenerPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken);

            return resultado
                ?? throw new InvalidOperationException(
                    "El área fue actualizada, pero no pudo recuperarse.");
        }

        public async Task DesactivarAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            AreaEspecializacion? area =
                await _areaRepository.ObtenerEntidadPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken);

            if (area is null)
            {
                throw new NotFoundException(
                    $"No se encontró el área de especialización con identificador {areaEspecializacionId}.");
            }

            if (!area.Activo)
            {
                throw new BusinessException(
                    "El área de especialización ya se encuentra desactivada.");
            }

            await _areaRepository.DesactivarAsync(
                areaEspecializacionId,
                cancellationToken);

            _logger.LogInformation(
                "Área de especialización desactivada. AreaEspecializacionId: {AreaEspecializacionId}",
                areaEspecializacionId);
        }

        public async Task<AreaEspecializacionDto> ActivarAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            AreaEspecializacion area =
                await _areaRepository.ObtenerEntidadPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken)
                ?? throw new NotFoundException(
                    $"No se encontró el área de especialización " +
                    $"con identificador {areaEspecializacionId}.");

            if (area.Activo)
            {
                throw new BusinessException(
                    "El área de especialización ya se encuentra activa.");
            }

            area.Activar();

            await _areaRepository.ActualizarAsync(
                area,
                cancellationToken);

            _logger.LogInformation(
                "Área de especialización reactivada. " +
                "AreaEspecializacionId: {AreaEspecializacionId}",
                areaEspecializacionId);

            AreaEspecializacionDto? resultado =
                await _areaRepository.ObtenerPorIdAsync(
                    areaEspecializacionId,
                    cancellationToken);

            return resultado
                ?? throw new InvalidOperationException(
                    "El área fue reactivada, pero no pudo recuperarse.");
        }

        private static string NormalizarNombre(string nombre)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

            return string.Join(
                " ",
                nombre.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries));
        }
    }
}
