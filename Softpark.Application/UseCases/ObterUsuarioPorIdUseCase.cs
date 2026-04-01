using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;
using Softpark.Domain.Exceptions;

namespace Softpark.Application.UseCases
{
    public sealed class ObterUsuarioPorIdUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObterUsuarioPorIdUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioResponseDto> ExecuteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id, cancellationToken);

            if (usuario is null)
                throw new DomainException("Usuário não encontrado.");

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Usuario = usuario.NomeUsuario,
                Status = usuario.Status,
                Perfis = usuario.Perfis.Select(p => p.Perfil).ToList()
            };
        }
    }
}
