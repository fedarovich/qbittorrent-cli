using System;

namespace QBittorrent.CommandLineInterface.Services
{
    public abstract class EncryptionService
    {
        public static EncryptionService Instance { get; }

        static EncryptionService()
        {
#if NET6_0_OR_GREATER
            Instance = OperatingSystem.IsWindows() 
                ? (EncryptionService) new WindowsEncryptionService()
                : new UnixEncryptionService();
#else
            Instance = Environment.OSVersion.Platform == PlatformID.Win32NT 
                ? (EncryptionService) new WindowsEncryptionService()
                : new UnixEncryptionService();
#endif

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
