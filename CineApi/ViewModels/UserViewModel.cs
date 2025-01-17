using System.ComponentModel.DataAnnotations;

namespace CineApi.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;

        public string Imagem { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }


    public class UserSignupViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;

        public string Imagem { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }

    public class UserLoginViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Senha { get; set; } = string.Empty;
    }

    public class UserUpdateViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Imagem { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
        public string SenhaAtual { get; set; } = string.Empty;
    }

    public class UserReturnViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Imagem { get; set; } = string.Empty;

        public string AssinaturaNome {  get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
