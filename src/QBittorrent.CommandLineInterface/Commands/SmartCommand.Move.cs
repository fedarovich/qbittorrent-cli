using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jint;
using Jint.Native;
using Jint.Parser;
using Jint.Runtime;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Exceptions;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("move", typeof(Move))]
    public partial class SmartCommand
    {
        [Command(Description = "Moves the torrent to a configured location.")]
        public class Move : TorrentSpecificCommandBase
        {
            [Option("-r|--rule <NAME>", "The name of the rules to analyze. Use * to analyze all the rules.", CommandOptionType.SingleValue)]
            [RegularExpression(@"^([a-zA-Z0-9_]*|\*)$", 
                ErrorMessage = "The rule name can consist only of latin characters, digits and underscore characters.")]
            public string RuleName { get; set; }

            [Option("--what-if", "Display the action but do not actually perform it.", CommandOptionType.NoValue)]
            public bool WhatIf { get; set; }

            [Option("--debug", "Display debug messages.", CommandOptionType.NoValue)]
            public bool Debug { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                try
                {
                    var torrents = await client.GetTorrentListAsync();
                    var torrent = torrents.SingleOrDefault(t =>
                        string.Equals(t.Hash, Hash, StringComparison.InvariantCultureIgnoreCase));
                    if (torrent == null)
                        return ExitCodes.NotFound;

                    int index = 0;
                    var rules = (JArray) SmartService.Instance.GetConfigurationObject("$.move");
                    foreach (var rule in rules.Cast<JObject>())
                    {
                        Log(console, string.Empty);
                        Log(console, $"Processing rule {index++}...", ConsoleColor.White);

                        if (!MatchesRuleName(rule))
                        {
                            Log(console,
                                "Skipped the rule because it has another name: " + rule["rule"]?.Value<string>());
                            continue;
                        }

                        var match = rule["match"]?.Value<string>();
                        Log(console, $"match = {match}");

                        var engine = SmartService.Instance.CreateEngine();
                        engine.SetValue("torrent", torrent);
                        var condition = engine
                            .Execute(rule["condition"].Value<string>())
                            .GetCompletionValue();
                        Log(console, $"condition = {condition}");
                        if (!MatchesCondition(condition, match, engine, torrent))
                        {
                            Log(console, "The condition is not satisfied. The rule was skipped", ConsoleColor.Yellow);
                            continue;
                        }

                        Log(console, "The condition is satisfied.", ConsoleColor.Green);
                        var moveTo = rule["moveTo"];
                        var moveToFolder = moveTo.Type == JTokenType.String
                            ? moveTo.Value<string>()
                            : engine.Execute(moveTo["script"].Value<string>()).GetCompletionValue().AsString();
                        Log(console, $"The file will be moved to \"{moveToFolder}\".", ConsoleColor.Cyan);

                        if (!WhatIf)
                        {
                            await client.SetLocationAsync(Hash, moveToFolder);
                        }

                        return ExitCodes.Success;
                    }

                    Log(console, "No appropriate rule is found.", ConsoleColor.Red);
                    return ExitCodes.RuleNotFound;
                }
                catch (FileNotFoundException ex)
                {
                    console.WriteLineColored(ex.Message, ConsoleColor.Red);
                    return ExitCodes.InvalidConfiguration;
                }
                catch (JsonValidationException ex)
                {
                    PrintJsonValidationError(console, ex);
                    return ExitCodes.InvalidConfiguration;
                }
                catch (ParserException ex)
                {
                    console
                        .WriteLineColored("The JavaScript parsing has failed:", ConsoleColor.Red)
                        .WriteLineColored(ex.Message, ConsoleColor.Red);
                    return ExitCodes.Failure;
                }
                catch (JavaScriptException ex)
                {
                    console
                        .WriteLineColored("The JavaScript exception has occurred:", ConsoleColor.Red)
                        .WriteLineColored(ex.Error.ToString(), ConsoleColor.Red);
                    return ExitCodes.Failure;
                }

                bool MatchesRuleName(JObject obj)
                {
                    if (RuleName == "*")
                        return true;
                    var name = obj["rule"];
                    if (string.IsNullOrEmpty(name?.Value<string>()))
                        return string.IsNullOrEmpty(RuleName);
                    return name.Value<string>() == RuleName;
                }
            }

            [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
            private bool MatchesCondition(JsValue condition, string match, Engine engine, TorrentInfo torrent)
            {
                switch (condition.Type)
                {
                    case Types.None:
                    case Types.Undefined:
                    case Types.Null:
                        return false;
                    case Types.Boolean:
                        return condition.AsBoolean();
                    case Types.String:
                        return GetMatchValue() == condition.AsString();
                    case Types.Number:
                        var number = condition.AsNumber();
                        return number != 0 && double.IsNaN(number);
                    case Types.Object when (condition.IsRegExp()):
                        var regExp = condition.AsRegExp();
                        var regExpMatch = regExp.Match(GetMatchValue(), 0);
                        engine.SetValue("match", regExpMatch);
                        return regExpMatch.Success;
                    case Types.Object:
                        return false;
                }

                return false;

                string GetMatchValue()
                {
                    switch (match)
                    {
                        case "category":
                            return torrent.Category;
                        case "state":
                            return torrent.State.ToString();
                        case "name":
                        default:
                            return torrent.Name;
                    }
                }
            }

            private void Log(IConsole console, string message, ConsoleColor? color = null)
            {
                if (!Debug)
                    return;

                console.WriteLineColored(message, color);
            }
        }
    }
}
