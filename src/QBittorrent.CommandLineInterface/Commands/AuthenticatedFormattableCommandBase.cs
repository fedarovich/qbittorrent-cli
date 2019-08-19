using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Formats;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class AuthenticatedFormattableCommandBase<T> : AuthenticatedCommandBase
    {
        private readonly ObjectFormatter<T> _formatter;
        private readonly Lazy<IEnumerable<(string name, PropertyInfo prop)>> _props;

        protected AuthenticatedFormattableCommandBase()
        {
            _props = new Lazy<IEnumerable<(string name, PropertyInfo prop)>>(() =>
                from commandProp in GetType().GetTypeInfo().DeclaredProperties
                let option = commandProp.GetCustomAttribute<OptionAttribute>()
                where option != null
                join prop in typeof(T).GetProperties() on commandProp.Name equals prop.Name
                select (new CommandOption(option.Template, option.OptionType.GetValueOrDefault()).LongName, prop));
            _formatter = new ObjectFormatter<T>(data => UIHelper.PrintObject(data, CustomFormatters), FindProperty);
        }

        protected virtual PropertyInfo FindProperty(string name) => _props.Value.FirstOrDefault(t => t.name == name).prop;

        protected Dictionary<string, Func<object, object>> CustomFormatters { get; } = new Dictionary<string, Func<object, object>>();

        protected void Print(in T obj) => _formatter.PrintFormat(obj, Format);

        [Option("-F|--format <OBJECT_FORMAT>", "Output format: list|csv|json|property", CommandOptionType.SingleValue)]
        public string Format { get; set; }
    }
}
