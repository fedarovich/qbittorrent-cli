using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mono.Unix;
using Mono.Unix.Native;

namespace QBittorrent.CommandLineInterface.Services
{
    internal class UnixEncryptionService : EncryptionService
    {
        private static readonly byte[] Key =
        {
            237, 158, 211, 168, 18, 187, 41, 93, 36, 150, 14, 142, 137, 9, 29, 108, 194, 174, 191, 28, 5, 9, 127, 78,
            84, 84, 6, 255, 195, 246, 124, 89, 89, 249, 104, 253, 177, 52, 111, 43, 223, 152, 114, 122, 79, 211, 28, 67,
            76, 148, 161, 180, 39, 202, 153, 67, 1, 155, 183, 106, 247, 64, 220, 140
        };

        public override string Encrypt(string input)
        {
            var key = EnsureKeyFile();
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Padding = PaddingMode.PKCS7;
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
                var inputStream = new MemoryStream(Convert.FromBase64String(input));
                var iv = new byte[aes.IV.Length];
                inputStream.Read(iv, 0, iv.Length);

                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

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
            CreateDirectory(directory);
            directory.Refresh();
            directory.FileAccessPermissions = FileAccessPermissions.UserExecute | FileAccessPermissions.UserWrite;

            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                var file = GetKeyFile();
                using (var stream = file.Open(FileMode.Create, FileAccess.Write, FilePermissions.S_IRUSR | FilePermissions.S_IWUSR))
                {
                    stream.FileAccessPermissions = FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite;
                    stream.Write(aes.Key, 0, aes.Key.Length);
                }
            }

            void CreateDirectory(UnixDirectoryInfo dir)
            {
                if (dir.Exists)
                    return;

                CreateDirectory(dir.Parent);
                dir.Create();
            }
        }

        private UnixDirectoryInfo GetKeyFileDir()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(appData, ".qbt", ".private");
            return new UnixDirectoryInfo(path);
        }

        private UnixFileInfo GetKeyFile()
        {
            var hmac = new HMACSHA256(Key);
            var bytes = Encoding.UTF8.GetBytes(UnixEnvironment.UserName);
            var hash = hmac.ComputeHash(bytes);
            var filename = "." + BitConverter.ToString(hash).Replace("-", "");
            return new UnixFileInfo(Path.Combine(GetKeyFileDir().FullName, filename));
        }

        private byte[] EnsureKeyFile()
        {
            var file = GetKeyFile();
            if (!file.Exists)
            {
                ResetKey();
            }

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FilePermissions.S_IRUSR))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }
    }
}
