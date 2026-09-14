using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
            );

        private static List<Models.DocumentoMetaDado> _documentosMetaDados = new List<Models.DocumentoMetaDado>();

        private static int _nexId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }
            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());
            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }
            string extensao = Path.GetExtension(arquivo.FileName);

            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(_caminhoRaiz, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            var documentoMetaDados = new Models.DocumentoMetaDado
            {
                Id = _nexId++,
                Name = nameOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };
            _documentosMetaDados.Add(documentoMetaDados);
            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome});

        }
        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarDocumentos(int codigoCliente)
        {
            var documentos = _documentosMetaDados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound("Nenhum documento encontrado para este cliente.");
            }

            return Ok(documentos);
        }
        [HttpGet("download/{id}")]
        public IActionResult Download(int id)
        {
            var documento = _documentosMetaDados
                .FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);

            string nomeArquivo = documento.Name + documento.Extensao;

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo
            );
        }
        [HttpDelete("excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            var documento = _documentosMetaDados
                .FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            System.IO.File.Delete(documento.Caminho);

            _documentosMetaDados.Remove(documento);

            return Ok("Documento excluído com sucesso.");
        }
    }
}


