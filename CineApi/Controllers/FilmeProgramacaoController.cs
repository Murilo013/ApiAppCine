using CineApi.Data;
using CineApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineApi.Controllers
{
    [ApiController]
    public class FilmeProgramacaoController : ControllerBase
    {
        [HttpGet("filmesCinema/{id}")]
        public async Task<IActionResult> GetProgramacaoById(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var programacao = await context.Cinemas.Where(x => x.Id == id).Select(x => x.Programacao).FirstOrDefaultAsync();
                return Ok(programacao);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adicionarFilme/{id}")]
        public async Task<IActionResult> PostFilmesAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id,
            [FromBody] FilmeProgramacao model)
        {
            try
            {
                var cinema = await context.Cinemas.FindAsync(id);

                if (cinema == null)
                {
                    return NotFound();
                }

                var newProgramacao = new FilmeProgramacao
                {
                    FilmeNome = model.FilmeNome,
                    Duracao = model.Duracao,
                    Horario = model.Horario,
                    Data = model.Data,
                    Sala = model.Sala,
                };

                cinema.Programacao.Add(newProgramacao);
                await context.SaveChangesAsync();
                return Ok(newProgramacao);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("atualizarFilme/{id}")]
        public async Task<IActionResult> PatchFilmesAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id,
            [FromBody] FilmeProgramacao model)
        {
            try
            {
                var filmeExistente = await context.Programacoes.FindAsync(id);

                if (filmeExistente == null)
                {
                    return NotFound(new { message = "Filme não encontrado." });
                }

                if (!string.IsNullOrEmpty(model.FilmeNome))
                {
                    filmeExistente.FilmeNome = model.FilmeNome;
                }

                if (!string.IsNullOrEmpty(model.Sala))
                {
                    filmeExistente.Sala = model.Sala;
                }

                if (model.Duracao != 0)
                {
                    filmeExistente.Duracao = model.Duracao;
                }

                if (model.Horario != null && model.Horario != 0)
                {
                    filmeExistente.Horario = model.Horario;
                }

                if (!string.IsNullOrEmpty(model.Data))
                {
                    filmeExistente.Data = model.Data;
                }

                context.Programacoes.Update(filmeExistente);
                await context.SaveChangesAsync();

                return Ok(new { message = "Filme atualizado com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("removerFilme/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var filme = await context.Programacoes.FindAsync(id);
                context.Programacoes.Remove(filme);
                await context.SaveChangesAsync();
                return Ok("filme removido");
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }

        }
    }
}
