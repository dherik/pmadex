using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmaDex.Util
{
    public static class DateUtil
    {

        public static string FormatToDiaMesAno(this DatePicker data)
        {
            return string.Format("{0:yyyy-MM-dd}", data.Value);
        }

        public static string GetEffortInMinutes(this TimePicker tpkEffort)
        {
            DateTime effortDt = (DateTime)tpkEffort.Value;
            DateTime baseDt = new DateTime(effortDt.Year, effortDt.Month, effortDt.Day, 0, 0, 0, 0, effortDt.Kind);
            string minutes = effortDt.Subtract(baseDt).TotalMinutes.ToString();
            return minutes;
        }
    }
}
