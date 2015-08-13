using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmaDex.Util
{
    public class TokenUtil
    {
        public static void SaveToken(string token)
        {
            // save value
            IsolatedStorageSettings.ApplicationSettings["token"] = token;
        }

        public static string GetToken()
        {
            string token = IsolatedStorageSettings.ApplicationSettings.Contains("token")
                ? (string)IsolatedStorageSettings.ApplicationSettings["token"]
                : string.Empty; // false is default value 
            return token;
        }
    }
}
