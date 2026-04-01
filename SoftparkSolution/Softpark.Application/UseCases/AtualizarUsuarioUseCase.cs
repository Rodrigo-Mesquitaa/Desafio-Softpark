using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;
using Softpark.Domain.Exceptions;

namespace Softpark.Application.UseCases
{
    public sealed class AtualizarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AtualizarUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task ExecuteAsync(
            int id,
            AtualizarUsuarioRequestDto request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id, cancellationToken);

            if (usuario is null)
                throw new DomainException("Usuário não encontrado.");

            usuario.Atualizar(request.Usuario, request.Status, request.Perfis);

            await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);
        }
    }
}
