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
            return Background ?? Console.BackgroundColor;
        }

        public ConsoleColor GetEffectiveForeground()
        {
            ConsoleColor bg = GetEffectiveBackground();
            ConsoleColor fg = Foreground ?? Console.ForegroundColor;
            return fg != bg ? fg : (AltForeground ?? Console.ForegroundColor);
        }
    }
}
