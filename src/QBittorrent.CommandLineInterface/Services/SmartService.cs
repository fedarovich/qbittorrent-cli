using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QBittorrent.CommandLineInterface.Services
{
    public class SmartService
    {
        private const string SchemaUri =
            "https://raw.githubusercontent.com/fedarovich/qbittorrent-cli/smart-commands/src/QBittorrent.CommandLineInterface/Schemas/smart-schema.json";

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

        public bool Initialize()
        {
            if (File.Exists(ConfigPath))
                return false;

            SettingsService.Instance.EnsureUserDir();
            JObject config = new JObject(
                new JObject(
                    new JProperty("$schema", SchemaUri),
                    new JProperty("add", new JArray()),
                    new JProperty("move", new JArray())));

            using (var stream = File.Open(ConfigPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var textWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                config.WriteTo(jsonWriter);
            }

            return true;
        }
    }
}
