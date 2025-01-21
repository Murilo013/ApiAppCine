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
                    FilmeId = model.FilmeId,
                    CinemaId = model.CinemaId,
                    Assentos = model.Assentos,
                    Total = model.Total
                };

                await context.Ingressos.AddAsync(newIngresso);
                await context.SaveChangesAsync();

                return Ok("Sucesso!");
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

                var filmeExistente = await context.Programacoes.FindAsync(model.FilmeId);
                if (filmeExistente == null)
                {
                    return BadRequest(new { message = "Filme não existe na programação." });
                }

                var cinemaTemFilme = await context.Programacoes.AnyAsync(p => p.Id == model.FilmeId && p.CinemaId == model.CinemaId);

                if (!cinemaTemFilme)
                {
                    return BadRequest(new { message = "Filme não pertence ao cinema informado." });
                }

                if (!string.IsNullOrEmpty(model.Assentos))
                {
                    ingressoExistente.Assentos = model.Assentos;
                }

                if (model.Total != 0)
                {
                    ingressoExistente.Total = model.Total;
                }

                if (model.FilmeId != 0)
                {
                    ingressoExistente.FilmeId = model.FilmeId;
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
