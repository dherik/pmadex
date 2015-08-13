using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PmaDex
{
    public class PmaXmlParser
    {
        public static bool IsError(string response)
        {
            XDocument entry = XDocument.Parse(response);
            string responseType = (string)entry.Element("response").Element("responseType");
            return responseType.Equals("1");
        }

        public static bool IsSuccess(string response)
        {
            return !IsError(response);
        }

        public static string GetErrorMessage(string response)
        {
            XDocument entry = XDocument.Parse(response);
            return (string)entry.Element("response").Element("content").Element("erros").Element("erro");
        }

    }
}
