using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.ColorSchemes;

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
            var cellStroke = new LineThickness(LineWidth.None, LineWidth.None);
            var valuePadding = new Thickness(1, 0, 0, 0);
            const string DefaultFormat = "{0}";

            var properties = (
                    from prop in typeof(T).GetRuntimeProperties()
                    let attr = prop.GetCustomAttribute<DisplayAttribute>()
                    let name = attr?.Name ?? prop.Name
                    let formatAttr = prop.GetCustomAttribute<DisplayFormatAttribute>()
                    orderby attr?.GetOrder() ?? 0
                    select (name, value: prop.GetValue(obj), format: formatAttr?.DataFormatString ?? DefaultFormat, nullString: formatAttr?.NullDisplayText)
                ).ToList();

            var document = new Document
            {
                Background = ColorScheme.Current.Normal.GetEffectiveBackground(),
                Color = ColorScheme.Current.Normal.GetEffectiveForeground(),
                Children =
                {
                    new Grid()
                    {
                        Stroke = cellStroke,
                        Columns =
                        {
                            new Column() { Width = GridLength.Auto },
                            new Column() { Width = GridLength.Star(1) }
                        },
                        Children =
                        {
                            GetPairs().Select(p => Row(p.label, p.value))
                        }
                    }
                }
            };
            ConsoleRenderer.RenderDocument(document);

            return console;

            IEnumerable<(string label, string value)> GetPairs()
            {
                foreach (var property in properties)
                {
                    var label = property.name + ":";
                    var value = (property.value == null && property.nullString != null)
                        ? property.nullString
                        : string.Format(property.format, property.value);
                    yield return (label, value);
                }
            }

            object[] Row(string label, string value) => new object[]
            {
                new Cell(label) { Stroke = cellStroke, Color = ColorScheme.Current.Strong.Foreground },
                new Cell(value) { Stroke = cellStroke, Padding = valuePadding }
            };
        }
    }
}
