using System;
using Auvo.Controllers;
using Auvo.Interfaces;
using Microsoft.Extensions.Logging;

namespace Auvo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Configurando o LoggerFactory
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            
            // Criando um logger
            ILogger logger = factory.CreateLogger("Program");

            try
            {
                IFileManager fileManager = new FileManager(logger);
                string? caminhoDaPasta = fileManager.LerInput();
                logger.LogInformation($"O caminho da pasta digitado é {caminhoDaPasta}");

                string[] arquivos = fileManager.LerPasta(caminhoDaPasta);

                fileManager.LerArquivos(arquivos);
            }
            catch (Exception ex)
            {
                logger.LogError($"Ocorreu um erro inesperado: {ex.ToString()}");
            }
        }
    }

}


