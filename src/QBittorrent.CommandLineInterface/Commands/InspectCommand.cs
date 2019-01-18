using System.ComponentModel.DataAnnotations;
using System.Linq;
using Alba.CsConsoleFormat;
using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Inspects the torrent.")]
    [Subcommand(typeof(File))]
    public class InspectCommand
    {
        [Command(Description = "Inspects the torrent file.")]
        public class File
        {
            [Argument(0, "path", "The path to the torrent file.")]
            [Required(AllowEmptyStrings = false)]
            public string Path { get; set; }

            [Option("-s|--strict", "Enables strict parsing mode.", CommandOptionType.NoValue)]
            public bool Strict { get; set; }

            public int OnExecuteAsync(CommandLineApplication app, IConsole console)
            {
                var cellStroke = new LineThickness(LineWidth.None, LineWidth.None);

                var torrent = ReadAndParseTorrent();

                var document = new Document(
                    new Grid
                    {
                        Stroke = cellStroke,
                        Columns = { UIHelper.FieldsColumns },
                        Children =
                        {
                            UIHelper.Row("Name", torrent.DisplayName),
                            UIHelper.Row("Hash", torrent.OriginalInfoHash),
                            UIHelper.Row("Size", $"{torrent.TotalSize:N0} bytes"),
                            UIHelper.Row("Created at", torrent.CreationDate),
                            UIHelper.Row("Created by", torrent.CreatedBy),
                            UIHelper.Row("Comment", torrent.Comment),
                            UIHelper.Row("Is Private", torrent.IsPrivate),
                            UIHelper.Row("Pieces", torrent.NumberOfPieces),
                            UIHelper.Row("Piece Size", $"{torrent.PieceSize:N0} bytes"),
                            UIHelper.Row("Is Private", torrent.IsPrivate),
                            UIHelper.Row("Magnet", torrent.GetMagnetLink()),
                            UIHelper.Row("File Mode", torrent.FileMode),
                            UIHelper.Row("Files", BuildFileTable(torrent)),
                            UIHelper.Row("Trackers", BuildTrackerList(torrent)),
                            UIHelper.Row("Extra Fields", BuildExtraFields(torrent)),
                        }
                    }).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(document);
                return ExitCodes.Success;
            }

            private Torrent ReadAndParseTorrent()
            {
                var bencodeParser = new BencodeParser();
                var torrentParser = new TorrentParser(bencodeParser,
                    Strict ? TorrentParserMode.Strict : TorrentParserMode.Tolerant);
                using (var stream = System.IO.File.OpenRead(Path))
                {
                    var torrent = torrentParser.Parse(stream);
                    return torrent;
                }
            }
        }

        private static Element BuildFileTable(Torrent torrent)
        {
            if (torrent.Files != null)
            {
                return new Stack(
                    $"Directory: {torrent.Files?.DirectoryName}",
                    new Grid
                    {
                        Stroke = new LineThickness(LineWidth.None, LineWidth.None),
                        Columns =
                        {
                            new Column {Width = GridLength.Star(1)},
                            new Column {Width = GridLength.Auto}
                        },
                        Children =
                        {
                            UIHelper.Header("Path"),
                            UIHelper.Header("Size", TextAlign.Center),
                            torrent.Files.Select(f => new[]
                            {
                                new Cell(f.FullPath),
                                new Cell(f.FileSize.ToString("N0")) { TextAlign = TextAlign.Right },
                            })
                        }
                    });
            }

            if (torrent.File != null)
            {
                return new Grid
                {
                    Stroke = new LineThickness(LineWidth.None, LineWidth.None),
                    Columns =
                    {
                        new Column {Width = GridLength.Star(1)},
                        new Column {Width = GridLength.Auto}
                    },
                    Children =
                    {
                        UIHelper.Header("Name"),
                        UIHelper.Header("Size", TextAlign.Center),
                        new Cell(torrent.File.FileName),
                        new Cell(torrent.File.FileSize.ToString("N0")) { TextAlign = TextAlign.Right },
                    }
                };
            }

            return null;
        }

        private static Element BuildTrackerList(Torrent torrent)
        {
            return new List(torrent.Trackers.Select(t => string.Join("\n", t)));
        }

        private static Element BuildExtraFields(Torrent torrent)
        {
            if (torrent.ExtraFields == null || torrent.ExtraFields.Count == 0)
                return null;

            var grid = FormatBDictionary(torrent.ExtraFields);
            return grid;

            BlockElement FormatBDictionary(BDictionary dict)
            {
                return new Grid()
                {
                    Stroke = new LineThickness(LineWidth.None, LineWidth.None),
                    Columns = { UIHelper.FieldsColumns },
                    Children =
                    {
                        UIHelper.Header("Key"),
                        UIHelper.Header("Value"),
                        dict.Select(f => new []
                        {
                            new Cell(f.Key.ToString()),
                            new Cell(FormatBOject(f.Value))
                        })
                    }
                };
            }

            Element FormatBOject(IBObject value)
            {
                switch (value)
                {
                    case BList bList:
                        return new List(bList.Select(FormatBOject));
                    case BNumber bNumber:
                        return new Span(bNumber.Value.ToString());
                    case BDictionary bDictionary:
                        return FormatBDictionary(bDictionary);
                    case BString bString:
                        return new Span(bString.ToString());
                    default:
                        return null;
                }
            }
        }

        public virtual int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return ExitCodes.WrongUsage;
        }
    }
}
