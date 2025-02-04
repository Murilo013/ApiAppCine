namespace CineApi.Models
{
    public class Ingresso
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        public string CinemaNome { get; set; }
        public string FilmeNome { get; set; }
        public string FilmeData { get; set; }
        public string Sala {  get; set; } 
        public string Assentos { get; set; }
        public decimal Total {  get; set; }
    }
}
