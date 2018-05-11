using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuSpecGen
{
    partial class NuSpecTemplate
    {
        public NuSpecTemplate(string packageVersion)
        {
            if (string.IsNullOrWhiteSpace(packageVersion))
                throw new ArgumentException("The package version cannot be a non-empty string.", nameof(packageVersion));

            PackageVersion = packageVersion;
        }

        public string PackageVersion { get; }
    }
}
