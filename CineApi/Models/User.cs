namespace CineApi.Models
{

    public class User
    {
        public int Id { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public Assinatura Assinatura { get; set; }
        public string Role {  get; set; } = string.Empty;

        public string Imagem { get; set; } = string.Empty;
    }
}
