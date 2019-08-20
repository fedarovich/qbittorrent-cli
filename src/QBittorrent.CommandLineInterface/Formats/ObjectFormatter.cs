using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CsvHelper;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.Formats
{
    public class ObjectFormatter<T>
    {
        private readonly Action<T> _printList;
        private readonly Func<string, PropertyInfo> _customPropertyBinder;

        public ObjectFormatter(Action<T> printList = null, Func<string, PropertyInfo> customPropertyBinder = null)
        {
            _printList = printList ?? (obj => UIHelper.PrintObject(obj));
            _customPropertyBinder = customPropertyBinder;
        }

        public static Lazy<IEnumerable<(string name, PropertyInfo prop)>> GetCommandPropertyMappings(Type commandType)
        {
            return new Lazy<IEnumerable<(string name, PropertyInfo prop)>>(() =>
                from commandProp in commandType.GetTypeInfo().DeclaredProperties
                let option = commandProp.GetCustomAttribute<OptionAttribute>()
                where option != null
                join prop in typeof(T).GetProperties() on commandProp.Name equals prop.Name
                select (new CommandOption(option.Template, option.OptionType.GetValueOrDefault()).LongName, prop));
        }

        public void PrintFormat(in T data, string formatOptions)
        {
            var (format, options) = FormatParser.Parse(formatOptions);
            if (string.IsNullOrWhiteSpace(format))
            {
                format = ObjectFormats.List;
            }

            switch (format)
            {
                case ObjectFormats.List:
                    _printList(data);
                    break;
                case ObjectFormats.Json:
                    PrintJson(data, options.GetJsonOptions());
                    break;
                case ObjectFormats.Csv:
                    PrintCsv(data, options.GetCsvOptions());
                    break;
                case ObjectFormats.Property:
                    PrintProperty(data, options.GetPropertyOptions());
                    break;
            }
        }

        private void PrintJson(in T data, JsonFormatOptions options)
        {
            var serializer = JsonSerializer.Create(options);
            serializer.Serialize(Console.Out, data);
            Console.WriteLine();
        }

        private void PrintCsv(in T data, CsvFormatOptions options)
        {
            using (var writer = new CsvWriter(Console.Out, options, true))
            {
                writer.Configuration.TypeConverterCache.AddConverter<Uri>(UriConverter.Instance);
                writer.WriteRecords(new [] {data});
            }
        }

        private void PrintProperty(in T data, PropertyFormatOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Name))
                throw new Exception("Property name is required.");

            var type = typeof(T);
            var property = TryGetPropertyByName()
                ?? TryGetPropertyByJsonName()
                ?? TryGetPropertyByDisplayName()
                ?? _customPropertyBinder?.Invoke(options.Name)
                ?? throw new Exception($"Cannot find property '{options.Name}'."); ;
            var value = property.GetValue(data);

            switch (value)
            {
                case null:
                    Console.WriteLine();
                    return;
                case string str:
                    Console.WriteLine(str);
                    break;
                case IEnumerable enumerable:
                    foreach (var item in enumerable)
                    {
                        PrintSingleObject(item);
                    }
                    break;
                default:
                    PrintSingleObject(value);
                    break;
            }


            PropertyInfo TryGetPropertyByName() => type.GetProperty(options.Name)
                ?? type.GetProperty(options.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            PropertyInfo TryGetPropertyByJsonName()
            {
                var props =
                    from p in type.GetProperties()
                    let attr = p.GetCustomAttribute<JsonPropertyAttribute>()
                    where string.Equals(attr?.PropertyName, options.Name, StringComparison.OrdinalIgnoreCase)
                    select p;

                return props.FirstOrDefault();
            }

            PropertyInfo TryGetPropertyByDisplayName()
            {
                var props =
                    from p in type.GetProperties()
                    let attr = p.GetCustomAttribute<DisplayAttribute>()
                    where string.Equals(attr?.Name, options.Name, StringComparison.OrdinalIgnoreCase)
                    select p;

                return props.FirstOrDefault();
            }

            void PrintSingleObject(object item)
            {
                var culture = string.IsNullOrWhiteSpace(options.Culture)
                    ? CultureInfo.InvariantCulture
                    : CultureInfo.GetCultureInfo(options.Culture);
                if (item is IFormattable formattable)
                {
                    item = formattable.ToString(options.Format, culture);
                }
                Console.WriteLine(item);
            }
        }
    }
}
