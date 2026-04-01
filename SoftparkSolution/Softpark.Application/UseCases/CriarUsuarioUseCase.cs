using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;
using Softpark.Domain.Entities;

namespace Softpark.Application.UseCases
{
    public sealed class CriarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public CriarUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<int> ExecuteAsync(
            CriarUsuarioRequestDto request,
            CancellationToken cancellationToken)
        {
            var usuario = new Usuario(request.Usuario, request.Status, request.Perfis);

            return await _usuarioRepository.CriarAsync(usuario, cancellationToken);
        }
    }
}
