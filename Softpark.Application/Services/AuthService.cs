using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;
using Softpark.Domain.Exceptions;

namespace Softpark.Application.Services
{
    public sealed class AuthService
    {
        private const string UsuarioFixo = "admin";
        private const string SenhaFixa = "123";

        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public LoginResponseDto Login(LoginRequestDto request)
        {
            if (request is null)
                throw new DomainException("A requisição de login é obrigatória.");

            if (!string.Equals(request.Usuario, UsuarioFixo, StringComparison.Ordinal) ||
                !string.Equals(request.Senha, SenhaFixa, StringComparison.Ordinal))
            {
                throw new DomainException("Usuário ou senha inválidos.");
            }

            return new LoginResponseDto
            {
                Token = _tokenService.GerarToken(request.Usuario)
            };
        }
    }
}
