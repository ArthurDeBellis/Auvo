using Auvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auvo.Interfaces
{
    public interface IFileManager
    {
        Task<string>? LerInput();
        Task<string[]> LerPasta(string? caminhoDaPasta);
        Task<List<ArquivoCSV>> LerArquivos(string[] arquivos);
    }
}
