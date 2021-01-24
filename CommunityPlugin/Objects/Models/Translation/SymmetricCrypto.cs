
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Models.Algorythm;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class SymmetricCrypto
    {
        public static readonly byte[] DefaultVI = Encoding.Default.GetBytes("EM123456");

        public Encoding Encoding { get; set; }

        public object Algorithm { get; private set; }

        private SymmetricCrypto()
        {
            this.Encoding = Encoding.Default;
        }

        public static SymmetricCrypto Create(byte[] key)
        {
            return SymmetricCrypto.Create(key, SymmetricAlgorithmType.TripleDES);
        }

        public static SymmetricCrypto Create(
          byte[] key,
          SymmetricAlgorithmType algorithmType)
        {
            return SymmetricCrypto.Create(SymmetricCrypto.DefaultVI, key, algorithmType);
        }

        public static SymmetricCrypto Create(
          byte[] vi,
          byte[] key,
          SymmetricAlgorithmType algorithmType)
        {
            SymmetricCrypto symmetricCrypto = new SymmetricCrypto();
            symmetricCrypto.Algorithm = symmetricCrypto.CreateAlgorithm(algorithmType);
            if (symmetricCrypto.Algorithm is SymmetricAlgorithm)
            {
                (symmetricCrypto.Algorithm as SymmetricAlgorithm).IV = SymmetricCrypto.processVI(vi, algorithmType);
                (symmetricCrypto.Algorithm as SymmetricAlgorithm).Key = SymmetricCrypto.processKey(key, algorithmType);
            }
            else if (symmetricCrypto.Algorithm is CSymmetricAlgorithm)
            {
                (symmetricCrypto.Algorithm as CSymmetricAlgorithm).IV = SymmetricCrypto.processVI(vi, algorithmType);
                (symmetricCrypto.Algorithm as CSymmetricAlgorithm).Key = SymmetricCrypto.processKey(key, algorithmType);
            }
            return symmetricCrypto;
        }

        protected object CreateAlgorithm(SymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.AES:
                    return (object)new AesManaged();
                case SymmetricAlgorithmType.DES:
                    return (object)new DESCryptoServiceProvider();
                case SymmetricAlgorithmType.TripleDES:
                    return (object)new TripleDESCryptoServiceProvider();
                case SymmetricAlgorithmType.RC2:
                    return (object)new RC2CryptoServiceProvider();
                case SymmetricAlgorithmType.Rijndael:
                    return (object)new RijndaelManaged();
                case SymmetricAlgorithmType.BlowFish:
                    return (object)new BlowFish();
                case SymmetricAlgorithmType.XTea:
                    return (object)new XTea();
                default:
                    throw new Exception(string.Empty);
            }
        }

        private static byte[] processVI(byte[] vi, SymmetricAlgorithmType algorithmType)
        {
            int count;
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.AES:
                case SymmetricAlgorithmType.Rijndael:
                    count = 16;
                    break;
                default:
                    count = 8;
                    break;
            }
            return vi.Length == count ? vi : Enumerable.Range(0, count).Select<int, byte>((Func<int, byte>)(i => i >= vi.Length ? Convert.ToByte(i) : vi[i])).ToArray<byte>();
        }

        private static byte[] processKey(byte[] key, SymmetricAlgorithmType algorithmType)
        {
            int count;
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.AES:
                    count = 32;
                    break;
                case SymmetricAlgorithmType.DES:
                case SymmetricAlgorithmType.RC2:
                    count = 8;
                    break;
                case SymmetricAlgorithmType.TripleDES:
                    count = 24;
                    break;
                case SymmetricAlgorithmType.Rijndael:
                    count = key.Length >= 16 ? (key.Length >= 24 ? 32 : 24) : 16;
                    break;
                case SymmetricAlgorithmType.XTea:
                case SymmetricAlgorithmType.RC6:
                    count = 16;
                    break;
                default:
                    count = key.Length >= 8 ? (key.Length >= 16 ? (key.Length >= 24 ? (key.Length >= 32 ? (key.Length >= 40 ? (key.Length >= 64 ? (key.Length >= 128 ? (key.Length >= 192 ? (key.Length >= 256 ? (key.Length >= 448 ? 512 : 448) : 256) : 192) : 128) : 64) : 40) : 32) : 24) : 16) : 8;
                    break;
            }
            return key.Length == count ? key : Enumerable.Range(0, count).Select<int, byte>((Func<int, byte>)(i => i >= key.Length ? Convert.ToByte(i) : key[i])).ToArray<byte>();
        }

        public string Decrypt(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return string.IsNullOrWhiteSpace(text) ? string.Empty : this.Encoding.GetString(this.Decrypt(Convert.FromBase64String(text)));
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return this.ConvertToBytes(buffer, true);
        }

        public string Encrypt(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return string.IsNullOrWhiteSpace(text) ? string.Empty : Convert.ToBase64String(this.Encrypt(this.Encoding.GetBytes(text)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return this.ConvertToBytes(buffer, false);
        }

        private byte[] ConvertToBytes(byte[] buffer, bool decryptedOrEncrypted)
        {
            if (this.Algorithm is SymmetricAlgorithm)
            {
                SymmetricAlgorithm algorithm = this.Algorithm as SymmetricAlgorithm;
                ICryptoTransform transform = decryptedOrEncrypted ? algorithm.CreateDecryptor() : algorithm.CreateEncryptor();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, transform, CryptoStreamMode.Write);
                try
                {
                    cryptoStream.Write(buffer, 0, buffer.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
                finally
                {
                    cryptoStream?.Close();
                    GC.Collect();
                }
            }
            else
            {
                if (!(this.Algorithm is CSymmetricAlgorithm))
                    return buffer;
                CSymmetricAlgorithm algorithm = this.Algorithm as CSymmetricAlgorithm;
                MemoryStream memoryStream1 = new MemoryStream(buffer);
                if (memoryStream1.CanSeek)
                    memoryStream1.Seek(0L, SeekOrigin.Begin);
                MemoryStream memoryStream2 = new MemoryStream();
                if (decryptedOrEncrypted)
                    algorithm.Decrypt((Stream)memoryStream1, (Stream)memoryStream2, algorithm.Key);
                else
                    algorithm.Encrypt((Stream)memoryStream1, (Stream)memoryStream2, algorithm.Key);
                return memoryStream2.ToArray();
            }
        }
    }
}
