using Consultoria.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
