using Dapper;
using Microsoft.Data.SqlClient;
using Softpark.Application.Interfaces;
using Softpark.Domain.Entities;
using Softpark.Infrastructure.Data;

namespace Softpark.Infrastructure.Repositories
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public UsuarioRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CriarAsync(Usuario usuario, CancellationToken cancellationToken)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                const string insertUsuarioSql = """
                INSERT INTO usuario (usuario, status)
                OUTPUT INSERTED.id
                VALUES (@Usuario, @Status);
                """;

                var usuarioId = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        insertUsuarioSql,
                        new
                        {
                            Usuario = usuario.NomeUsuario,
                            Status = usuario.Status
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));

                const string insertPerfilSql = """
                INSERT INTO usuario_perfil (usuario_id, perfil)
                VALUES (@UsuarioId, @Perfil);
                """;

                foreach (var perfil in usuario.Perfis)
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            insertPerfilSql,
                            new
                            {
                                UsuarioId = usuarioId,
                                Perfil = perfil.Perfil
                            },
                            transaction: transaction,
                            cancellationToken: cancellationToken));
                }

                transaction.Commit();
                return usuarioId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateUsuarioSql = """
                UPDATE usuario
                SET usuario = @Usuario,
                    status = @Status
                WHERE id = @Id;
                """;

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        updateUsuarioSql,
                        new
                        {
                            Id = usuario.Id,
                            Usuario = usuario.NomeUsuario,
                            Status = usuario.Status
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));

                const string deletePerfisSql = """
                DELETE FROM usuario_perfil
                WHERE usuario_id = @UsuarioId;
                """;

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        deletePerfisSql,
                        new { UsuarioId = usuario.Id },
                        transaction: transaction,
                        cancellationToken: cancellationToken));

                const string insertPerfilSql = """
                INSERT INTO usuario_perfil (usuario_id, perfil)
                VALUES (@UsuarioId, @Perfil);
                """;

                foreach (var perfil in usuario.Perfis)
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            insertPerfilSql,
                            new
                            {
                                UsuarioId = usuario.Id,
                                Perfil = perfil.Perfil
                            },
                            transaction: transaction,
                            cancellationToken: cancellationToken));
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            const string sqlUsuario = """
            SELECT id, usuario, status
            FROM usuario
            WHERE id = @Id;
            """;

            var usuarioData = await connection.QueryFirstOrDefaultAsync<UsuarioDbModel>(
                new CommandDefinition(
                    sqlUsuario,
                    new { Id = id },
                    cancellationToken: cancellationToken));

            if (usuarioData is null)
                return null;

            const string sqlPerfis = """
            SELECT perfil
            FROM usuario_perfil
            WHERE usuario_id = @Id;
            """;

            var perfis = await connection.QueryAsync<string>(
                new CommandDefinition(
                    sqlPerfis,
                    new { Id = id },
                    cancellationToken: cancellationToken));

            var usuario = new Usuario(usuarioData.Usuario, usuarioData.Status, perfis);
            usuario.DefinirId(usuarioData.Id);

            return usuario;
        }

        public async Task<(IReadOnlyList<Usuario> Usuarios, int TotalRegistros)> ListarPaginadoAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var offset = (page - 1) * pageSize;

            const string countSql = """
            SELECT COUNT(1)
            FROM usuario;
            """;

            var totalRegistros = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    countSql,
                    cancellationToken: cancellationToken));

            const string usuariosSql = """
            SELECT id, usuario, status
            FROM usuario
            ORDER BY id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            var usuariosDb = (await connection.QueryAsync<UsuarioDbModel>(
                new CommandDefinition(
                    usuariosSql,
                    new
                    {
                        Offset = offset,
                        PageSize = pageSize
                    },
                    cancellationToken: cancellationToken))).ToList();

            if (usuariosDb.Count == 0)
                return (new List<Usuario>(), totalRegistros);

            var ids = usuariosDb.Select(u => u.Id).ToArray();

            const string perfisSql = """
            SELECT usuario_id AS UsuarioId, perfil
            FROM usuario_perfil
            WHERE usuario_id IN @Ids;
            """;

            var perfisDb = (await connection.QueryAsync<UsuarioPerfilDbModel>(
                new CommandDefinition(
                    perfisSql,
                    new { Ids = ids },
                    cancellationToken: cancellationToken))).ToList();

            var usuarios = new List<Usuario>();

            foreach (var item in usuariosDb)
            {
                var perfisDoUsuario = perfisDb
                    .Where(p => p.UsuarioId == item.Id)
                    .Select(p => p.Perfil)
                    .ToList();

                var usuario = new Usuario(item.Usuario, item.Status, perfisDoUsuario);
                usuario.DefinirId(item.Id);

                usuarios.Add(usuario);
            }

            return (usuarios, totalRegistros);
        }

        private sealed class UsuarioDbModel
        {
            public int Id { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public bool Status { get; set; }
        }

        private sealed class UsuarioPerfilDbModel
        {
            public int UsuarioId { get; set; }
            public string Perfil { get; set; } = string.Empty;
        }
    }
}
