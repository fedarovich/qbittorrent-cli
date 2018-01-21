using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.Client
{
    public struct Range : IFormattable
    {
        public Range(long startIndex, long endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public long StartIndex { get; }

        public long EndIndex { get; }

        public override string ToString()
        {
            return $"[{StartIndex}; {EndIndex}]";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var formatString = $"[{{0:{format}}}; {{1:{format}}}]";
            return string.Format(formatProvider, formatString, StartIndex, EndIndex);
        }
    }
}
