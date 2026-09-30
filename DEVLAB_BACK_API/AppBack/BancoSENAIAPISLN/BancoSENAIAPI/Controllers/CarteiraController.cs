using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        // Lista todas as carteiras cadastradas
        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteiras.ToListAsync();

            return Ok(carteiras);
        }

        // Cadastra uma nova carteira
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {
            // Verifica se o número da carteira já está cadastrado
            if (await _context.Carteiras.AnyAsync(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new
                {
                    message = "Este número de carteira já existe."
                });

            // O apetite não pode ser menor que zero
            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });

            // Se o apetite não for informado, usa o valor padrão
            if (novaCarteira.ApetiteCarteira == 0)
                novaCarteira.ApetiteCarteira = 1000000;

            await _context.Carteiras.AddAsync(novaCarteira);
            await _context.SaveChangesAsync();

            return Created("", novaCarteira);
        }

        // Consulta uma carteira pelo número
        [HttpGet("{numero}")]
        public async Task<IActionResult> ConsultarPorNumero(int numero)
        {
            var carteira = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            return Ok(carteira);
        }

        // Atualiza uma carteira existente
        [HttpPut("{numero}")]
        public async Task<IActionResult> Alterar(
            int numero,
            [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteiraExistente == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            // Verifica se o novo apetite é válido
            if (carteiraAtualizada.ApetiteCarteira < 0)
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });

            carteiraExistente.NomeCarteira =
                carteiraAtualizada.NomeCarteira;

            carteiraExistente.ApetiteCarteira =
                carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Exclui uma carteira
        [HttpDelete("{numero}")]
        public async Task<IActionResult> Excluir(int numero)
        {
            var carteira = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            _context.Carteiras.Remove(carteira);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carteira excluída com sucesso."
            });
        }
    }
}