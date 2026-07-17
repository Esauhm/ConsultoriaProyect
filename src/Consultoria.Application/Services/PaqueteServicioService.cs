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
        private readonly IAreaEspecializacionRepository _areaRepository;
        private readonly ILogger<PaqueteServicioService> _logger;

        public PaqueteServicioService(
            IPaqueteServicioRepository paqueteRepository,
            IConsultorRepository consultorRepository,
            IAreaEspecializacionRepository areaRepository,
            ILogger<PaqueteServicioService> logger)
        {
            _paqueteRepository = paqueteRepository;
            _consultorRepository = consultorRepository;
            _areaRepository = areaRepository;
            _logger = logger;
        }

        public async Task<PaqueteServicioDto> CrearAsync(
            CrearPaqueteServicioDto request,
            CancellationToken cancellationToken = default)
        {
            Consultor consultor =
                await ObtenerConsultorValidoAsync(
                    request.ConsultorId,
                    cancellationToken);

            var paquete = new PaqueteServicio(
                request.Nombre.Trim(),
                consultor.AreaEspecializacionId,
                consultor.ConsultorId,
                request.DuracionHoras,
                consultor.TarifaHora,
                request.Descripcion.Trim());

            int paqueteId =
                await _paqueteRepository.CrearAsync(
                    paquete,
                    cancellationToken);

            _logger.LogInformation(
                "Se creó el paquete {PaqueteId} para el consultor {ConsultorId}. " +
                "Área: {AreaEspecializacionId}, Tarifa aplicada: {TarifaHoraAplicada}, " +
                "Duración: {DuracionHoras}, Costo: {Costo}.",
                paqueteId,
                consultor.ConsultorId,
                consultor.AreaEspecializacionId,
                consultor.TarifaHora,
                request.DuracionHoras,
                paquete.Costo);

            return await ObtenerPorIdAsync(
                paqueteId,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<PaqueteServicioDto>>
            ObtenerTodosAsync(
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

            Consultor consultor =
                await ObtenerConsultorValidoAsync(
                    request.ConsultorId,
                    cancellationToken);

            paquete.Actualizar(
                request.Nombre.Trim(),
                consultor.AreaEspecializacionId,
                consultor.ConsultorId,
                request.DuracionHoras,
                consultor.TarifaHora,
                request.Descripcion.Trim());

            await _paqueteRepository.ActualizarAsync(
                paquete,
                cancellationToken);

            _logger.LogInformation(
                "Se actualizó el paquete {PaqueteId}. " +
                "Consultor: {ConsultorId}, Área: {AreaEspecializacionId}, " +
                "Tarifa aplicada: {TarifaHoraAplicada}, Costo: {Costo}.",
                paqueteId,
                consultor.ConsultorId,
                consultor.AreaEspecializacionId,
                consultor.TarifaHora,
                paquete.Costo);

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

        private async Task<Consultor> ObtenerConsultorValidoAsync(
            int consultorId,
            CancellationToken cancellationToken)
        {
            Consultor? consultor =
                await _consultorRepository.ObtenerEntidadPorIdAsync(
                    consultorId,
                    cancellationToken);

            if (consultor is null)
            {
                throw new BusinessException(
                    $"El consultor con identificador {consultorId} no existe.");
            }

            if (!consultor.Activo)
            {
                throw new BusinessException(
                    $"El consultor con identificador {consultorId} " +
                    "se encuentra inactivo.");
            }

            bool areaActiva =
                await _areaRepository.ExisteActivaAsync(
                    consultor.AreaEspecializacionId,
                    cancellationToken);

            if (!areaActiva)
            {
                throw new BusinessException(
                    "El área de especialización asignada al consultor " +
                    "no existe o se encuentra inactiva.");
            }

            return consultor;
        }
    }
}
