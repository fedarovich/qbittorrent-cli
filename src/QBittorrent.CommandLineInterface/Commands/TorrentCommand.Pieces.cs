using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("pieces", typeof(Pieces))]
    public partial class TorrentCommand
    {
        [Command(Description = "Shows the torrent pieces' hashes and states.")]
        public class Pieces : TorrentSpecificCommandBase
        {
            [Option("-d|--display <MODE>", "Display Mode (FULL|HASHES|STATES|DIAGRAM). FULL is default", CommandOptionType.SingleValue)]
            [EnumValidation(typeof(DisplayMode), AllowEmpty = true)]
            public string Display { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                if (!Enum.TryParse(Display, true, out DisplayMode mode) && !string.IsNullOrEmpty(Display))
                    return ExitCodes.WrongUsage;

                switch (mode)
                {
                    case DisplayMode.Full:
                        await ShowFull();
                        break;
                    case DisplayMode.Hashes:
                        await ShowHashes();
                        break;
                    case DisplayMode.States:
                        await ShowStates();
                        break;
                    case DisplayMode.Diagram:
                        await ShowDiagram();
                        break;
                }

                async Task ShowFull()
                {
                    var (hashes, states) = await TaskHelper.WhenAll(
                        client.GetTorrentPiecesHashesAsync(Hash),
                        client.GetTorrentPiecesStatesAsync(Hash));
                    var width = (int)Math.Log10(hashes.Count) + 1;
                    var sequence = hashes.Zip(states, (hash, state) => (hash, state));

                    int index = 0;
                    foreach (var (hash, state) in sequence)
                    {
                        console.WriteLineColored($"{(index++).ToString().PadLeft(width)}  {hash}  {state}", ColorScheme.Current.Normal);
                    }
                }

                async Task ShowHashes()
                {
                    var hashes = await client.GetTorrentPiecesHashesAsync(Hash);
                    var width = (int)Math.Log10(hashes.Count) + 1;

                    int index = 0;
                    foreach (var hash in hashes)
                    {
                        console.WriteLineColored($"{(index++).ToString().PadLeft(width)}  {hash}", ColorScheme.Current.Normal);
                    }
                }

                async Task ShowStates()
                {
                    var states = await client.GetTorrentPiecesStatesAsync(Hash);
                    var width = (int)Math.Log10(states.Count) + 1;

                    int index = 0;
                    foreach (var state in states)
                    {
                        console.WriteLineColored($"{(index++).ToString().PadLeft(width)}  {state}", ColorScheme.Current.Normal);
                    }
                }

                async Task ShowDiagram()
                {
                    var fgColors = new[] {ConsoleColor.White, ConsoleColor.DarkGreen, ConsoleColor.Cyan};
                    var bgColors = new[] {ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.DarkBlue};

                    console
                        .WriteColored(".", fgColors[0], bgColors[0])
                        .Write(" Not Downloaded   ")
                        .WriteColored(".", fgColors[1], bgColors[1])
                        .Write(" Downloading   ")
                        .WriteColored(".", fgColors[2], bgColors[2])
                        .WriteLine(" Downloaded")
                        .WriteLine();

                    var states = await client.GetTorrentPiecesStatesAsync(Hash);
                    var width = (int)Math.Log10(states.Count) + 1;
                    var rowWidth = (Console.BufferWidth > 100 + width) ? 100 : 50;

                    for (int index = 0; index < states.Count; index += rowWidth)
                    {
                        console.Write(index.ToString().PadLeft(width) + " ");
                        for (int offset = 0; offset < rowWidth && index + offset < states.Count; offset++)
                        {
                            var state = (int)states[index + offset];
                            console.WriteColored(".", fgColors[state], bgColors[state]);
                        }
                        console.WriteLine();
                    }
                }

                return ExitCodes.Success;
            }

            public enum DisplayMode
            {
                Full,
                Hashes,
                States,
                Diagram
            }
        }
    }
}
