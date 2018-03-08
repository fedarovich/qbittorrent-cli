using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using QBittorrent.CommandLineInterface.Exceptions;

namespace QBittorrent.CommandLineInterface.Services
{
    public class SmartService
    {
        private const string SchemaUri =
            "https://raw.githubusercontent.com/fedarovich/qbittorrent-cli/smart-commands/src/QBittorrent.CommandLineInterface/Schemas/smart-schema.json";

        private const string SmartSchemaResource = "QBittorrent.CommandLineInterface.Schemas.smart-schema.json";

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

        public JToken GetConfigurationObject(string path = "$")
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(ConfigPath))
                throw new FileNotFoundException(
                    "The smart command configuration file not found.", ConfigPath);
            
            var root = GetRootObject();
            var schema = ReadSchema();
            var errors = schema.Validate(root);
            if (errors.Any())
                throw new JsonValidationException(
                    "The smart command configuration file has errors.", errors);

            return root.SelectToken(path, true);

            JObject GetRootObject()
            {
                using (var textReader = File.OpenText(ConfigPath))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    return JObject.Load(
                        jsonReader, 
                        new JsonLoadSettings
                        {
                            CommentHandling = CommentHandling.Load,
                            LineInfoHandling = LineInfoHandling.Load
                        });
                }
            }
        }

        public Engine CreateEngine()
        {
            var engine = new Engine(opt => opt.LimitRecursion(100));

            var scriptPath = Path.Combine(SettingsService.Instance.GetUserDir(), "smart.js");
            if (File.Exists(scriptPath))
            {
                using (var reader = File.OpenText(scriptPath))
                {
                    var script = reader.ReadToEnd();
                    engine.Execute(script);
                }
            }

            return engine;
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

        private JsonSchema4 ReadSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SmartSchemaResource))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return JsonSchema4.FromJsonAsync(json).Result;
            }
        }
    }
}
