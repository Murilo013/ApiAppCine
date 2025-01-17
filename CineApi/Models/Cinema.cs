namespace CineApi.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public decimal PrecoIngresso { get; set; } 
        public List<FilmeProgramacao> Programacao { get; set; } = new List<FilmeProgramacao>();
    }

    public class FilmeProgramacao
    {
        public int Id { get; set; }
        public string FilmeNome { get; set; } = string.Empty;
        public int Duracao { get; set; }
        public int Horario { get; set; } 
        public string Data { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public int CinemaId { get; set; } // FK

    }
}
