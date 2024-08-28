using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auvo.Models
{
    public class ArquivoCSV
    {
        public string NomeArquivo { get; set; }
        public List<InformacoesFuncionario>? Informacoes { get; set; }
    }
}
