using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Formats;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class ListCommandBase<T> : AuthenticatedCommandBase
    {
        private readonly ListFormatter<T> _formatter;

        protected ListCommandBase()
        {
            _formatter = new ListFormatter<T>(PrintTable, PrintList);
        }

        [Option("-F|--format <LIST_FORMAT>", "Output format: table|list|csv|json", CommandOptionType.SingleValue)]
        public virtual string Format { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Print(IEnumerable<T> data, bool preferList = false) => _formatter.PrintFormat(data, Format, preferList);
       
        protected virtual void PrintTable(IEnumerable<T> list)
        {
            throw new Exception("Unsupported output format.");
        }

        protected virtual void PrintList(IEnumerable<T> list)
        {
            UIHelper.PrintList(list, ListCustomFormatters);
        }

        protected virtual IReadOnlyDictionary<string, Func<object, object>> ListCustomFormatters => null;
    }
}
