using McMaster.Extensions.CommandLineUtils.Conventions;

namespace QBittorrent.CommandLineInterface.Conventions
{
    public static class ConventionBuilderExtensions
    {
        public static IConventionBuilder UsePagerForHelpText(this IConventionBuilder builder, bool usePager = true)
        {
            builder.AddConvention(new UsePagerForHelpTextConvention(usePager));
            return builder;
        }

        public static IConventionBuilder MakeSuggestionsInErrorMessage(this IConventionBuilder builder,
            bool makeSuggestions = true)
        {
            builder.AddConvention(new MakeSuggestionsInErrorMessageConvention(makeSuggestions));
            return builder;
        }
    }
}
