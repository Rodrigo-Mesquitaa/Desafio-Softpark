using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Softpark.Application.DTOs;
using Softpark.Application.UseCases;

namespace Softpark.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/usuarios")]
    public sealed class UsuariosController : ControllerBase
    {
        private readonly CriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly AtualizarUsuarioUseCase _atualizarUsuarioUseCase;
        private readonly ObterUsuarioPorIdUseCase _obterUsuarioPorIdUseCase;
        private readonly ListarUsuariosUseCase _listarUsuariosUseCase;

        public UsuariosController(
            CriarUsuarioUseCase criarUsuarioUseCase,
            AtualizarUsuarioUseCase atualizarUsuarioUseCase,
            ObterUsuarioPorIdUseCase obterUsuarioPorIdUseCase,
            ListarUsuariosUseCase listarUsuariosUseCase)
        {
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _atualizarUsuarioUseCase = atualizarUsuarioUseCase;
            _obterUsuarioPorIdUseCase = obterUsuarioPorIdUseCase;
            _listarUsuariosUseCase = listarUsuariosUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<UsuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _listarUsuariosUseCase.ExecuteAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterPorId(
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _obterUsuarioPorIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Criar(
            [FromBody] CriarUsuarioRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var id = await _criarUsuarioUseCase.ExecuteAsync(request, cancellationToken);

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Atualizar(
            [FromRoute] int id,
            [FromBody] AtualizarUsuarioRequestDto request,
            CancellationToken cancellationToken = default)
        {
            await _atualizarUsuarioUseCase.ExecuteAsync(id, request, cancellationToken);
            return NoContent();
        }
    }
}
