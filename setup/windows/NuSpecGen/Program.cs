using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace NuSpecGen
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        [Option("-v|--version <VERSION>")]
        [Required]
        public string Version { get; set; }

        [Option("-o|--output <PATH>")]
        [Required]
        [LegalFilePath]
        public string OutputPath { get; set; }

        public int OnExecute()
        {
            var template = new NuSpecTemplate(Version);
            using (var writer = new StreamWriter(OutputPath, false, Encoding.UTF8))
            {
                writer.Write(template);
            }
            return 0;
        }
    }
}
