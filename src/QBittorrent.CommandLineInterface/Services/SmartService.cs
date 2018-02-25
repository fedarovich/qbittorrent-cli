using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QBittorrent.CommandLineInterface.Services
{
    public class SmartService
    {
        public static SmartService Instance { get; } = new SmartService();

        private SmartService()
        {
        }

        static SmartService()
        {
        }

        public string ConfigPath
        {
            get
            {
                var dir = SettingsService.Instance.GetUserDir();
                return Path.Combine(dir, "smart.json");
            }
        }
    }
}
