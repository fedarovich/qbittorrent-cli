using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Unix;

namespace QBittorrent.CommandLineInterface.Services
{
    internal class UnixEncryptionService : EncryptionService
    {
        public override string Encrypt(string input)
        {
            var key = EnsureKeyFile();
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var memoryStream = new MemoryStream();
                memoryStream.Write(aes.IV, 0, aes.IV.Length);
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var inputBytes = Encoding.UTF8.GetBytes(input);
                    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                }

                var outputBytes = memoryStream.ToArray();
                return Convert.ToBase64String(outputBytes);
            }
        }

        public override string Decrypt(string input)
        {
            var key = EnsureKeyFile();
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var inputStream = new MemoryStream(Convert.FromBase64String(input));
                inputStream.Read(aes.IV, 0, aes.IV.Length);
                using (var outputStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputStream);
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
        }

        public override void ResetKey()
        {
            var directory = GetKeyFileDir();
            if (!directory.Exists)
            {
                directory.Create(FileAccessPermissions.UserExecute | FileAccessPermissions.UserWrite);
            }
            else
            {
                directory.FileAccessPermissions = FileAccessPermissions.UserExecute | FileAccessPermissions.UserWrite;
            }
            directory.FileSpecialAttributes = FileSpecialAttributes.Sticky;
            
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                var file = GetKeyFile();
                using (var stream = file.OpenWrite())
                {
                    stream.SetLength(0);
                    stream.FileAccessPermissions = FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite;
                    stream.Write(aes.Key, 0, aes.Key.Length);
                }
            }
        }

        private UnixDirectoryInfo GetKeyFileDir()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(appData, ".qbt", ".private");
            return new UnixDirectoryInfo(path);
        }

        private UnixFileInfo GetKeyFile()
        {
            return new UnixFileInfo(Path.Combine(GetKeyFileDir().FullName, ".key"));
        }

        private byte[] EnsureKeyFile()
        {
            var file = GetKeyFile();
            if (!file.Exists)
            {
                ResetKey();
            }

            using (var stream = file.OpenRead())
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }
    }
}
