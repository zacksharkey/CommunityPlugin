using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public struct DWord
    {
        private byte[] _value;

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= 4)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return this._value[index];
            }
            set
            {
                if (index < 0 || index >= 4)
                    throw new ArgumentOutOfRangeException(nameof(index));
                this._value[index] = value;
            }
        }

        public byte[] Bytes
        {
            get
            {
                return this._value;
            }
        }

        public DWord(int word)
        {
            this._value = BitConverter.GetBytes(word);
        }

        public DWord(uint word)
        {
            this._value = BitConverter.GetBytes(word);
        }

        public int ToInt32()
        {
            return BitConverter.ToInt32(this._value, 0);
        }

        public uint ToUInt32()
        {
            return BitConverter.ToUInt32(this._value, 0);
        }

        public static DWord New()
        {
            return new DWord(0);
        }

        public static DWord[] And(DWord[] dws1, DWord[] dws2)
        {
            if (dws1 == null)
                throw new ArgumentNullException(nameof(dws1));
            if (dws2 == null)
                throw new ArgumentNullException(nameof(dws2));
            if (dws1.Length != dws2.Length)
                throw new ArgumentException("dws1's length must equals dws2's length");
            DWord[] dwordArray = new DWord[dws1.Length];
            for (int index = 0; index < dwordArray.Length; ++index)
                dwordArray[index] = dws1[index] & (int)dws2[index];
            return dwordArray;
        }

        public static DWord[] Or(DWord[] dws1, DWord[] dws2)
        {
            if (dws1 == null)
                throw new ArgumentNullException(nameof(dws1));
            if (dws2 == null)
                throw new ArgumentNullException(nameof(dws2));
            if (dws1.Length != dws2.Length)
                throw new ArgumentException("dws1's length must equals dws2's length");
            DWord[] dwordArray = new DWord[dws1.Length];
            for (int index = 0; index < dwordArray.Length; ++index)
                dwordArray[index] = dws1[index] | dws2[index];
            return dwordArray;
        }

        public static DWord[] Xor(DWord[] dws1, DWord[] dws2)
        {
            if (dws1 == null)
                throw new ArgumentNullException(nameof(dws1));
            if (dws2 == null)
                throw new ArgumentNullException(nameof(dws2));
            if (dws1.Length != dws2.Length)
                throw new ArgumentException("dws1's length must equals dws2's length");
            DWord[] dwordArray = new DWord[dws1.Length];
            for (int index = 0; index < dwordArray.Length; ++index)
                dwordArray[index] = dws1[index] ^ dws2[index];
            return dwordArray;
        }

        public static DWord operator +(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() + w2.ToUInt32());
        }

        public static DWord operator +(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() + i);
        }

        public static DWord operator +(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() + (uint)i);
        }

        public static DWord operator -(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() - w2.ToUInt32());
        }

        public static DWord operator -(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() - i);
        }

        public static DWord operator -(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() - (uint)i);
        }

        public static DWord operator *(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() * w2.ToUInt32());
        }

        public static DWord operator *(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() * i);
        }

        public static DWord operator *(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() * (uint)i);
        }

        public static DWord operator /(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() / w2.ToUInt32());
        }

        public static DWord operator /(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() / i);
        }

        public static DWord operator /(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() / (uint)i);
        }

        public static DWord operator %(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() % w2.ToUInt32());
        }

        public static DWord operator %(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() % i);
        }

        public static DWord operator %(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() % (uint)i);
        }

        public static DWord operator &(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() & i);
        }

        public static DWord operator &(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() & (uint)i);
        }

        public static DWord operator |(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() | w2.ToUInt32());
        }

        public static DWord operator |(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() | i);
        }

        public static DWord operator |(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() | (uint)i);
        }

        public static DWord operator ^(DWord w1, DWord w2)
        {
            return new DWord(w1.ToUInt32() ^ w2.ToUInt32());
        }

        public static DWord operator ^(DWord w1, uint i)
        {
            return new DWord(w1.ToUInt32() ^ i);
        }

        public static DWord operator ^(DWord w1, int i)
        {
            return new DWord(w1.ToUInt32() ^ (uint)i);
        }

        public static DWord operator >>(DWord w1, int shift)
        {
            return new DWord(w1.ToUInt32() >> shift);
        }

        public static DWord operator <<(DWord w1, int shift)
        {
            return new DWord(w1.ToUInt32() << shift);
        }

        public static explicit operator DWord(uint i)
        {
            return new DWord(i);
        }

        public static explicit operator DWord(int i)
        {
            return new DWord(i);
        }

        public static implicit operator uint(DWord dw)
        {
            return dw.ToUInt32();
        }

        public static implicit operator int(DWord dw)
        {
            return dw.ToInt32();
        }
    }
}
