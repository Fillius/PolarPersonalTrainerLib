using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPersonalTrainerLib.Tests
{
    public static class Settings
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public static string UserName { get; private set; }
        /// <summary>
        /// Password
        /// </summary>
        public static string Password { get; private set; }

        static Settings()
        {
            UserName = ConfigurationManager.AppSettings["username"];
            Password = ConfigurationManager.AppSettings["password"];
        }
    }
}
