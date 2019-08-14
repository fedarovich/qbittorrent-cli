using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using Newtonsoft.Json;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.Formats
{
    public class ListFormatter<T>
    {
        private readonly Action<IEnumerable<T>> _printTable;
        private readonly Action<IEnumerable<T>> _printList;

        public ListFormatter(Action<IEnumerable<T>> printTable, Action<IEnumerable<T>> printList)
        {
            _printTable = printTable;
            _printList = printList;
        }

        public void PrintFormat(IEnumerable<T> data, string formatOptions, bool preferList = false)
        {
            var (format, options) = FormatParser.Parse(formatOptions);
            if (string.IsNullOrWhiteSpace(format))
            {
                format = preferList ? ListFormats.List : ListFormats.Table;
            }

            switch (format)
            {
                case ListFormats.Table when _printTable != null:
                    _printTable(data);
                    break;
                case ListFormats.List when _printList != null:
                    _printList(data);
                    break;
                case ListFormats.Json:
                    PrintJson(data, options.GetJsonOptions());
                    break;
                case ListFormats.Csv:
                    PrintCsv(data, options.GetCsvOptions());
                    break;
                default:
                    throw new Exception("Unsupported output format.");
            }
        }

        private void PrintJson(IEnumerable<T> data, JsonFormatOptions options)
        {
            var serializer = JsonSerializer.Create(options);
            serializer.Serialize(Console.Out, data);
            Console.WriteLine();
        }

        private void PrintCsv(IEnumerable<T> data, CsvFormatOptions options)
        {
            using (var writer = new CsvWriter(Console.Out, options, true))
            {
                writer.Configuration.TypeConverterCache.AddConverter<Uri>(UriConverter.Instance);
                writer.WriteRecords(data);
            }
        }
    }
}
