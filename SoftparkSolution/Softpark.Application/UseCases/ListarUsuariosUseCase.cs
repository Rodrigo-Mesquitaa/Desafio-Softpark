using Softpark.Application.Common;
using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;

namespace Softpark.Application.UseCases
{
    public sealed class ListarUsuariosUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ListarUsuariosUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PagedResultDto<UsuarioResponseDto>> ExecuteAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            page = PaginationHelper.NormalizePage(page);
            pageSize = PaginationHelper.NormalizePageSize(pageSize);

            var (usuarios, totalRegistros) = await _usuarioRepository.ListarPaginadoAsync(
                page,
                pageSize,
                cancellationToken);

            return new PagedResultDto<UsuarioResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRegistros,
                TotalPages = PaginationHelper.CalculateTotalPages(totalRegistros, pageSize),
                Data = usuarios.Select(usuario => new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Usuario = usuario.NomeUsuario,
                    Status = usuario.Status,
                    Perfis = usuario.Perfis.Select(p => p.Perfil).ToList()
                }).ToList()
            };
        }
    }
}
