using Auvo.Controllers;
using Auvo.Interfaces;
using Auvo.Models;
using Microsoft.Extensions.Logging;

namespace Auvo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Configurando o LoggerFactory
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            
            // Criando um logger
            ILogger logger = factory.CreateLogger("Program");

            try
            {
                // Controladores
                IFileManager fileManager = new FileManager(logger);
                ICalculoRh calculoRh = new CalculoRh(logger);

                string caminhoDaPasta = await fileManager.LerInput();
                logger.LogInformation($"O caminho da pasta digitado é {caminhoDaPasta}");

                string[] arquivos = await fileManager.LerPasta(caminhoDaPasta);
                List<ArquivoCSV> informacoes = await fileManager.LerArquivos(arquivos);

                List<InformacoesDepartamento> calculo = await calculoRh.CalcularInformacoes(informacoes);

                Console.WriteLine(calculo);

            }
            catch (Exception ex)
            {
                logger.LogError($"Ocorreu um erro inesperado: {ex.ToString()}");
            }
        }
    }

}


