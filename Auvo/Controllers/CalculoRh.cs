using Auvo.Interfaces;
using Auvo.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Auvo.Controllers
{
    internal class CalculoRh : ICalculoRh
    {
        private readonly ILogger _logger;

        public CalculoRh(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Esta função calcula as informações relativas a cada departamento, a lógica de dias úteis é feita a partir do mês e 
        /// a lógica de horas é com base em um dia de trabalho com 8 horas 
        /// </summary>
        /// <param name="arquivos">Os arquivos que foram lidos da pasta</param>
        /// <returns>A Lista de Informações por Departamento</returns>
        /// <exception cref="ArgumentException">Erro disparado caso a lista de arquivos esteja vazia</exception>
        public async Task<List<InformacoesDepartamento>> CalcularInformacoes(List<ArquivoCSV> arquivos)
        {
            List<InformacoesDepartamento> departamentos = new List<InformacoesDepartamento>();

            try
            {
                if (!arquivos.Any())
                {
                    throw new ArgumentException(nameof(arquivos));
                }

                foreach (ArquivoCSV arquivo in arquivos)
                {
                    string nomearquivo = arquivo.NomeArquivo.Substring(0, arquivo.NomeArquivo.LastIndexOf('.')); // Retira a extensão .csv da string
                    string[] infos = nomearquivo.Split('-');

                    InformacoesDepartamento departamento = new InformacoesDepartamento
                    {
                        Departamento = infos[0],
                        MesVigencia = infos[1],
                        AnoVigencia = int.Parse(infos[2]),
                    };

                    // Variáveis que serão preenchidas posteriormente
                    double totalPagar = 0;
                    double totalDescontos = 0;
                    double totalExtras = 0;
                    List<Funcionario> funcionarios = new List<Funcionario>();

                    var infosPorFuncionario = arquivo.Informacoes.GroupBy(info => info.Codigo);
                    
                    foreach (var funcionario in infosPorFuncionario)
                    {

                        var informacoesFuncionario = funcionario.ToList();

                        string nome  = informacoesFuncionario[0].Nome;
                        string codigo  = informacoesFuncionario[0].Codigo;
                        double totalReceber  = 0;
                        double horasExtras  = 0;
                        double horasDebito  = 0;
                        double diasFalta  = 0;
                        double diasExtras  = 0;
                        int diasTrabalhados  = 0;

                        string expressaoRegular = @"[R$ ]";

                        double valorHoraGlobal = Double.Parse(Regex.Replace(informacoesFuncionario[0].ValorHora, expressaoRegular, ""));

                        int mes = DateTime.ParseExact(departamento.MesVigencia, "MMMM", new CultureInfo("pt-BR")).Month;
                        int quantidadeDeDiasUteis = ObtemQuantidadeDeDiasUteis(departamento.AnoVigencia, mes);
                        
                        Funcionario func = new Funcionario
                        {
                            Nome = nome,
                            Codigo = codigo,
                        };

                        foreach (var informacao in informacoesFuncionario)
                        {
                            DateTime data = ObtemInformacaoDateTime(informacao.Data, "dd/MM/yyyy");
                            DateTime entrada = ObtemInformacaoDateTime(informacao.Entrada, "HH:mm:ss");
                            DateTime saida = ObtemInformacaoDateTime(informacao.Saida, "HH:mm:ss");
                            var horasTotaisDia = saida - entrada;
                            string[] horariosAlmoco = informacao.Almoco.Split(" - ");
                            DateTime almocoInicio = ObtemInformacaoDateTime(horariosAlmoco[0], "HH:mm");
                            DateTime almocoFim = ObtemInformacaoDateTime(horariosAlmoco[1], "HH:mm");
                            var horasTotaisAlmoco = almocoFim - almocoInicio;
                            var totalDeHoras = horasTotaisDia - horasTotaisAlmoco;

                            double valorHora = Double.Parse(Regex.Replace(informacao.ValorHora, expressaoRegular, ""));

                            double horaExtraDia = totalDeHoras.TotalHours - 8;

                            bool eFinalDeSemana = data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday;

                            // Para finais de semana todas as horas são extras
                            if (eFinalDeSemana)
                            {
                                diasExtras += 1;
                                horasExtras += totalDeHoras.TotalHours;
                                totalExtras += totalDeHoras.TotalHours * valorHora;
                            }
                            else
                            {
                                diasTrabalhados += 1;

                                // Se houver horas extras as horas extras totais também são contabilizadas
                                if(horaExtraDia >= 0)
                                {
                                    // Caso o valor da variavel horaExtraDia seja zero, não há alteração no valor original das variáveis
                                    horasExtras += horaExtraDia;
                                    totalExtras += horaExtraDia * valorHora;
                                }
                                else
                                {
                                    horasDebito += 8 - totalDeHoras.TotalHours;
                                    totalDescontos += (8 - totalDeHoras.TotalHours) * valorHora;
                                }

                            }

                            // O funcionário receberá o total de horas trabalhadas * valor de sua hora independente da quantidade de horas
                            totalReceber += totalDeHoras.TotalHours * valorHora;

                        }

                        diasFalta = quantidadeDeDiasUteis - diasTrabalhados;
                        totalDescontos = diasFalta * valorHoraGlobal;

                        func.TotalReceber = Math.Round(totalReceber, 2);
                        func.HorasExtras = Math.Round(horasExtras, 2);
                        func.HorasDebito = Math.Round(horasDebito, 2);
                        func.DiasFalta = diasFalta;
                        func.DiasExtras = diasExtras;
                        func.DiasTrabalhados = diasTrabalhados;

                        funcionarios.Add(func);
                    }

                    totalPagar = funcionarios.Sum(x => x.TotalReceber);

                    departamento.TotalPagar = Math.Round(totalPagar, 2);
                    departamento.TotalDescontos = Math.Round(totalDescontos, 2);
                    departamento.TotalExtras = Math.Round(totalExtras, 2);
                    departamento.Funcionarios = funcionarios;

                    departamentos.Add(departamento);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Ocorreu um erro ao obter as informações do arquivo. {ex.ToString()}");
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError($"O nome do arquivo não foi formatado de forma correta. {ex.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocorreu um erro inesperado. {ex.ToString()}");
            }

            return departamentos;
        }

        internal int ObtemQuantidadeDeDiasUteis(int ano, int mes)
        {
            int quantidadeDeDias = DateTime.DaysInMonth(ano, mes);

            return Enumerable.Range(1, quantidadeDeDias)
                                .Select(dia => new DateTime(ano, mes, dia))
                                .Count(data => data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday);
        }

        internal DateTime ObtemInformacaoDateTime(string data, string formato)
        {
            return DateTime.ParseExact(data, formato, new CultureInfo("pt-BR"));
        }
    }
}
