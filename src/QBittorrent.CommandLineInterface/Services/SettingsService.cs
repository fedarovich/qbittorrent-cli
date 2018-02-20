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
            EnsureUserDir();
            EncryptionService.Instance.ResetKey();
            var file = new FileInfo(GetSettingsPath());

            var serializer = new JsonSerializer();
            using (var stream = file.Open(FileMode.Create, FileAccess.Write))
            using (var textWriter = new StreamWriter(stream, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(textWriter) {Formatting = Formatting.Indented} )
            {
                serializer.Serialize(jsonWriter, settings);
            }
        }

        private string GetUserDir()
        {
            string root;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                root = Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.Create);
            }
            else
            {
                root = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }

            return Path.Combine(root, ".qbt");
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
