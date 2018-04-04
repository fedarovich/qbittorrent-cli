using System;
using System.Runtime.CompilerServices;
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
                if (foregroundColor != null || backgroundColor != null)
                {
                    console.ResetColor();
                }
            }

            return console;
        }

        public static IConsole WriteLineColored(this IConsole console, string text,
            ConsoleColor? foregroundColor = null,
            ConsoleColor? backgroundColor = null)
        {
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
                if (foregroundColor != null || backgroundColor != null)
                {
                    console.ResetColor();
                }
            }

            return console;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IConsole WriteLineColored(this IConsole console, string text, ColorSet colorSet)
        {
            return console.WriteLineColored(text, colorSet?.GetEffectiveForeground(), colorSet?.GetEffectiveBackground());
        }
    }
}
