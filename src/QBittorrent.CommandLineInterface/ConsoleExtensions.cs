using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface
{
    public static class ConsoleExtensions
    {
        public static void WriteColored(this IConsole console, string text,
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
        }

        public static void WriteLineColored(this IConsole console, string text,
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
        }
    }
}
