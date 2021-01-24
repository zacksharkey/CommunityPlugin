using System;
using System.Linq;
using System.Security.Cryptography;

namespace CommunityPlugin.Objects.Helpers
{
    public class RNDGenerator
    {
        private const int DefaultLength = 16;
        private RandomNumberGenerator _generator;

        protected virtual int Length
        {
            get
            {
                return 16;
            }
        }

        public RNDGenerator()
          : this(RandomNumberGenerator.Create())
        {
        }

        protected RNDGenerator(RandomNumberGenerator generator)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            this._generator = generator;
        }

        public short GetInt16()
        {
            return BitConverter.ToInt16(this.GetBytes(4), 0);
        }

        public int GetInt32()
        {
            return BitConverter.ToInt32(this.GetBytes(4), 0);
        }

        public long GetInt64()
        {
            return BitConverter.ToInt64(this.GetBytes(8), 0);
        }

        public Guid GetGuid()
        {
            return new Guid(this.GetBytes(16));
        }

        public byte[] GetBytes()
        {
            return this.GetBytes(this.Length);
        }

        public virtual byte[] GetBytes(int length)
        {
            byte[] data = new byte[length];
            this._generator.GetBytes(data);
            return data;
        }

        public byte[] GetNonZeroBytes()
        {
            return this.GetNonZeroBytes(this.Length);
        }

        public virtual byte[] GetNonZeroBytes(int length)
        {
            byte[] data = new byte[length];
            this._generator.GetNonZeroBytes(data);
            return data;
        }

        public string GetString()
        {
            return this.GetString(this.Length);
        }

        public string GetString(int length)
        {
            return this.BytesToHex(this.GetBytes(length));
        }

        public string GetNonZeroString()
        {
            return this.GetNonZeroString(this.Length);
        }

        public string GetNonZeroString(int length)
        {
            return this.BytesToHex(this.GetNonZeroBytes(length));
        }

        private string BytesToHex(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            return string.Join(string.Empty, Enumerable.Range(0, bytes.Length).Select<int, string>((Func<int, string>)(i => bytes[i].ToString("X2"))).ToArray<string>());
        }
    }
}
