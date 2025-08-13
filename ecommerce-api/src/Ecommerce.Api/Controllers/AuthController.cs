using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realizar login no sistema
    /// </summary>
    /// <param name="loginDto">Dados de login (email e senha)</param>
    /// <returns>Token JWT e dados do usuário</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            
            if (result == null)
            {
                return Problem(
                    title: "Credenciais inválidas",
                    detail: "Email ou senha incorretos",
                    statusCode: 401
                );
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro interno",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}
