using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface
{
    /// <summary>
    /// Process access to a console pager, which supports scrolling and search.
    /// </summary>
    public class Pager : IDisposable
    {
        private string _prompt = "Use arrow keys to scroll\\. Press 'q' to exit\\.";
        private readonly Lazy<Process> _less;
        private readonly TextWriter _fallbackWriter;
        private bool _disposed;

        public Pager()
          : this(PhysicalConsole.Singleton)
        {
        }

        public Pager(IConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(nameof(console));
            Enabled = !console.IsOutputRedirected && PagerExists();
            _less = new Lazy<Process>(CreateWriter);
            _fallbackWriter = console.Out;

            bool PagerExists() => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                                  || File.Exists(GetPagerPath());
        }

        public bool Enabled { get; private set; }

        /// <summary>
        /// The prompt to display at the bottom of the pager.
        /// <seealso href="https://www.computerhope.com/unix/uless.htm#Prompts" /> for details.
        /// </summary>
        public string Prompt
        {
            get => _prompt;
            set
            {
                if (_less.IsValueCreated)
                    throw new InvalidOperationException("Cannot set the prompt on the pager after the pager has begun");
                _prompt = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets an object which can be used to write text into the pager.
        /// </para>
        /// <para>
        /// This fallback to <see cref="P:McMaster.Extensions.CommandLineUtils.IConsole.Out" /> if the pager is not available.
        /// </para>
        /// </summary>
        public TextWriter Writer
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(Pager));
                return _less.Value?.StandardInput ?? _fallbackWriter;
            }
        }

        /// <summary>This will wait until the user exits the pager.</summary>
        public void WaitForExit()
        {
            Dispose();
        }

        /// <summary>Force close the pager.</summary>
        public void Kill()
        {
            if (!_less.IsValueCreated)
                return;
            _less.Value.Kill();
        }

        private Process CreateWriter()
        {
            if (!Enabled)
                return null;
            List<string> stringList = new List<string>
            {
                "-K",
                "--prompt=" + Prompt
            };
            Process process = new Process
            {
                StartInfo = {
                    FileName = GetPagerPath(),
                    Arguments = ArgumentEscaper.EscapeAndConcatenate(stringList),
                    RedirectStandardInput = true
                }
            };
            try
            {
                process.Start();
                return process;
            }
            catch (Exception ex)
            {
                if (DotNetCliContext.IsGlobalVerbose())
                    Console.Error.WriteLine("debug: Failed to start pager: " + ex);
                Enabled = false;
                return null;
            }
        }

        /// <summary>This will wait until the user exits the pager.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            if (!_less.IsValueCreated)
                return;
            Process process = _less.Value;
            if (process == null)
                return;
            process.StandardInput.Dispose();
            process.WaitForExit();
            process.Dispose();
        }

        private string GetPagerPath() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(GetStartupPath(), "utils", "less", "less.exe")
            : "less";

        private string GetStartupPath() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    }
}
