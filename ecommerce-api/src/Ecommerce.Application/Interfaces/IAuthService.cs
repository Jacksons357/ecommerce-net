using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    string GenerateToken(string email, string nome);
}
