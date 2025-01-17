using CineApi.Data;
using CineApi.Models;
using CineApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineApi.Controllers
{
    [ApiController]
    public class AssinaturaController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("assinatura")]
        public async Task<IActionResult> CreateAssinatura(
            [FromServices] AppDbContext context,
            [FromBody] Assinatura model)
        {
            try
            {
                var newAssinatura = new Assinatura
                {
                    Nome = model.Nome,
                    Preco = model.Preco,
                    Desconto = model.Desconto
                };
                
                await context.Assinaturas.AddAsync(newAssinatura);
                await context.SaveChangesAsync();

                return Ok(newAssinatura);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpGet("assinatura")]
        public async Task<IActionResult> GetAllSubscriptions(
            [FromServices] AppDbContext context)
        {
            try
            {
                var assinaturas = await context.Assinaturas.ToListAsync();
                return Ok(assinaturas);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [HttpGet("assinatura/{id}")]
        public async Task<IActionResult> GetById(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var assinaturas = await context.Assinaturas.FindAsync(id);
                if (assinaturas == null)
                {
                    return NotFound(new { message = "Assinatura não encontrada." });
                }

                return Ok(assinaturas);
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("assinatura/{id}")]
        public async Task<IActionResult> UpdateSubscriptionAsync(
            [FromServices] AppDbContext context,
            [FromBody] Assinatura model,
            [FromRoute] int id)
        {
            try
            {
                var assinatura = await context.Assinaturas.FindAsync(id);
                if (assinatura == null)
                {
                    return NotFound(new { message = "Assinatura não encontrada." });
                }

                if (!string.IsNullOrEmpty(model.Nome))
                {
                    assinatura.Nome = model.Nome;
                }

                if (model.Preco.ToString() != "")
                {
                    assinatura.Preco = model.Preco;
                }

                if (model.Desconto.ToString() != "")
                {
                    assinatura.Desconto = model.Desconto;
                }

                context.Assinaturas.Update(assinatura);
                await context.SaveChangesAsync();

                return Ok(new { message = "Assinatura atualizada com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("assinatura/{id}")]
        public async Task<IActionResult> DeleteSubscription(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            try
            {
                var assinatura = await context.Assinaturas.FindAsync(id);
                if (assinatura == null)
                {
                    return NotFound(new { message = "Assinatura não encontrada." });
                }

                context.Assinaturas.Remove(assinatura);
                await context.SaveChangesAsync();

                return Ok(new { message = "Assinatura removida com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { message = "Falha interna no servidor." });
            }
        }
    }
}
