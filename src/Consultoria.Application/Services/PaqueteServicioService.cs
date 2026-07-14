using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Paquetes;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Services
{
    public sealed class PaqueteServicioService : IPaqueteServicioService
    {
        private readonly IPaqueteServicioRepository _paqueteRepository;
        private readonly IConsultorRepository _consultorRepository;
        private readonly ILogger<PaqueteServicioService> _logger;

        public PaqueteServicioService(
            IPaqueteServicioRepository paqueteRepository,
            IConsultorRepository consultorRepository,
            ILogger<PaqueteServicioService> logger)
        {
            _paqueteRepository = paqueteRepository;
            _consultorRepository = consultorRepository;
            _logger = logger;
        }

        public async Task<PaqueteServicioDto> CrearAsync(
            CrearPaqueteServicioDto request,
            CancellationToken cancellationToken = default)
        {
            await ValidarConsultorActivoAsync(
                request.ConsultorId,
                cancellationToken);

            var paquete = new PaqueteServicio(
                request.Nombre.Trim(),
                request.AreaEspecializacionId,
                request.ConsultorId,
                request.DuracionHoras,
                request.Costo,
                request.Descripcion);

            int paqueteId = await _paqueteRepository.CrearAsync(
                paquete,
                cancellationToken);

            _logger.LogInformation(
                "Se creó el paquete de servicio {PaqueteId} asignado al consultor {ConsultorId}.",
                paqueteId,
                request.ConsultorId);

            return await ObtenerPorIdAsync(
                paqueteId,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<PaqueteServicioDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _paqueteRepository.ObtenerTodosAsync(
                cancellationToken);
        }

        public async Task<PaqueteServicioDto> ObtenerPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default)
        {
            PaqueteServicioDto? paquete =
                await _paqueteRepository.ObtenerPorIdAsync(
                    paqueteId,
                    cancellationToken);

            if (paquete is null)
            {
                throw new NotFoundException(
                    "Paquete de servicio",
                    paqueteId);
            }

            return paquete;
        }

        public async Task<PaqueteServicioDto> ActualizarAsync(
            int paqueteId,
            ActualizarPaqueteServicioDto request,
            CancellationToken cancellationToken = default)
        {
            PaqueteServicio? paquete =
                await _paqueteRepository.ObtenerEntidadPorIdAsync(
                    paqueteId,
                    cancellationToken);

            if (paquete is null)
            {
                throw new NotFoundException(
                    "Paquete de servicio",
                    paqueteId);
            }

            await ValidarConsultorActivoAsync(
                request.ConsultorId,
                cancellationToken);

            paquete.Actualizar(
                request.Nombre.Trim(),
                request.AreaEspecializacionId,
                request.ConsultorId,
                request.DuracionHoras,
                request.Costo,
                request.Descripcion);

            await _paqueteRepository.ActualizarAsync(
                paquete,
                cancellationToken);

            _logger.LogInformation(
                "Se actualizó el paquete de servicio {PaqueteId}.",
                paqueteId);

            return await ObtenerPorIdAsync(
                paqueteId,
                cancellationToken);
        }

        public async Task DesactivarAsync(
            int paqueteId,
            CancellationToken cancellationToken = default)
        {
            PaqueteServicio? paquete =
                await _paqueteRepository.ObtenerEntidadPorIdAsync(
                    paqueteId,
                    cancellationToken);

            if (paquete is null)
            {
                throw new NotFoundException(
                    "Paquete de servicio",
                    paqueteId);
            }

            if (!paquete.Activo)
            {
                throw new BusinessException(
                    "El paquete de servicio ya se encuentra inactivo.");
            }

            await _paqueteRepository.DesactivarAsync(
                paqueteId,
                cancellationToken);

            _logger.LogInformation(
                "Se desactivó el paquete de servicio {PaqueteId}.",
                paqueteId);
        }

        private async Task ValidarConsultorActivoAsync(
            int consultorId,
            CancellationToken cancellationToken)
        {
            bool consultorActivo =
                await _consultorRepository.ExisteActivoAsync(
                    consultorId,
                    cancellationToken);

            if (!consultorActivo)
            {
                throw new BusinessException(
                    "El consultor seleccionado no existe o se encuentra inactivo.");
            }
        }
    }
}
