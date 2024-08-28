using Auvo.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Globalization;
using System.IO;
using Auvo.Models;

namespace Auvo.Controllers
{
    internal class FileManager : IFileManager
    {
        private readonly ILogger _logger;

        public FileManager(ILogger logger) 
        {
            _logger = logger;
        }

        public string? LerInput()
        {
            string? caminhoDaPasta = "";
            try
            {
                Console.Write("Nome da pasta que deseja ler: ");
                caminhoDaPasta = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(caminhoDaPasta))
                {
                    throw new ArgumentException("A entrada não pode ser vazia.", nameof(caminhoDaPasta));
                }
            }catch(ArgumentException ex)
            {
                _logger.LogError($"Ocorreu um erro: {ex.ToString()}");
            }

            return caminhoDaPasta;
        }

        public string[] LerPasta(string? caminhoDaPasta)
        {
            string[] arquivos = [];
            try
            {
                if (!Directory.Exists(caminhoDaPasta))
                {
                    throw new DirectoryNotFoundException("Diretório não encontrado");
                }
                arquivos = Directory.GetFiles(caminhoDaPasta);
            }
            catch (DirectoryNotFoundException ex) 
            {
                _logger.LogError($"Ocorreu um erro: {ex.ToString()}");
            }

            return arquivos;
        }
        public void LerArquivos(string[] arquivos)
        {
            try
            {
                if (arquivos == null || arquivos.Length == 0)
                {
                    throw new ArgumentNullException(nameof(arquivos));
                }

                foreach(string arquivo in arquivos)
                {
                    // Configuração personalizada para o delimitador
                    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                    };

                    // Leitura de um arquivo CSV
                    using (StreamReader reader = new StreamReader(arquivo))
                    using (CsvReader csv = new CsvReader(reader, config))
                    {
                        // Ler registros e mapear para a classe Record
                        IEnumerable<InformacoesFuncionario> records = csv.GetRecords<InformacoesFuncionario>();

                        // Iterar sobre os registros e exibir os dados
                        foreach (var record in records)
                        {
                            Console.WriteLine(record.Nome);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"A pasta não possui arquivos: {ex.ToString()}");
            }        
        }
    }
}
