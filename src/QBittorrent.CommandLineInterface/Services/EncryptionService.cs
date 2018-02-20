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
            Instance = Environment.OSVersion.Platform == PlatformID.Win32NT 
                ? (EncryptionService) new WindowsEncryptionService()
                : new UnixEncryptionService();
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
