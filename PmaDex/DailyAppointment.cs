namespace PmaDex
{
    public class DailyAppointment
    {
        public string Data { get; set; }

        public string Inicio { get; set; }

        public string Fim { get; set; }

        public string Intervalo { get; set; }

        public string Resumo {
            get 
            {
                return this.Inicio + " - " + this.Fim + " (" + this.Intervalo + ")"; 
            }

            set { this.Resumo = value; }
        }
    }
}
