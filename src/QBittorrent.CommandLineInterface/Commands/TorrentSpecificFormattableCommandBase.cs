using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Formats;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class TorrentSpecificFormattableCommandBase<T> : TorrentSpecificCommandBase
    {
        private readonly ObjectFormatter<T> _formatter;
        private readonly Lazy<IEnumerable<(string name, PropertyInfo prop)>> _props;

        protected TorrentSpecificFormattableCommandBase()
        {
            _props = ObjectFormatter<T>.GetCommandPropertyMappings(GetType());
            _formatter = new ObjectFormatter<T>(PrintList, FindProperty);
        }

        protected virtual PropertyInfo FindProperty(string name) => _props.Value.FirstOrDefault(t => t.name == name).prop;

        protected virtual IReadOnlyDictionary<string, Func<object, object>> CustomFormatters => null;

        protected virtual void PrintList(T data) => UIHelper.PrintObject(data, CustomFormatters);

        protected void Print(in T obj) => _formatter.PrintFormat(obj, Format);

        [Option("-F|--format <OBJECT_FORMAT>", "Output format: list|csv|json|property", CommandOptionType.SingleValue)]
        public string Format { get; set; }
    }
}
