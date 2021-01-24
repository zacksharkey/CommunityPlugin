using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public struct BlockCipher : ICloneable
    {
        private int cliperSize;
        private byte[] _buffer;

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= this._buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return this._buffer[index];
            }
            set
            {
                if (index < 0 || index >= this._buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                this._buffer[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return this._buffer.Length;
            }
        }

        public BlockCipher(int bitLength)
          : this(new byte[bitLength / 8])
        {
        }

        public BlockCipher(byte[] buffer)
        {
            this._buffer = buffer;
            this.cliperSize = this._buffer.Length * 8;
        }

        public BlockCipher(byte[] buffer, int offset, int length)
        {
            this._buffer = new byte[length];
            Array.Copy((Array)buffer, offset, (Array)this._buffer, 0, length);
            this.cliperSize = this._buffer.Length * 8;
        }

        public BlockCipher(DWord left, DWord right)
          : this(new DWord[2] { left, right })
        {
        }

        public BlockCipher(DWord[] dws)
        {
            this._buffer = new byte[dws.Length * 4];
            for (int index = 0; index < dws.Length; ++index)
                Array.Copy((Array)dws[index].Bytes, 0, (Array)this._buffer, index * 4, 4);
            this.cliperSize = this._buffer.Length * 8;
        }

        public byte[] ToBytesArray()
        {
            return this._buffer;
        }

        public DWord[] ToDWords()
        {
            DWord[] dwordArray = new DWord[this._buffer.Length % 4 == 0 ? this._buffer.Length / 4 : this._buffer.Length / 4 + 1];
            for (int index = 0; index < this._buffer.Length / 4; ++index)
                dwordArray[index] = new DWord(BitConverter.ToUInt32(this._buffer, index * 4));
            return dwordArray;
        }

        public object Clone()
        {
            return (object)new BlockCipher((byte[])this._buffer.Clone());
        }

        public static BlockCipher operator ^(BlockCipher b1, BlockCipher b2)
        {
            byte[] buffer1 = b1._buffer;
            byte[] buffer2 = b2._buffer;
            int length = buffer1.Length >= buffer2.Length ? buffer1.Length : buffer2.Length;
            byte[] buffer3 = new byte[length];
            for (int index = 0; index < length; ++index)
                buffer3[index] = index < buffer1.Length ? (index < buffer2.Length ? (byte)((uint)buffer1[index] ^ (uint)buffer2[index]) : buffer1[index]) : buffer2[index];
            return new BlockCipher(buffer3);
        }

        public static BlockCipher operator &(BlockCipher b1, BlockCipher b2)
        {
            byte[] buffer1 = b1._buffer;
            byte[] buffer2 = b2._buffer;
            int length = buffer1.Length >= buffer2.Length ? buffer1.Length : buffer2.Length;
            byte[] buffer3 = new byte[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                if (index1 >= buffer1.Length)
                {
                    byte[] numArray = buffer3;
                    int index2 = index1;
                    int num1 = (int)buffer2[index1];
                    int num2 = 0;
                    numArray[index2] = (byte)num2;
                }
                else if (index1 >= buffer2.Length)
                {
                    byte[] numArray = buffer3;
                    int index2 = index1;
                    int num1 = (int)buffer1[index1];
                    int num2 = 0;
                    numArray[index2] = (byte)num2;
                }
                else
                    buffer3[index1] = (byte)((uint)buffer1[index1] & (uint)buffer2[index1]);
            }
            return new BlockCipher(buffer3);
        }

        public static BlockCipher operator |(BlockCipher b1, BlockCipher b2)
        {
            byte[] buffer1 = b1._buffer;
            byte[] buffer2 = b2._buffer;
            int length = buffer1.Length >= buffer2.Length ? buffer1.Length : buffer2.Length;
            byte[] buffer3 = new byte[length];
            for (int index = 0; index < length; ++index)
                buffer3[index] = index < buffer1.Length ? (index < buffer2.Length ? (byte)((uint)buffer1[index] | (uint)buffer2[index]) : buffer1[index]) : buffer2[index];
            return new BlockCipher(buffer3);
        }

        public static explicit operator BlockCipher(byte[] b)
        {
            return new BlockCipher(b);
        }

        public static implicit operator byte[](BlockCipher c)
        {
            return c._buffer;
        }
    }
}
