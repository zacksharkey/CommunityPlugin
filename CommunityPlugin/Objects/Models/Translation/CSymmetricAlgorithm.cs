using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommunityPlugin.Objects.Models.Translation
{
    public abstract class CSymmetricAlgorithm
    {
        protected int DefaultBlockSizeValue = 32;
        protected byte[] pIVValue;
        protected byte[] pKeyValue;
        protected int pKeySizeValue;
        protected KeySizes[] pLegalKeySizesValue;
        protected int pBlockSizeValue;

        public CipherMode CipherMode { get; set; }

        public Encoding Encoding { get; set; }

        public byte[] IV
        {
            get
            {
                if (this.pIVValue == null)
                    this.GenerateIV();
                return this.pIVValue;
            }
            set
            {
                this.pIVValue = value;
            }
        }

        public virtual byte[] Key
        {
            get
            {
                return this.pKeyValue;
            }
            set
            {
                this.CheckForKey(value);
                this.CheckForKeyLength(value);
                this.pKeyValue = value;
                this.SetKey(this.pKeyValue);
            }
        }

        protected CSymmetricAlgorithm()
        {
            this.Encoding = Encoding.Default;
            this.CipherMode = CipherMode.CBC;
            this.pLegalKeySizesValue = new KeySizes[0];
            this.pBlockSizeValue = this.DefaultBlockSizeValue;
        }

        protected virtual void CheckForKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length == 0)
                throw new WeakedKeyException("Zero Key Length.");
        }

        protected virtual void CheckForKeyLength(byte[] key)
        {
            int num = key.Length * 8;
            for (int index = 0; index < this.pLegalKeySizesValue.Length; ++index)
            {
                KeySizes keySizes = this.pLegalKeySizesValue[index];
                if (num >= keySizes.MinSize && num <= keySizes.MaxSize && num != keySizes.SkipSize)
                    return;
            }
            throw new Exception("Invalid Key Length.");
        }

        protected virtual void SetKey(byte[] key)
        {
        }

        public virtual void Encrypt(Stream inStream, Stream outStream, byte[] key)
        {
            BinaryWriter binaryWriter = (BinaryWriter)null;
            BinaryReader binaryReader = (BinaryReader)null;
            this.Key = key;
            int position = (int)outStream.Position;
            int length = (int)inStream.Length;
            int capacity = length + this.pBlockSizeValue / 8 + 4;
            if (capacity % (this.pBlockSizeValue / 8) != 0)
                capacity += this.pBlockSizeValue / 8 - capacity % (this.pBlockSizeValue / 8);
            MemoryStream memoryStream = new MemoryStream(capacity);
            try
            {
                memoryStream.SetLength((long)capacity);
                binaryWriter = new BinaryWriter((Stream)memoryStream, this.Encoding);
                binaryReader = new BinaryReader(inStream, this.Encoding);
                binaryWriter.Write(new RNDGenerator().GetBytes(this.pBlockSizeValue / 8));
                binaryWriter.Write(length);
                binaryWriter.Write(binaryReader.ReadBytes(length));
                binaryReader = new BinaryReader((Stream)memoryStream, this.Encoding);
                binaryWriter = new BinaryWriter(outStream, this.Encoding);
                BlockCipher block1 = new BlockCipher(this.pBlockSizeValue);
                if (this.CipherMode == CipherMode.CBC || this.CipherMode == CipherMode.CFB)
                {
                    if (this.pIVValue == null)
                        this.pIVValue = this.GenerateIV();
                    for (int index = 0; index < block1.Length && index < this.pIVValue.Length; ++index)
                        block1[index] = this.pIVValue[index];
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);
                outStream.Seek((long)position, SeekOrigin.Begin);
                while (memoryStream.Position < memoryStream.Length)
                {
                    switch (this.CipherMode)
                    {
                        case CipherMode.CBC:
                            BlockCipher blockCipher1 = this.EncryptBlock(new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8)) ^ block1);
                            block1 = blockCipher1;
                            binaryWriter.Write((byte[])blockCipher1);
                            continue;
                        case CipherMode.ECB:
                            BlockCipher blockCipher2 = this.EncryptBlock(new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8)));
                            binaryWriter.Write((byte[])blockCipher2);
                            continue;
                        case CipherMode.OFB:
                            block1 = this.EncryptBlock(block1);
                            BlockCipher blockCipher3 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8)) ^ block1;
                            block1 = blockCipher3;
                            binaryWriter.Write((byte[])blockCipher3);
                            continue;
                        case CipherMode.CFB:
                            block1 = this.EncryptBlock(block1);
                            BlockCipher blockCipher4 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8)) ^ block1;
                            block1 = blockCipher4;
                            binaryWriter.Write((byte[])blockCipher4);
                            continue;
                        case CipherMode.CTS:
                            BlockCipher block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            BlockCipher blockCipher5;
                            if (outStream.Position + (long)(this.pBlockSizeValue / 8 * 2) >= outStream.Length)
                            {
                                blockCipher5 = this.EncryptBlock(block2);
                            }
                            else
                            {
                                blockCipher5 = this.EncryptBlock(block2 ^ block1);
                                block1 = blockCipher5;
                            }
                            binaryWriter.Write((byte[])blockCipher5);
                            continue;
                        default:
                            throw new NotSupportedException("Cann't support the special CipherMode.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                binaryReader?.Close();
                binaryWriter?.Flush();
                memoryStream?.Flush();
            }
            GC.Collect();
        }

        public virtual void Decrypt(Stream inStream, Stream outStream, byte[] key)
        {
            MemoryStream memoryStream = (MemoryStream)null;
            BinaryWriter binaryWriter = (BinaryWriter)null;
            BinaryReader binaryReader = (BinaryReader)null;
            this.Key = key;
            int length = (int)inStream.Length;
            try
            {
                memoryStream = new MemoryStream(length);
                binaryWriter = new BinaryWriter((Stream)memoryStream);
                binaryReader = new BinaryReader(inStream, this.Encoding);
                BlockCipher block1 = new BlockCipher(this.pBlockSizeValue);
                BlockCipher block2;
                while (inStream.Position < inStream.Length)
                {
                    switch (this.CipherMode)
                    {
                        case CipherMode.CBC:
                            block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            BlockCipher blockCipher1 = (BlockCipher)block2.Clone();
                            block2 = this.DecryptBlock(block2);
                            block2 ^= block1;
                            block1 = blockCipher1;
                            binaryWriter.Write((byte[])block2);
                            continue;
                        case CipherMode.ECB:
                            block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            block2 = this.DecryptBlock(block2);
                            binaryWriter.Write((byte[])block2);
                            continue;
                        case CipherMode.OFB:
                            block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            BlockCipher blockCipher2 = this.EncryptBlock(block1);
                            BlockCipher blockCipher3 = (BlockCipher)block2.Clone();
                            block2 ^= blockCipher2;
                            block1 = blockCipher3;
                            binaryWriter.Write((byte[])block2);
                            continue;
                        case CipherMode.CFB:
                            block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            BlockCipher blockCipher4 = this.EncryptBlock(block1);
                            BlockCipher blockCipher5 = (BlockCipher)block2.Clone();
                            block2 ^= blockCipher4;
                            block1 = blockCipher5;
                            binaryWriter.Write((byte[])block2);
                            continue;
                        case CipherMode.CTS:
                            block2 = new BlockCipher(binaryReader.ReadBytes(this.pBlockSizeValue / 8));
                            if (outStream.Position + (long)(this.pBlockSizeValue / 8 * 2) >= outStream.Length)
                            {
                                block2 = this.DecryptBlock(block2);
                            }
                            else
                            {
                                BlockCipher blockCipher6 = (BlockCipher)block2.Clone();
                                block2 = this.DecryptBlock(block2);
                                block2 ^= block1;
                                block1 = blockCipher6;
                            }
                            binaryWriter.Write((byte[])block2);
                            continue;
                        default:
                            throw new NotSupportedException("Cann't support the special CipherMode.");
                    }
                }
                binaryReader = new BinaryReader((Stream)memoryStream, this.Encoding);
                binaryWriter = new BinaryWriter(outStream, this.Encoding);
                memoryStream.Seek((long)(this.pBlockSizeValue / 8), SeekOrigin.Begin);
                int count = binaryReader.ReadInt32();
                binaryWriter.Write(binaryReader.ReadBytes(count));
            }
            finally
            {
                binaryReader?.Close();
                binaryWriter?.Flush();
                memoryStream?.Flush();
            }
            GC.Collect();
        }

        protected abstract BlockCipher EncryptBlock(BlockCipher block);

        protected abstract BlockCipher DecryptBlock(BlockCipher block);

        public byte[] GenerateIV()
        {
            if (this.pIVValue == null)
                this.pIVValue = this.GenerateIV(this.pBlockSizeValue);
            return this.pIVValue;
        }

        protected virtual byte[] GenerateIV(int bitLength)
        {
            if (bitLength < 0)
                throw new ArgumentOutOfRangeException(nameof(bitLength));
            if (bitLength % 8 != 0)
                throw new ArgumentException("Invalid bitLength");
            bitLength /= 8;
            return new RNDGenerator().GetBytes(bitLength);
        }
    }
}
