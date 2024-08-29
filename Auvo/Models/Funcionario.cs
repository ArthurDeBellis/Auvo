namespace Auvo.Models
{
    public class Funcionario
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public double TotalReceber { get; set; }
        public double HorasExtras { get; set; }
        public double HorasDebito { get; set; }
        public double DiasFalta { get; set; }
        public double DiasExtras { get; set; }
        public double DiasTrabalhados { get; set; }
    }
}
