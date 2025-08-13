using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;

    public AuthService(IUnitOfWork unitOfWork, string jwtSecret, string jwtIssuer)
    {
        _unitOfWork = unitOfWork;
        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var usuario = await _unitOfWork.Usuarios.FindFirstAsync(u => u.Email == loginDto.Email);
        
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash))
            return null;

        var token = GenerateToken(usuario.Email, usuario.Nome);
        
        return new LoginResponseDto
        {
            Token = token,
            Nome = usuario.Nome
        };
    }

    public string GenerateToken(string email, string nome)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, nome)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _jwtIssuer,
            Audience = _jwtIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
