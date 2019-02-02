using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ViewModels;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Rule))]
    public partial class RssCommand
    {
        [Command(Description = "Manages RSS automatic download rules.", ExtendedHelpText = ExperimentalHelpText)]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(Set))]
        [Subcommand(typeof(Rename))]
        [Subcommand(typeof(Delete))]
        [Subcommand(typeof(List))]
        public class Rule : ClientRootCommandBase
        {
            public abstract class AddSetCommandBase : AuthenticatedCommandBase
            {
                [Argument(0, "<NAME>", "Rule name")]
                [Required]
                public string Name { get; set; }

                [Option("-i|--contains <STRING>", "The substring that the torrent name must contain.",
                    CommandOptionType.SingleValue)]
                public string MustContain { get; set; }

                [Option("-x|--not-contains <STRING>", "The substring that the torrent name must not contain",
                    CommandOptionType.SingleValue)]
                public string MustNotContain { get; set; }

                [Option("-f|--episode-filter", "Episode filter definition, e.g. \"1x01-;\".",
                    CommandOptionType.SingleValue)]
                public string EpisodeFilter { get; set; }

                [Option("-E|--prev-matched-episode <ID>",
                    "The episode ID already matched by smart filter. Can be specified multiple times.",
                    CommandOptionType.MultipleValue)]
                public string[] PreviouslyMatchedEpisodes { get; set; }

                [Option("-u|--feed-url <URL>",
                    "The feed URL the rule applied to. Can be specified multiple times.",
                    CommandOptionType.MultipleValue)]
                public Uri[] AffectedFeeds { get; set; }

                [Option("-c|--category <CATEGORY>", "Assign category to the torrent.", CommandOptionType.SingleValue)]
                public string Category { get; set; }

                [Option("-s|--save-path <PATH>")] public string SavePath { get; set; }

                protected bool? ConvertPauseState(RssRulePauseState? state)
                {
                    switch (state)
                    {
                        case null:
                        case RssRulePauseState.Auto:
                            return null;
                        case RssRulePauseState.False:
                            return false;
                        case RssRulePauseState.True:
                            return true;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                }
            }

            [Command(Description = "Adds the RSS automatic download rule.", ExtendedHelpText = ExperimentalHelpText)]
            public class Add : AddSetCommandBase
            {
                [Option("-D|--disabled", "Add the rule as disabled.", CommandOptionType.NoValue)]
                public bool Disabled { get; set; }

                [Option("-r|--regex", "Enable regex mode in \"--contains\" and \"--not-contains\" options.",
                    CommandOptionType.NoValue)]
                public bool UseRegex { get; set; }

                [Option("-F|--smart-filter", "Enable smart episode filter.", CommandOptionType.NoValue)]
                public bool SmartFilter { get; set; }

                [Option("-d|--ignore-days <DAYS>", "Ignore subsequent rule matches.", CommandOptionType.SingleValue)]
                public int IgnoreDays { get; set; }

                [Option("-l|--last-match <DATE_TIME>", "The rule last match time.", CommandOptionType.SingleValue)]
                public DateTimeOffset? LastMatch { get; set; }

                [Option("-p|--paused <VALUE>", "Add matched torrent in pause mode: (Auto|True|False).",
                    CommandOptionType.SingleValue)]
                public RssRulePauseState? Paused { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client,
                    CommandLineApplication app, IConsole console)
                {
                    var rules = await client.GetRssAutoDownloadingRulesAsync();
                    if (rules.ContainsKey(Name))
                        throw new Exception($"The rule with the name \"{Name}\" already exists.");

                    var rule = new RssAutoDownloadingRule
                    {
                        Enabled = !Disabled,
                        MustContain = MustContain ?? string.Empty,
                        MustNotContain = MustNotContain ?? string.Empty,
                        UseRegex = UseRegex,
                        EpisodeFilter = EpisodeFilter ?? string.Empty,
                        SmartFilter = SmartFilter,
                        PreviouslyMatchedEpisodes = PreviouslyMatchedEpisodes,
                        AffectedFeeds = AffectedFeeds,
                        IgnoreDays = IgnoreDays,
                        LastMatch = LastMatch,
                        AddPaused = ConvertPauseState(Paused),
                        AssignedCategory = Category ?? string.Empty,
                        SavePath = SavePath ?? string.Empty
                    };

                    await client.SetRssAutoDownloadingRuleAsync(Name, rule);
                    return ExitCodes.Success;
                }
            }

            [Command("set", "update", Description = "Changes the RSS automatic download rule.",
                ExtendedHelpText = ExperimentalHelpText)]
            public class Set : AddSetCommandBase
            {
                [Option("-D|--disabled <BOOL>", "Disable/enable the rule.", CommandOptionType.SingleValue)]
                public bool? Disabled { get; set; }

                [Option("-r|--regex <BOOL>",
                    "Enable/disable regex mode in \"--contains\" and \"--not-contains\" options.",
                    CommandOptionType.SingleValue)]
                public bool? UseRegex { get; set; }

                [Option("-F|--smart-filter", "Enable/disable smart episode filter.", CommandOptionType.SingleValue)]
                public bool SmartFilter { get; set; }

                [Option("-d|--ignore-days <DAYS>", "Ignore subsequent rule matches.", CommandOptionType.SingleValue)]
                public int? IgnoreDays { get; set; }

                [Option("-l|--last-match <DATE_TIME>", "The rule last match time.", CommandOptionType.SingleValue)]
                public DateTimeOffset? LastMatch { get; set; }

                [Option("-p|--paused <VALUE>", "Add matched torrent in pause mode: (Auto|True|False).",
                    CommandOptionType.SingleValue)]
                public RssRulePauseState? Paused { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client,
                    CommandLineApplication app, IConsole console)
                {
                    var rules = await client.GetRssAutoDownloadingRulesAsync();
                    if (!rules.TryGetValue(Name, out var rule))
                        throw new Exception($"The rule with the name \"{Name}\" does not exist.");

                    Set(nameof(rule.Enabled), !Disabled);
                    Set(nameof(rule.MustContain), MustContain);
                    Set(nameof(rule.MustNotContain), MustNotContain);
                    Set(nameof(rule.UseRegex), UseRegex);
                    Set(nameof(rule.EpisodeFilter), EpisodeFilter);
                    Set(nameof(rule.IgnoreDays), IgnoreDays);
                    Set(nameof(rule.LastMatch), LastMatch);
                    Set(nameof(rule.AssignedCategory), Category);
                    Set(nameof(rule.SavePath), SavePath);

                    if (Paused != null)
                    {
                        Set(nameof(rule.AddPaused), ConvertPauseState(Paused));
                    }

                    if (PreviouslyMatchedEpisodes != null)
                    {
                        rule.PreviouslyMatchedEpisodes = PreviouslyMatchedEpisodes
                            .Where(e => !string.IsNullOrWhiteSpace(e))
                            .ToList();
                    }

                    if (AffectedFeeds != null)
                    {
                        rule.AffectedFeeds = AffectedFeeds.Where(e => e.IsAbsoluteUri).ToList();
                    }

                    await client.SetRssAutoDownloadingRuleAsync(Name, rule);
                    return ExitCodes.Success;

                    void Set<T>(string propertyName, T value)
                    {
                        if (value != null)
                        {
                            rule.GetType().GetProperty(propertyName).SetValue(rule, value);
                        }
                    }
                }
            }
            
            [Command(Description = "Renames RSS automatic downloading rule.", ExtendedHelpText = ExperimentalHelpText)]
            public class Rename : AuthenticatedCommandBase
            {
                [Argument(0, "<NAME>", "Rule name")]
                [Required]
                public string Name { get; set; }

                [Argument(1, "<NEW_NAME>", "New rule name")]
                [Required]
                public string NewName { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.RenameRssAutoDownloadingRuleAsync(Name, NewName);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Deletes RSS automatic downloading rule.", ExtendedHelpText = ExperimentalHelpText)]
            public class Delete : AuthenticatedCommandBase
            {
                [Argument(0, "<NAME>", "Rule name")]
                [Required]
                public string Name { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.DeleteRssAutoDownloadingRuleAsync(Name);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Shows RSS automatic downloading rule.", ExtendedHelpText = ExperimentalHelpText)]
            public class List : AuthenticatedCommandBase
            {
                private static readonly Dictionary<string, Func<object, object>> CustomFormatters;

                static List()
                {
                    CustomFormatters = new Dictionary<string, Func<object, object>>
                    {
                        [nameof(RssRuleViewModel.AffectedFeeds)] = FormatAffectedFeeds,
                        [nameof(RssRuleViewModel.PreviouslyMatchedEpisodes)] = FormatPreviouslyMatchedEpisodes,
                    };
                }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var rules = await client.GetRssAutoDownloadingRulesAsync();
                    foreach (var (name, rule) in rules)
                    {
                        var vm = new RssRuleViewModel(name, rule);
                        UIHelper.PrintObject(vm, CustomFormatters);
                        console.WriteLine();
                    }

                    return ExitCodes.Success;
                }

                private static object FormatAffectedFeeds(object obj)
                {
                    if (!(obj is IReadOnlyList<Uri> feeds))
                        return null;

                    return new Alba.CsConsoleFormat.List(feeds.Where(f => f != null).Select(f => f.AbsoluteUri))
                    {
                        IndexFormat = string.Empty
                    };
                }

                private static object FormatPreviouslyMatchedEpisodes(object obj)
                {
                    if (!(obj is IReadOnlyList<string> episodes))
                       return null;

                    return new Alba.CsConsoleFormat.List(episodes.Where(e => !string.IsNullOrEmpty(e)));
                }
            }
        }
    }
}
