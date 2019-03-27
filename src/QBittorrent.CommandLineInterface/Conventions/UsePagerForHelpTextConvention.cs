using McMaster.Extensions.CommandLineUtils.Conventions;

namespace QBittorrent.CommandLineInterface.Conventions
{
    public class UsePagerForHelpTextConvention : IConvention
    {
        public UsePagerForHelpTextConvention(bool usePager)
        {
            UsePager = usePager;
        }

        public bool UsePager { get; }

        public void Apply(ConventionContext context)
        {
            context.Application.UsePagerForHelpText = UsePager;
        }
    }
}
