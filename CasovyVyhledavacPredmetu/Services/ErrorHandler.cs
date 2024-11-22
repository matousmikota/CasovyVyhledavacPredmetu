using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasovyVyhledavacPredmetuSIS.Services
{
    public static class ErrorHandler
    {
        /// <summary>
        /// Vypíše chybovou hlášku do konzole a debugu jako řádek
        /// </summary>
        public static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
        /// <summary>
        /// Předá message Log metodě a vyhodí výjimku
        /// </summary>
        public static void LogAndThrow(string message, Exception ex)
        {
            Log(message);

            throw new Exception(message, ex);
        }
    }
}
