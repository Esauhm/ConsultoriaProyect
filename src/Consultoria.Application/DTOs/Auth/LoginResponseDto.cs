using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.DTOs.Auth
{
    public sealed class LoginResponseDto
    {
        public string Token { get; init; } = string.Empty;

        public DateTime ExpiresAtUtc { get; init; }

        public string UserName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Role { get; init; } = string.Empty;
    }
}
