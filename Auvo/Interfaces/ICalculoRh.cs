using Auvo.Models;

namespace Auvo.Interfaces
{
    public interface ICalculoRh
    {
        Task<List<InformacoesDepartamento>> CalcularInformacoes(List<ArquivoCSV> arquivos);
    }
}
