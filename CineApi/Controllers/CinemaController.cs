using CineApi.Data;
using CineApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

namespace CineApi.Controllers
{
    [ApiController]
    public class CinemaController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("cinema")]
        public async Task<IActionResult> CreateCinemaAsync(
        [FromServices] AppDbContext context,
        [FromBody] Cinema model)
        {
            var cinemaExistente = await context.Cinemas.FirstOrDefaultAsync(x => x.CNPJ == model.CNPJ);

            if (cinemaExistente != null)
            {
                return StatusCode(409, new { message = "CNPJ já cadastrado" });
            }

            try
            {
                var newCinema = new Cinema
                {
                    CNPJ = model.CNPJ,
                    Nome = model.Nome,
                    PrecoIngresso = model.PrecoIngresso,
                    CEP = model.CEP,
                    Rua = model.Rua,
                    Numero = model.Numero,
                    Cidade = model.Cidade
                };


                await context.Cinemas.AddAsync(newCinema);
                await context.SaveChangesAsync(); 

                return Ok(new { cinemaId = newCinema.Id });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("cinema/{id}")]
        public async Task<IActionResult> UpdateCinemaAsnc(
            [FromServices] AppDbContext context,
            [FromRoute] int id,
            [FromBody] Cinema model)
        {
            try
            {
                var cinemaExistente = await context.Cinemas.FindAsync(id);
                var CinemaCNPJ = await context.Cinemas.FirstOrDefaultAsync(x => x.CNPJ == model.CNPJ);

                if (cinemaExistente == null)
                {
                    return NotFound(new { message = "Cinema não encontrado." });
                }

                if (!string.IsNullOrEmpty(model.CNPJ))
                {
                    cinemaExistente.CNPJ = model.CNPJ;
                }

                if (!string.IsNullOrEmpty(model.CEP))
                {
                    cinemaExistente.CEP = model.CEP;
                }

                if (model.PrecoIngresso != 0)
                {
                    cinemaExistente.PrecoIngresso = model.PrecoIngresso;
                }

                if (!string.IsNullOrEmpty(model.Nome))
                {
                    if (model.Nome != cinemaExistente.Nome && CinemaCNPJ == null)
                    {
                        cinemaExistente.Nome = model.Nome;
                    }
                }

                context.Cinemas.Update(cinemaExistente);
                await context.SaveChangesAsync();

                return Ok(new { message = "Cinema atualizado com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("cinema/{id}")]
        public async Task<IActionResult> DeleteCinema(
        [FromServices] AppDbContext context,
        [FromRoute] int id)
        {
            try
            {
                var cinemaExistente = await context.Cinemas
                    .Include(c => c.Programacao) 
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cinemaExistente == null)
                {
                    return NotFound(new { message = "Cinema não encontrado." });
                }

                context.Programacoes.RemoveRange(cinemaExistente.Programacao);
                context.Cinemas.Remove(cinemaExistente);

                await context.SaveChangesAsync();

                return Ok(new { message = "Cinema removido com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Falha interna no servidor.", error = ex.Message });
            }
        }



        [HttpGet("cinema")]
        public async Task<IActionResult> GetAllCinemaAsync(
            [FromServices] AppDbContext context)
        {
            try
            {
                var cinemas = await context.Cinemas.Include(c => c.Programacao).ToListAsync();
                return Ok(cinemas);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpGet("cinema/{id}")]
        public async Task<IActionResult> GetByIdCinemaAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var cinemaExistente = await context.Cinemas.Include(x => x.Programacao).FirstOrDefaultAsync(x => x.Id == id);
                return Ok(cinemaExistente);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

    }
}
