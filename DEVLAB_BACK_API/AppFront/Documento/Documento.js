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

        document.getElementById("codigoBusca").value = codigoCliente;

        await buscarDocumentos();

        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else {
        alert("Falha ao enviar o arquivo");
    }
}


async function buscarDocumentos() {
    const codigoCliente = document.getElementById("codigoBusca").value;

    if (!codigoCliente) {
        alert("Informe o codigo do cliente");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`, {
        method: "GET"
    });

    if (response.ok) {
        const documentos = await response.json();

        const tabela = document.getElementById("tabelaDocumentos");

        tabela.innerHTML = "";

        documentos.forEach(documento => {
            const linha = document.createElement("tr");

            linha.innerHTML =
                `<td>${documento.id}</td>
                <td>${documento.name}</td>
                <td>${documento.extensao}</td>
                <td>
                    <button onclick="baixarDocumento(${documento.id})">Baixar</button>
                    <button onclick="excluirDocumento(${documento.id})">Excluir</button>
                </td>`;

            tabela.appendChild(linha);
        });
    } else {
        alert("Nenhum documento encontrado");
    }
}


async function baixarDocumento(id) {
    const response = await fetch(`${URL_API}/download/${id}`);

    if (response.ok) {
        const arquivo = await response.blob();

        const url = window.URL.createObjectURL(arquivo);

        const link = document.createElement("a");
        link.href = url;
        link.download = "documento";

        document.body.appendChild(link);
        link.click();
        link.remove();

        window.URL.revokeObjectURL(url);
    } else {
        alert("Falha ao baixar o arquivo");
    }
}
async function excluirDocumento(id) {
    const confirmar = confirm("Deseja realmente excluir este documento?");

    if (!confirmar) {
        return;
    }
    const response = await fetch(`${URL_API}/excluir/${id}`, {
        method: "DELETE"
    });
    if (response.ok) {
        alert("Documento excluido com sucesso!");

        buscarDocumentos();
    } else {
        alert("Falha ao excluir o documento");
    }
}