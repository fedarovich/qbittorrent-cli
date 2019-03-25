using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Alba.CsConsoleFormat;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface
{
    public static class UIHelper
    {
        public static readonly LineThickness NoneStroke = new LineThickness(LineWidth.None, LineWidth.None);
        public static readonly LineThickness GridHeaderStroke = new LineThickness(LineWidth.Single, LineWidth.Double);

        public static object[] FieldsColumns =>
            // ReSharper disable once CoVariantArrayConversion
            new[]
            {
                new Column {Width = GridLength.Auto},
                new Column {Width = GridLength.Star(1)}
            };

        public static Cell Label(string text) => new Cell(text + ":") { Color = ColorScheme.Current.Strong.Foreground, Stroke = NoneStroke };

        public static Cell Data<T>(T data) => new Cell(data?.ToString()) { Stroke = NoneStroke, Padding = new Thickness(1, 0, 0, 0) };

        public static Cell Header(string text, TextAlign? textAlign = null, int? minWidth = null)
        {
            var cell = new Cell(text) { Stroke = GridHeaderStroke };
            cell.TextAlign = textAlign ?? cell.TextAlign;
            cell.MinWidth = minWidth ?? cell.MinWidth;
            return cell;
        }

        public static object[] Row<T>(string label, T data)
        {
            Cell dataCell;
            switch (data)
            {
                case Cell cell:
                    dataCell = cell;
                    break;
                case Element element:
                    dataCell = new Cell(element) { Stroke = NoneStroke, Padding = new Thickness(1, 0, 0, 0) };
                    break;
                default:
                    dataCell = Data(data);
                    break;
            }

            return new object[] { Label(label), dataCell };
        }

        public static void PrintObject<T>(T obj,
            IReadOnlyDictionary<string, Func<object, object>> customFormatters = null)
        {
            var document = ToDocument(obj, customFormatters);
            ConsoleRenderer.RenderDocument(document);
        }

        public static Document ToDocument<T>(T obj,
            IReadOnlyDictionary<string, Func<object, object>> customFormatters = null)
        {
            const string DefaultFormat = "{0}";

            var properties = (
                    from prop in typeof(T).GetRuntimeProperties()
                    let attr = prop.GetCustomAttribute<DisplayAttribute>()
                    let name = attr?.Name ?? prop.Name
                    let formatAttr = prop.GetCustomAttribute<DisplayFormatAttribute>()
                    orderby attr?.GetOrder() ?? 0
                    select (name, value: prop.GetValue(obj), format: formatAttr?.DataFormatString ?? DefaultFormat, nullString: formatAttr?.NullDisplayText, propName: prop.Name)
                ).ToList();

            var document = new Document
            {
                Background = ColorScheme.Current.Normal.GetEffectiveBackground(),
                Color = ColorScheme.Current.Normal.GetEffectiveForeground(),
                Children =
                {
                    new Grid
                    {
                        Stroke = NoneStroke,
                        Columns =
                        {
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Star(1) }
                        },
                        Children =
                        {
                            GetPairs().Select(p => Row(p.label, p.value))
                        }
                    }
                }
            };
            return document;

            IEnumerable<(string label, object value)> GetPairs()
            {
                foreach (var property in properties)
                {
                    var label = property.name;
                    if (customFormatters != null && customFormatters.TryGetValue(property.propName, out var formatter))
                    {
                        var customValue = formatter(property.value);
                        if (customValue != null)
                        {
                            yield return (label, customValue);
                            continue;
                        }
                    }

                    var value = (property.value == null && property.nullString != null)
                        ? property.nullString
                        : string.Format(property.format, property.value);
                    yield return (label, value);
                }
            }
        }

        public static T SetColors<T>(this T element, ColorSet colors)
            where T : Element
        {
            element.Background = colors.GetEffectiveBackground();
            element.Color = colors.GetEffectiveForeground();
            return element;
        }

        public static T With<T>(this T element, Action<T> action)
            where T : Element
        {
            action(element);
            return element;
        }
    }
}
