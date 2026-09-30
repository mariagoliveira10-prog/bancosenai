using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(
            int codigoCliente,
            IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if (arquivo.Length > 2 * 1024 * 1024)
            {
                return BadRequest("O arquivo não pode ter mais de 2 MB.");
            }

            string extensao = Path.GetExtension(arquivo.FileName);

            if (extensao != ".pdf" &&
                extensao != ".jpg" &&
                extensao != ".png")
            {
                return BadRequest(
                    "Extensão de arquivo não permitida. Use apenas .pdf, .jpg ou .png."
                );
            }

            string pastaCliente = Path.Combine(
                _caminhoRaiz,
                codigoCliente.ToString()
            );

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nameOriginal =
                Path.GetFileNameWithoutExtension(arquivo.FileName);

            string novoNome =
                $"{codigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";

            string caminhoFinal = Path.Combine(
                pastaCliente,
                novoNome
            );

            using (var stream = new FileStream(
                caminhoFinal,
                FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetaDados = new DocumentoMetaDado
            {
                Name = nameOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            await _context.Documentos.AddAsync(documentoMetaDados);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso",
                arquivoSalvo = novoNome
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarDocumentos(int codigoCliente)
        {
            var documentos = await _context.Documentos
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound(
                    "Nenhum documento encontrado para este cliente."
                );
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            byte[] fileBytes =
                await System.IO.File.ReadAllBytesAsync(documento.Caminho);

            string nomeArquivo =
                documento.Name + documento.Extensao;

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo
            );
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();

            return Ok("Documento excluído com sucesso.");
        }
    }
}

