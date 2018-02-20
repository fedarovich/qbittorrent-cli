using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.CommandLineInterface
{
    public static class ExitCodes
    {
        public const int Success = 0;

        public const int Failure = 1;

        public const int WrongUsage = 2;

        public const int NotFound = 3;

        public const int Cancelled = 4;
    }
}
