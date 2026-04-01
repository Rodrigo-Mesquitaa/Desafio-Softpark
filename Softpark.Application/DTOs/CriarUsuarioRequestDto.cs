namespace Softpark.Application.DTOs
{
    public sealed class CriarUsuarioRequestDto
    {
        public string Usuario { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<string> Perfis { get; set; } = new();
    }
}
