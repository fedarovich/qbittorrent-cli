using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace QBittorrent.CommandLineInterface.Services
{
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    internal class WindowsEncryptionService : EncryptionService
    {
        private const int EntropyLength = 16;

        public override string Encrypt(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var entropy = new byte[EntropyLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(entropy);
            }

            var outputBytes = ProtectedData.Protect(inputBytes, entropy, DataProtectionScope.CurrentUser);
            var resultBytes = new byte[entropy.Length + outputBytes.Length];
            Buffer.BlockCopy(entropy, 0, resultBytes, 0, entropy.Length);
            Buffer.BlockCopy(outputBytes, 0, resultBytes, entropy.Length, outputBytes.Length);
            return Convert.ToBase64String(resultBytes);
        }

        public override string Decrypt(string input)
        {
            var inputBytes = Convert.FromBase64String(input);
            var entropy = new byte[EntropyLength];
            Buffer.BlockCopy(inputBytes, 0, entropy, 0, EntropyLength);
            var dataBytes = new byte[inputBytes.Length - EntropyLength];
            Buffer.BlockCopy(inputBytes, EntropyLength, dataBytes, 0, dataBytes.Length);
            var outBytes = ProtectedData.Unprotect(dataBytes, entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(outBytes);
        }
    }
}
