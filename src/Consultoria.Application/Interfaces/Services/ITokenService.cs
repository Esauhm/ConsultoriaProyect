using Consultoria.Application.DTOs.Auth;
using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface ITokenService
    {
        LoginResponseDto GenerarToken(Usuario usuario);
    }
}
