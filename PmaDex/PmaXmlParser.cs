using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PmaDex
{
    class PmaXmlParser
    {
        public static bool isError(string response)
        {
            XDocument entry = XDocument.Parse(response);
            string responseType = (string)entry.Element("response").Element("responseType");
            return responseType.Equals("1");
        }

        public static bool isSuccess(string response)
        {
            return !isError(response);
        }

        public static string getErrorMessage(string response)
        {
            XDocument entry = XDocument.Parse(response);
            return (string)entry.Element("response").Element("content").Element("erros").Element("erro");
        }

    }
}
