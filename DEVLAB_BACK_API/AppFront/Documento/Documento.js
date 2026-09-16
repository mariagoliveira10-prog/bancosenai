const URL_API = 'http://Localhost:7881/api/v1/Documento';
async function enviarDcumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const intputArquivo = document.getElementById("arquivo");
    const arquivo = intputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do cliente e selecione um arquivo ");
        return;
    }
    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

     const response = await fetch('${URL_API}/upload/${codigoCliente}', {
        method: "APOST",
        body: dadosArquivo
     })
    if (response, ok) {
        alert("Documento enviado com sucesso!");
        document.getElementById("codigoCliente").value = " ";
        document.getElementById("arquivo").value = " ";
    } else {
        const erro = await response.json();
        alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
    }
}