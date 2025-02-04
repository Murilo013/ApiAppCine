using CineApi.Data;
using CineApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineApi.Controllers
{
    [ApiController]
    public class IngressoController : Controller
    {
        [HttpPost("ingresso")]
        public async Task<IActionResult> PostIngressoAsync(
           [FromServices] AppDbContext context,
           [FromBody] Ingresso model)
        {
            try
            {
                var AssentoEscolhido = await context.Ingressos.FirstOrDefaultAsync(x => x.Assentos == model.Assentos);
                if (AssentoEscolhido != null)
                {
                    return StatusCode(401, "erro ao criar ingresso, assento já pertence a outro usuário");
                }

                var newIngresso = new Ingresso
                {
                    UsuarioId = model.UsuarioId,
                    FilmeNome = model.FilmeNome,
                    CinemaNome = model.CinemaNome,
                    FilmeData = model.FilmeData,
                    Sala = model.Sala,
                    Assentos = model.Assentos,
                    Total = model.Total
                };

                await context.Ingressos.AddAsync(newIngresso);
                await context.SaveChangesAsync();

                return Ok(newIngresso);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }

        [HttpGet("ingresso")]
        public async Task<IActionResult> GetAllAsync(
            [FromServices] AppDbContext context)
        {
            try
            {
                var ingressos = await context.Ingressos.ToListAsync();
                return Ok(ingressos);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpGet("ingresso/{id}")]

        public async Task<IActionResult> GetAllAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var ingressosUsuario = await context.Ingressos.Where(i => i.UsuarioId == id).ToListAsync();

                return Ok(ingressosUsuario);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpDelete("ingresso/{id}")]

        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var ingresso = context.Ingressos.FindAsync(id);
                context.Remove(ingresso);
                await context.SaveChangesAsync();
                return Ok("Ingresso removido");
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("ingresso/{id}")]
        public async Task<IActionResult> UpdateAsync(
        [FromServices] AppDbContext context,
        [FromRoute] int id,
        [FromBody] Ingresso model)
        {
            try
            {
                var ingressoExistente = await context.Ingressos.FindAsync(id);
                if (ingressoExistente == null)
                {
                    return NotFound(new { message = "Ingresso não encontrado." });
                }

                var cinemaTemFilme = await context.Programacoes
                    .AnyAsync(p => p.FilmeNome == model.FilmeNome);

                if (!cinemaTemFilme)
                {
                    return BadRequest(new { message = "Filme não pertence ao cinema informado ou não existe na programação." });
                }

                if (!string.IsNullOrEmpty(model.Assentos))
                {
                    ingressoExistente.Assentos = model.Assentos;
                }

                if (model.Total != 0)
                {
                    ingressoExistente.Total = model.Total;
                }

                // Atualiza o nome do filme somente se o filme já existir na programação
                if (model.FilmeNome != null && cinemaTemFilme)
                {
                    ingressoExistente.FilmeNome = model.FilmeNome;
                }

                await context.SaveChangesAsync();

                return Ok(new { message = "Ingresso atualizado com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }


    }
}
