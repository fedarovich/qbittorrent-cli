using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace QBittorrent.CommandLineInterface.Services
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();

        static SettingsService()
        {
        }

        private SettingsService()
        {
        }

        public Settings Get()
        {
            var file = new FileInfo(GetSettingsPath());
            if (!file.Exists)
            {
                return new Settings();
            }

            var serializer = new JsonSerializer();
            using (var textReader = file.OpenText())
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<Settings>(jsonReader);
            }
        }

        public void Save(Settings settings)
        {

        }

        private string GetUserDir()
        {
            var appData = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData,
                Environment.SpecialFolderOption.Create);
            return Path.Combine(appData, ".qbt");
        }

        private void EnsureUserDir()
        {
            Directory.CreateDirectory(GetUserDir());
        }

        private string GetSettingsPath()
        {
            return Path.Combine(GetUserDir(), "settings.json");
        }
    }
}
