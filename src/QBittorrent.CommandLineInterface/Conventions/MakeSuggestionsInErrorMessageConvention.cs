using McMaster.Extensions.CommandLineUtils.Conventions;

namespace QBittorrent.CommandLineInterface.Conventions
{
    public class MakeSuggestionsInErrorMessageConvention : IConvention
    {
        public MakeSuggestionsInErrorMessageConvention(bool makeSuggestions)
        {
            MakeSuggestions = makeSuggestions;
        }

        public bool MakeSuggestions { get; }

        public void Apply(ConventionContext context)
        {
            context.Application.MakeSuggestionsInErrorMessage = MakeSuggestions;
        }
    }
}
