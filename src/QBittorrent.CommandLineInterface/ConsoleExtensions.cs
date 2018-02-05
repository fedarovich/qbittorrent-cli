using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface
{
    public static class ConsoleExtensions
    {
        public static IConsole WriteLine(this IConsole console)
        {
            return console.WriteLine(string.Empty);
        }

        public static IConsole WriteColored(this IConsole console, string text,
            ConsoleColor? foregroundColor = null,
            ConsoleColor? backgroundColor = null)
        {
            ConsoleColor fg = console.ForegroundColor;
            ConsoleColor bg = console.BackgroundColor;
            if (foregroundColor != null)
            {
                console.ForegroundColor = foregroundColor.Value;
            }
            if (backgroundColor != null)
            {
                console.BackgroundColor = backgroundColor.Value;
            }

            try
            {
                console.Write(text);
            }
            finally
            {
                console.BackgroundColor = bg;
                console.ForegroundColor = fg;
            }

            return console;
        }

        public static IConsole WriteLineColored(this IConsole console, string text,
            ConsoleColor? foregroundColor = null,
            ConsoleColor? backgroundColor = null)
        {
            ConsoleColor fg = console.ForegroundColor;
            ConsoleColor bg = console.BackgroundColor;
            if (foregroundColor != null)
            {
                console.ForegroundColor = foregroundColor.Value;
            }
            if (backgroundColor != null)
            {
                console.BackgroundColor = backgroundColor.Value;
            }

            try
            {
                console.WriteLine(text);
            }
            finally
            {
                console.BackgroundColor = bg;
                console.ForegroundColor = fg;
            }

            return console;
        }

        public static IConsole PrintObject<T>(this IConsole console, T obj)
        {
            const string DefaultFormat = "{0}";

            var properties = (
                    from prop in typeof(T).GetRuntimeProperties()
                    let attr = prop.GetCustomAttribute<DisplayAttribute>()
                    let name = attr?.Name ?? prop.Name
                    let formatAttr = prop.GetCustomAttribute<DisplayFormatAttribute>()
                    orderby attr?.GetOrder() ?? 0
                    select (name, value: prop.GetValue(obj), format: formatAttr?.DataFormatString ?? DefaultFormat, nullString: formatAttr?.NullDisplayText)
                ).ToList();

            var columnWidth = properties.Max(p => p.name.Length) + 1;
            foreach (var property in properties)
            {
                var name = (property.name + ":").PadRight(columnWidth);
                var value = (property.value == null && property.nullString != null)
                    ? property.nullString
                    : string.Format(property.format, property.value);
                console
                    .WriteColored(name, ConsoleColor.Yellow)
                    .WriteLineColored(" " + value, ConsoleColor.White);
            }

            return console;
        }
    }
}
