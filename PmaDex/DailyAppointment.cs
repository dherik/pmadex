using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmaDex
{
    class DailyAppointment
    {
        public string Data { get; set; }
        public string Inicio { get; set; }
        public string Fim { get; set; }
        public string Intervalo { get; set; }
        public string Resumo {
            get 
            { 
                return Inicio + " - " + Fim + " (" + Intervalo + ")"; 
            }
            set { Resumo = value; }
        }
    }
}
