using System.Globalization;
using CsvHelper.Configuration;

namespace QBittorrent.CommandLineInterface.Formats
{
    public class CsvFormatOptions
    {
        public string Delimiter { get; set; }

        public char Quote { get; set; }

        public bool Sanitize { get; set; }

        public string Culture { get; set; }

        public static implicit operator Configuration(CsvFormatOptions options)
        {
            return new Configuration
            {
                Delimiter = options.Delimiter,
                Quote = options.Quote,
                SanitizeForInjection = options.Sanitize,
                CultureInfo = string.IsNullOrWhiteSpace(options.Culture) 
                    ? CultureInfo.InvariantCulture 
                    : CultureInfo.GetCultureInfo(options.Culture)
            };
        }
    }
}
