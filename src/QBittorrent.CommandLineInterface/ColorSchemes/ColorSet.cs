using System;
using Newtonsoft.Json;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.ColorSchemes
{
    public class ColorSet
    {
        [JsonProperty("bg")]
        [JsonConverter(typeof(ColorConverter))]
        public Color? Background { get; private set; }

        [JsonProperty("fg")]
        [JsonConverter(typeof(ColorConverter))]
        public Color? Foreground { get; private set; }

        [JsonProperty("fg-alt")]
        [JsonConverter(typeof(ColorConverter))]
        public Color? AltForeground { get; private set; }

        public ConsoleColor GetEffectiveBackground()
        {
            return Background ?? GetSystemBackground();
        }

        public ConsoleColor GetEffectiveForeground()
        {
            ConsoleColor bg = GetEffectiveBackground();
            ConsoleColor systemFg = GetSystemForeground();
            ConsoleColor fg = Foreground ?? systemFg;
            return fg != bg ? fg : (AltForeground ?? systemFg);
        }

        private ConsoleColor GetSystemBackground()
        {
            var color = Console.BackgroundColor;
            return EnumHelper.IsDefined(color) ? color : ConsoleColor.Black;
        }

        private ConsoleColor GetSystemForeground()
        {
            var color = Console.ForegroundColor;
            return EnumHelper.IsDefined(color) ? color : ConsoleColor.White;
        }
    }
}
