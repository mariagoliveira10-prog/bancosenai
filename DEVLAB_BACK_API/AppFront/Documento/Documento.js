const URL_API = 'https://localhost:7081/api/v1/Documento';
async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const intputArquivo = document.getElementById("arquivo");
    const arquivo = intputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do cliente e selecione um arquivo ");
        return;
    }
    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST",
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!");
        document.getElementById("codigoCliente").value = " ";
        document.getElementById("arquivo").value = " ";
    } else {
        alert("Falha ao enviar o arquivo");
    }
}
