using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.CommandLineInterface.Services
{
    public abstract class EncryptionService
    {
        public static EncryptionService Instance { get; }

        static EncryptionService()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Instance = new WindowsEncryptionService();
            }
        }

        private protected EncryptionService()
        {
        }

        public abstract string Encrypt(string input);

        public abstract string Decrypt(string input);

        public virtual void ResetKey()
        {
        }
    }
}
