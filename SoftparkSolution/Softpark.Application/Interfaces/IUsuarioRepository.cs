using Softpark.Domain.Entities;

namespace Softpark.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<int> CriarAsync(Usuario usuario, CancellationToken cancellationToken);
        Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken);
        Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken);
        Task<(IReadOnlyList<Usuario> Usuarios, int TotalRegistros)> ListarPaginadoAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
