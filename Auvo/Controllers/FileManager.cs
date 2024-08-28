using Auvo.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Globalization;
using System.IO;
using Auvo.Models;
using System.Collections.Generic;

namespace Auvo.Controllers
{
    internal class FileManager : IFileManager
    {
        private readonly ILogger _logger;

        public FileManager(ILogger logger) 
        {
            _logger = logger;
        }

        public async Task<string>? LerInput()
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

        public async Task<string[]> LerPasta(string? caminhoDaPasta)
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
        public async Task<List<ArquivoCSV>> LerArquivos(string[] arquivos)
        {
            List<ArquivoCSV> arquivosCsv = new List<ArquivoCSV>();

            try
            {
                if (arquivos == null || arquivos.Length == 0)
                {
                    throw new ArgumentNullException(nameof(arquivos));
                }

                foreach(string arquivo in arquivos)
                {

                    ArquivoCSV arquivoCSV = new ArquivoCSV();

                    arquivoCSV.NomeArquivo = Path.GetFileName(arquivo);
                    arquivoCSV.Informacoes = new List<InformacoesFuncionario>();

                    
                    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                    };

                    // Leitura de um arquivo CSV
                    using (StreamReader reader = new StreamReader(arquivo))
                    using (CsvReader csv = new CsvReader(reader, config))
                    {
                        arquivoCSV.Informacoes = csv.GetRecords<InformacoesFuncionario>().ToList();
                    }

                    arquivosCsv.Add(arquivoCSV);
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"A pasta não possui arquivos: {ex.ToString()}");
            }

            return arquivosCsv;
        }
    }
}
