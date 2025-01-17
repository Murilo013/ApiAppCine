namespace CineApi.Models
{
    public class Ingresso
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        public int CinemaId { get; set; }
        public int FilmeId { get; set; }
        public string Assentos { get; set; }
        public decimal Total {  get; set; }
    }
}
