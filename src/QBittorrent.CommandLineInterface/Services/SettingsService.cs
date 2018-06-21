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

        public Settings GetGeneral()
        {
            var file = new FileInfo(GetGeneralSettingsPath());
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

        public NetworkSettings GetNetwork()
        {
            var file = new FileInfo(GetNetworkSettingsPath());
            if (!file.Exists)
            {
                var settings = GetGeneral();
                var proxy = GetLegacyProxySettings(settings);
                return GetLegacyNetworkSettings(settings, proxy);
            }

            var serializer = new JsonSerializer();
            using (var textReader = file.OpenText())
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<NetworkSettings>(jsonReader);
            }

            ProxySettings GetLegacyProxySettings(Settings settings)
            {
                return settings.Other.TryGetValue("Proxy", out var jtoken) ? jtoken.ToObject<ProxySettings>() : null;
            }

            NetworkSettings GetLegacyNetworkSettings(Settings settings, ProxySettings proxy)
            {
                if (!settings.Other.TryGetValue("NetworkSettings", out var jtoken))
                    return new NetworkSettings();

                var networkSettings = jtoken.ToObject<NetworkSettings>();
                networkSettings.Proxy = proxy;
                return networkSettings;
            }
        }

        public void Save(Settings settings)
        {
            EnsureUserDir();
            EncryptionService.Instance.ResetKey();
            var file = new FileInfo(GetGeneralSettingsPath());

            var serializer = new JsonSerializer();
            using (var stream = file.Open(FileMode.Create, FileAccess.Write))
            using (var textWriter = new StreamWriter(stream, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(textWriter) {Formatting = Formatting.Indented} )
            {
                serializer.Serialize(jsonWriter, settings);
            }
        }

        public void Save(NetworkSettings settings)
        {
            EnsureUserDir();
            EncryptionService.Instance.ResetKey();
            var file = new FileInfo(GetNetworkSettingsPath());

            var serializer = new JsonSerializer();
            using (var stream = file.Open(FileMode.Create, FileAccess.Write))
            using (var textWriter = new StreamWriter(stream, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(textWriter) { Formatting = Formatting.Indented })
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

        private string GetGeneralSettingsPath()
        {
            return Path.Combine(GetUserDir(), "settings.json");
        }

        private string GetNetworkSettingsPath()
        {
            return Path.Combine(GetUserDir(), "settings.json");
        }
    }
}
