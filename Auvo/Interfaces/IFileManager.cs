using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auvo.Interfaces
{
    public interface IFileManager
    {
        string? LerInput();
        string[] LerPasta(string? caminhoDaPasta);
        void LerArquivos(string[] arquivos);
    }
}
