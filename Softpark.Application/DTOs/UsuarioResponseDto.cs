namespace Softpark.Application.DTOs
{
    public sealed class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<string> Perfis { get; set; } = new();
    }
}
