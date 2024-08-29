using Auvo.Models;

namespace Auvo.Interfaces
{
    public interface IFileManager
    {
        Task<string>? LerInput();
        Task<string[]> LerPasta(string? caminhoDaPasta);
        Task<List<ArquivoCSV>> LerArquivos(string[] arquivos);
        Task<string> EscreveJson(List<InformacoesDepartamento> calculoRh, string diretorio);
    }
}
