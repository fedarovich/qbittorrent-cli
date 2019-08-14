using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace QBittorrent.CommandLineInterface.Converters
{
    internal class UriConverter : ITypeConverter
    {
        public static readonly UriConverter Instance = new UriConverter();

        static UriConverter()
        {
        }

        private UriConverter()
        {
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((Uri) value).ToString();
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return new Uri(text, UriKind.RelativeOrAbsolute);
        }
    }
}
