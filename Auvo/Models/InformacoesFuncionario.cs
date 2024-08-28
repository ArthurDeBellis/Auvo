using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Auvo.Models
{
    public class InformacoesFuncionario
    {
        [Name("Código")]
        public string Codigo { get; set; }
        [Name("Nome")]
        public string Nome { get; set; }
        [Name("Valor hora")]
        public string ValorHora { get; set; }
        [Name("Data")]
        public string Data { get; set; }
        [Name("Entrada")]
        public string Entrada { get; set; }
        [Name("Saída")]
        public string Saida { get; set; }
        [Name("Almoço")]
        public string Almoco { get; set; }
    }
}
