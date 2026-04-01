using Softpark.Domain.Exceptions;

namespace Softpark.Domain.Entities
{
    public sealed class Usuario
    {
        private readonly List<UsuarioPerfil> _perfis = new();

        public int Id { get; private set; }
        public string NomeUsuario { get; private set; }
        public bool Status { get; private set; }
        public IReadOnlyCollection<UsuarioPerfil> Perfis => _perfis.AsReadOnly();

        public Usuario(string nomeUsuario, bool status, IEnumerable<string> perfis)
        {
            ValidarNomeUsuario(nomeUsuario);

            NomeUsuario = nomeUsuario.Trim();
            Status = status;

            DefinirPerfis(perfis);
        }

        public void DefinirId(int id)
        {
            if (id <= 0)
                throw new DomainException("O identificador do usuário é inválido.");

            Id = id;
        }

        public void Atualizar(string nomeUsuario, bool status, IEnumerable<string> perfis)
        {
            ValidarNomeUsuario(nomeUsuario);

            NomeUsuario = nomeUsuario.Trim();
            Status = status;

            DefinirPerfis(perfis);
        }

        private void DefinirPerfis(IEnumerable<string> perfis)
        {
            if (perfis is null)
                throw new DomainException("É obrigatório informar ao menos um perfil.");

            var perfisTratados = perfis
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (perfisTratados.Count == 0)
                throw new DomainException("É obrigatório informar ao menos um perfil.");

            _perfis.Clear();

            foreach (var perfil in perfisTratados)
            {
                _perfis.Add(new UsuarioPerfil(perfil));
            }
        }

        private static void ValidarNomeUsuario(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
                throw new DomainException("O campo usuário é obrigatório.");
        }
    }
}
