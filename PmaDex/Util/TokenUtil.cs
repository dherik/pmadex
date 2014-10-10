using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmaDex.Util
{
    class TokenUtil
    {
        public static void SaveTokenToIsolatedStorage(string token)
        {
            // save value
            IsolatedStorageSettings.ApplicationSettings["token"] = token;
        }

        public static string GetTokenFromIsolatedStorage()
        {
            string token = IsolatedStorageSettings.ApplicationSettings.Contains("token")
                ? (string)IsolatedStorageSettings.ApplicationSettings["token"]
                : ""; // false is default value 
            return token;
        }
    }
}
