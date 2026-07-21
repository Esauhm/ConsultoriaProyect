using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Consultores;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Services
{
    public sealed class ConsultorService : IConsultorService
    {
        private readonly IConsultorRepository _consultorRepository;
        private readonly IAreaEspecializacionRepository _areaRepository;
        private readonly ILogger<ConsultorService> _logger;

        public ConsultorService(
            IConsultorRepository consultorRepository,
            IAreaEspecializacionRepository areaRepository,
            ILogger<ConsultorService> logger)
        {
            _consultorRepository = consultorRepository;
            _areaRepository = areaRepository;
            _logger = logger;
        }

        public async Task<ConsultorDto> CrearAsync(
            CrearConsultorDto request,
            CancellationToken cancellationToken = default)
        {
            string nombreNormalizado = request.Nombre.Trim();

            string emailNormalizado = request.EmailCorporativo
                .Trim()
                .ToLowerInvariant();

            await ValidarAreaActivaAsync(
                request.AreaEspecializacionId,
                cancellationToken);

            await ValidarEmailUnicoAsync(
                emailNormalizado,
                consultorIdExcluir: null,
                cancellationToken);

            await ValidarNombreAreaUnicoAsync(
                nombreNormalizado,
                request.AreaEspecializacionId,
                consultorIdExcluir: null,
                cancellationToken);

            var consultor = new Consultor(
                nombreNormalizado,
                request.AreaEspecializacionId,
                request.TarifaHora,
                emailNormalizado);

            int consultorId =
                await _consultorRepository.CrearAsync(
                    consultor,
                    cancellationToken);

            _logger.LogInformation(
                "Se creó el consultor {ConsultorId} con el correo {EmailCorporativo}.",
                consultorId,
                emailNormalizado);

            return await ObtenerPorIdAsync(
                consultorId,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<ConsultorDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _consultorRepository.ObtenerTodosAsync(
                cancellationToken);
        }

        public async Task<ConsultorDto> ObtenerPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            ConsultorDto? consultor =
                await _consultorRepository.ObtenerPorIdAsync(
                    consultorId,
                    cancellationToken);

            if (consultor is null)
            {
                throw new NotFoundException(
                    "Consultor",
                    consultorId);
            }

            return consultor;
        }

        public async Task<ConsultorDto> ActualizarAsync(
            int consultorId,
            ActualizarConsultorDto request,
            CancellationToken cancellationToken = default)
        {
            Consultor? consultor =
                await _consultorRepository.ObtenerEntidadPorIdAsync(
                    consultorId,
                    cancellationToken);

            if (consultor is null)
            {
                throw new NotFoundException(
                    "Consultor",
                    consultorId);
            }

            string nombreNormalizado = request.Nombre.Trim();

            string emailNormalizado = request.EmailCorporativo
                .Trim()
                .ToLowerInvariant();

            await ValidarAreaActivaAsync(
                request.AreaEspecializacionId,
                cancellationToken);

            await ValidarEmailUnicoAsync(
                emailNormalizado,
                consultorId,
                cancellationToken);

            await ValidarNombreAreaUnicoAsync(
                nombreNormalizado,
                request.AreaEspecializacionId,
                consultorId,
                cancellationToken);

            consultor.Actualizar(
                nombreNormalizado,
                request.AreaEspecializacionId,
                request.TarifaHora,
                emailNormalizado);

            await _consultorRepository.ActualizarAsync(
                consultor,
                cancellationToken);

            _logger.LogInformation(
                "Se actualizó el consultor {ConsultorId}.",
                consultorId);

            return await ObtenerPorIdAsync(
                consultorId,
                cancellationToken);
        }

        public async Task DesactivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            Consultor? consultor =
                await _consultorRepository.ObtenerEntidadPorIdAsync(
                    consultorId,
                    cancellationToken);

            if (consultor is null)
            {
                throw new NotFoundException(
                    "Consultor",
                    consultorId);
            }

            if (!consultor.Activo)
            {
                throw new BusinessException(
                    "El consultor ya se encuentra inactivo.");
            }

            await _consultorRepository.DesactivarAsync(
                consultorId,
                cancellationToken);

            _logger.LogInformation(
                "Se desactivó el consultor {ConsultorId}.",
                consultorId);
        }

        private async Task ValidarAreaActivaAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken)
        {
            bool areaExisteYEstaActiva =
                await _areaRepository.ExisteActivaAsync(
                    areaEspecializacionId,
                    cancellationToken);

            if (!areaExisteYEstaActiva)
            {
                throw new BusinessException(
                    $"El área de especialización con identificador " +
                    $"{areaEspecializacionId} no existe o se encuentra inactiva.");
            }
        }

        private async Task ValidarEmailUnicoAsync(
            string emailCorporativo,
            int? consultorIdExcluir,
            CancellationToken cancellationToken)
        {
            bool emailExiste =
                await _consultorRepository.ExisteEmailAsync(
                    emailCorporativo,
                    consultorIdExcluir,
                    cancellationToken);

            if (emailExiste)
            {
                throw new ConflictException(
                    "Ya existe un consultor con ese correo corporativo.");
            }
        }

        private async Task ValidarNombreAreaUnicoAsync(
            string nombre,
            int areaEspecializacionId,
            int? consultorIdExcluir,
            CancellationToken cancellationToken)
        {
            bool combinacionExiste =
                await _consultorRepository.ExisteNombreAreaAsync(
                    nombre,
                    areaEspecializacionId,
                    consultorIdExcluir,
                    cancellationToken);

            if (combinacionExiste)
            {
                throw new ConflictException(
                    "Ya existe un consultor con el mismo nombre y área de especialización.");
            }
        }

        public async Task<ConsultorDto> ActivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            Consultor consultor =
                await _consultorRepository.ObtenerEntidadPorIdAsync(
                    consultorId,
                    cancellationToken)
                ?? throw new NotFoundException(
                    $"No se encontró el consultor con identificador {consultorId}.");

            if (consultor.Activo)
            {
                throw new BusinessException(
                    "El consultor ya se encuentra activo.");
            }

            bool areaActiva =
                await _areaRepository.ExisteActivaAsync(
                    consultor.AreaEspecializacionId,
                    cancellationToken);

            if (!areaActiva)
            {
                throw new BusinessException(
                    "No se puede reactivar el consultor porque su área " +
                    "de especialización se encuentra inactiva.");
            }

            consultor.Activar();

            await _consultorRepository.ActualizarAsync(
                consultor,
                cancellationToken);

            _logger.LogInformation(
                "Consultor reactivado. ConsultorId: {ConsultorId}, " +
                "AreaEspecializacionId: {AreaEspecializacionId}",
                consultorId,
                consultor.AreaEspecializacionId);

            ConsultorDto? resultado =
                await _consultorRepository.ObtenerPorIdAsync(
                    consultorId,
                    cancellationToken);

            return resultado
                ?? throw new InvalidOperationException(
                    "El consultor fue reactivado, pero no pudo recuperarse.");
        }
    }
}
