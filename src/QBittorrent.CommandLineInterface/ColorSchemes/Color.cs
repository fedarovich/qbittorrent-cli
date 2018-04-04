using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;

namespace QBittorrent.CommandLineInterface.ColorSchemes
{
    public struct Color
    {
        private readonly ColorType _type;
        private readonly ConsoleColor? _consoleColor;

        public static readonly Color Black = new Color(System.ConsoleColor.Black);
        public static readonly Color Blue = new Color(System.ConsoleColor.Blue);
        public static readonly Color Cyan = new Color(System.ConsoleColor.Cyan);
        public static readonly Color DarkBlue = new Color(System.ConsoleColor.DarkBlue);
        public static readonly Color DarkCyan = new Color(System.ConsoleColor.DarkCyan);
        public static readonly Color DarkGray = new Color(System.ConsoleColor.DarkGray);
        public static readonly Color DarkGreen = new Color(System.ConsoleColor.DarkGreen);
        public static readonly Color DarkMagenta = new Color(System.ConsoleColor.DarkMagenta);
        public static readonly Color DarkRed = new Color(System.ConsoleColor.DarkRed);
        public static readonly Color DarkYellow = new Color(System.ConsoleColor.DarkYellow);
        public static readonly Color Gray = new Color(System.ConsoleColor.Gray);
        public static readonly Color Green = new Color(System.ConsoleColor.Green);
        public static readonly Color Magenta = new Color(System.ConsoleColor.Magenta);
        public static readonly Color Red = new Color(System.ConsoleColor.Red);
        public static readonly Color White = new Color(System.ConsoleColor.White);
        public static readonly Color Yellow = new Color(System.ConsoleColor.Yellow);

        public static readonly Color SystemBackground = new Color(ColorType.SystemBackground);
        public static readonly Color SystemForeground = new Color(ColorType.SystemForeground);

        public Color(ConsoleColor consoleColor)
        {
            _type = ColorType.Console;
            _consoleColor = consoleColor;
        }

        private Color(ColorType type)
        {
            _type = type;
            _consoleColor = null;
        }

        public Color(string value)
        {
            switch (value)
            {
                case "system-bg":
                    _type = ColorType.SystemBackground;
                    _consoleColor = null;
                    break;
                case "system-fg":
                    _type = ColorType.SystemForeground;
                    _consoleColor = null;
                    break;
                case var _ when Enum.TryParse(value?.Replace("-", ""), true, out ConsoleColor color):
                    _type = ColorType.Console;
                    _consoleColor = color;
                    break;
                default:
                    throw new ArgumentException("Invalid value.", nameof(value));
            }
        }

        public static implicit operator ConsoleColor(Color value)
        {
            switch (value._type)
            {
                case ColorType.Console:
                    // ReSharper disable once PossibleInvalidOperationException
                    return value._consoleColor.Value;
                case ColorType.SystemBackground:
                    return EnumHelper.IsDefined(Console.BackgroundColor) ? Console.BackgroundColor : System.ConsoleColor.Black;
                case ColorType.SystemForeground:
                    return EnumHelper.IsDefined(Console.ForegroundColor) ? Console.ForegroundColor : System.ConsoleColor.White;
                default:
                    throw new ArgumentException();
            }
        }

        public ConsoleColor? ConsoleColor => _consoleColor;

        public override string ToString()
        {
            switch (_type)
            {
                case ColorType.Console:
                    var colorName = _consoleColor.ToString();
                    if (colorName.StartsWith("Dark"))
                        return "dark-" + colorName.Substring(4).ToLowerInvariant();
                    return colorName.ToLowerInvariant();
                case ColorType.SystemBackground:
                    return "system-bg";
                case ColorType.SystemForeground:
                    return "system-fg";
                default:
                    return "<invalid>";
            }
        }
    }
}
