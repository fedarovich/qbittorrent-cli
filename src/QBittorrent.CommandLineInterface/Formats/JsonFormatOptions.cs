using Newtonsoft.Json;

namespace QBittorrent.CommandLineInterface.Formats
{
    public class JsonFormatOptions
    {
        public bool Ident { get; set; }

        public static implicit operator JsonSerializerSettings(JsonFormatOptions options)
        {
            return new JsonSerializerSettings
            {
                Formatting = options.Ident ? Formatting.Indented : Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }
    }
}
