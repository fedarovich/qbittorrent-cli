using System;
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
    }
}
