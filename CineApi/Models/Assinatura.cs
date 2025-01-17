namespace CineApi.Models
{
    public class Assinatura
    {

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty; // Iniciante, Cinefilo, Fanático, Diretor
        public decimal Preco { get; set; }
        public decimal Desconto { get; set; }

        
    }
}
