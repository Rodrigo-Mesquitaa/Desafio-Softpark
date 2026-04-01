using Softpark.Domain.Exceptions;

namespace Softpark.Domain.Entities
{
    public sealed class UsuarioPerfil
    {
        public string Perfil { get; private set; }

        public UsuarioPerfil(string perfil)
        {
            if (string.IsNullOrWhiteSpace(perfil))
                throw new DomainException("O perfil é obrigatório.");

            Perfil = perfil.Trim();
        }
    }
}
