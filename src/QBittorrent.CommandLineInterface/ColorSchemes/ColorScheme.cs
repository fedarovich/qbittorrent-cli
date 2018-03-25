using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace QBittorrent.CommandLineInterface.ColorSchemes
{
    public class ColorScheme
    {
        private const string SchemaResource = "QBittorrent.CommandLineInterface.Schemas.colors-schema.json";
        private const string DarkResource = "QBittorrent.CommandLineInterface.ColorSchemes.dark.json";
        private const string LightResource = "QBittorrent.CommandLineInterface.ColorSchemes.light.json";

        private static Lazy<ColorScheme> _dark;
        private static Lazy<ColorScheme> _light;
        private static Lazy<ColorScheme> _default;
        private static ColorScheme _current;

        private JObject _config;

        private ColorScheme(JObject config)
        {
            _config = config;
        }

        static ColorScheme()
        {
            _dark = new Lazy<ColorScheme>(() => new ColorScheme(JObject.Parse(ReadJsonFromResource(DarkResource))));
            _light = new Lazy<ColorScheme>(() => new ColorScheme(JObject.Parse(ReadJsonFromResource(LightResource))));
            _default = new Lazy<ColorScheme>(() => IsLight() ? _light.Value : _dark.Value);

            bool IsLight()
            {
                return Console.BackgroundColor == ConsoleColor.White
                    || Console.BackgroundColor == ConsoleColor.Gray
                    || Console.BackgroundColor == ConsoleColor.Yellow
                    || Console.BackgroundColor == ConsoleColor.Cyan
                    || Console.BackgroundColor == ConsoleColor.Magenta;
            }
        }

        public static ColorScheme Dark => _dark.Value;

        public static ColorScheme Light => _light.Value;

        public static ColorScheme Current
        {
            get => _current ?? _default.Value;
            set => _current = value;
        }

        public static async Task<ColorScheme> FromJsonAsync(string json)
        {
            var config = JObject.Parse(json);
            var schema = await LoadSchemaAsync().ConfigureAwait(false);
            var errors = schema.Validate(config);
            if (errors != null && errors.Any())
                throw new Exception("The color scheme file is invalid."); // TODO: Throw specific exception.

            return new ColorScheme(config);
        }

        private static async Task<JsonSchema4> LoadSchemaAsync()
        {
            var json = ReadJsonFromResource(SchemaResource);
            return await JsonSchema4.FromJsonAsync(json).ConfigureAwait(false);
        }

        private static string ReadJsonFromResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return json;
            }
        }

        private static ConsoleColor ConvertColor(string colorName)
        {
            switch (colorName)
            {
                case "system-bg":
                    return Console.BackgroundColor;
                case "system-fg":
                    return Console.ForegroundColor;
                default:
                    var str = colorName.Replace("-", "");
                    return Enum.Parse<ConsoleColor>(str, true);
            }
        }
    }
}
