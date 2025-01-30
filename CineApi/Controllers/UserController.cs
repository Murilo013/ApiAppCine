using CineApi.Data;
using CineApi.Models;
using CineApi.Services;
using CineApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.IO;

namespace CineApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("user")]
        public async Task<IActionResult> SignupAsync(
        [FromServices] AppDbContext context,
        [FromBody] UserSignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExistente = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (usuarioExistente != null)
            {
                return StatusCode(401, new { message = "Email já cadastrado" });
            }

            try
            {
                var assinatura = await context.Assinaturas.FirstOrDefaultAsync(x => x.Id == 1);

                if (assinatura == null)
                {
                    return StatusCode(400, new { message = "Assinatura não encontrada." });
                }

                var newUser = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Senha = Setting.GenerateHash(model.Senha),
                    Imagem = model.Imagem,
                    Assinatura = assinatura, 
                    Role = "Usuario"
                };
                
                await context.Usuarios.AddAsync(newUser);
                await context.SaveChangesAsync();
                return Ok(new { userId = newUser.Id, message = "Usuário criado com sucesso" });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }


        [HttpPost("user/login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginViewModel model,
            [FromServices] AppDbContext context,
            [FromServices] TokenService tokenService)
        {
            var usuarioExistente = await context.Usuarios.FirstOrDefaultAsync(x => x.UserName == model.UserName);

            if (usuarioExistente == null)
            {
                return StatusCode(401, new { message = "Usuário ou senha inválidos" });
            }

            if (Setting.GenerateHash(model.Senha) != usuarioExistente.Senha)
            {
                return StatusCode(401, new { message = "Usuário ou senha inválidos" });
            }

            try
            {
                var token = tokenService.CreateToken(usuarioExistente);
                var id = usuarioExistente.Id;
                return Ok(new { id, token = token });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user")]
        public async Task<IActionResult> GetAllUsers(
            [FromServices] AppDbContext context)
        {
            try
            {

                var usuarios = await context.Usuarios.Include(x => x.Assinatura).ToListAsync();

                var usuariosViewModel = new List<UserReturnViewModel>();
                foreach (var dado in usuarios) 
                {
                    var newUser = new UserReturnViewModel
                    {
                        Id = dado.Id,
                        UserName = dado.UserName,
                        Email = dado.Email,
                        Imagem = dado.Imagem,
                        AssinaturaNome = dado.Assinatura.Nome,
                        Role = dado.Role,
                    };
                    usuariosViewModel.Add(newUser);
                }


                return Ok(usuariosViewModel);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetById(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var usuarioExistente = await context.Usuarios
                    .Include(u => u.Assinatura)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (usuarioExistente == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var assinatura = usuarioExistente.Assinatura;

                var response = new
                {
                    Id = usuarioExistente.Id,
                    Username = usuarioExistente.UserName,
                    Email = usuarioExistente.Email,
                    Imagem = usuarioExistente.Imagem,
                    AssinaturaNome = assinatura?.Nome,
                    AssinaturaDesconto = assinatura?.Desconto,
                    RoleUsuario = usuarioExistente.Role
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Falha interna no servidor", details = ex.Message });
            }
        }


        [Authorize(Roles = "Usuario,Admin")]
        [HttpPatch("user/{id}")]
        public async Task<IActionResult> UpdateUserAsync(
        [FromServices] AppDbContext context,
        [FromBody] UserUpdateViewModel model,
        [FromRoute] int id)
        {
            try
            {
                var usuarioExistente = await context.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }

                if (!string.IsNullOrEmpty(model.Email) && model.Email != usuarioExistente.Email)
                {
                    var emailJaExiste = await context.Usuarios.AnyAsync(x => x.Email == model.Email);
                    if (emailJaExiste)
                    {
                        return BadRequest(new { message = "O email informado já está em uso." });
                    }
                    usuarioExistente.Email = model.Email;
                }


                if (!string.IsNullOrEmpty(model.UserName))
                {
                    usuarioExistente.UserName = model.UserName;
                }


                if (!string.IsNullOrEmpty(model.NovaSenha) && !string.IsNullOrEmpty(model.SenhaAtual))
                {
                    if (Setting.GenerateHash(model.SenhaAtual) == usuarioExistente.Senha)
                    {
                        usuarioExistente.Senha = Setting.GenerateHash(model.NovaSenha);
                    }
                    else
                    {
                        return BadRequest(new { message = "A senha atual está incorreta." });
                    }
                }

                if (!string.IsNullOrEmpty(model.Imagem))
                {
                    usuarioExistente.Imagem = model.Imagem;
                }

                context.Usuarios.Update(usuarioExistente);
                await context.SaveChangesAsync();

                return Ok(new { message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Falha interna no servidor.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Usuario,Admin")]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var usuarioExistente = await context.Usuarios.FindAsync(id);

                if (usuarioExistente == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }

                context.Usuarios.Remove(usuarioExistente);
                await context.SaveChangesAsync();

                return Ok(new { message = "Usuário Removido com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }          
        }

        [HttpPatch("userAssinatura/{idUsuario}")]
        public async Task<IActionResult> UpdateAssinatura(
        [FromServices] AppDbContext context,
        [FromBody] AssinaturaUserUpdateViewModel model,
        [FromRoute] int idUsuario) 
        {

            var assinatura = await context.Assinaturas.FindAsync(model.AssinaturaId); 

            if (assinatura == null)
            {
                return NotFound(new { message = "Assinatura não encontrada" });
            }

            var usuarioExistente = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == idUsuario);

            if (usuarioExistente == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            try
            {
                usuarioExistente.Assinatura = assinatura;
                await context.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Falha interna do servidor", error = ex.Message });
            }

            return Ok("Assinatura atualizada");
        }


    }
}
