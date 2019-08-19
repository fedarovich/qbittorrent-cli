using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QBittorrent.CommandLineInterface.Formats
{
    public static class FormatParser
    {
        public static (string format, IReadOnlyDictionary<string, string> options) Parse(string formatString)
        {
            if (string.IsNullOrWhiteSpace(formatString))
                return (null, null);

            var parts = Regex.Split(formatString, @"(?<!\\):");
            return (parts[0].ToLowerInvariant(), parts.Skip(1).Where(o => !string.IsNullOrWhiteSpace(o)).Select(ParseOption).ToDictionary(x => x.key, x => x.value));

            (string key, string value) ParseOption(string option)
            {
                var optionParts = option.Split(new [] { '=' }, 2);
                return (
                    optionParts[0], 
                    optionParts.ElementAtOrDefault(1)
                        ?.Replace(@"\:", ":")
                        .Replace(@"\t", "\t"));
            }
        }

        public static JsonFormatOptions GetJsonOptions(this IReadOnlyDictionary<string, string> options)
        {
            return new JsonFormatOptions
            {
                Ident = options.TryGetBoolean("ident", true)
            };
        }

        public static CsvFormatOptions GetCsvOptions(this IReadOnlyDictionary<string, string> options)
        {
            return new CsvFormatOptions
            {
                Delimiter = options.TryGetNotEmptyString("delimiter", ","),
                Quote = options.TryGetChar("quote", '"'),
                Sanitize = false
            };
        }

        public static PropertyFormatOptions GetPropertyOptions(this IReadOnlyDictionary<string, string> options)
        {
            return new PropertyFormatOptions
            {
                Name = options.TryGetNotEmptyString("name", null),
                Format = options.TryGetNotEmptyString("format", null),
                Culture = options.TryGetNotEmptyString("culture", null)
            };
        }

        public static bool TryGetBoolean(this IReadOnlyDictionary<string, string> options, string key, bool defaultValue = false)
        {
            return options.TryGetValue(key, out var stringValue) && bool.TryParse(stringValue, out bool result) ? result : defaultValue;
        }

        public static char TryGetChar(this IReadOnlyDictionary<string, string> options, string key, char defaultValue = default)
        {
            return options.TryGetValue(key, out var stringValue) && char.TryParse(stringValue, out char result) ? result : defaultValue;
        }

        public static string TryGetNotEmptyString(this IReadOnlyDictionary<string, string> options, string key, string defaultValue)
        {
            return options.TryGetValue(key, out var stringValue) && !string.IsNullOrWhiteSpace(stringValue) ? stringValue : defaultValue;
        }
    }
}
