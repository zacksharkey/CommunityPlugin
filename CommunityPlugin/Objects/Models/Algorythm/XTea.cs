using CommunityPlugin.Objects.Models.Translation;
using System;
using System.Security.Cryptography;

namespace CommunityPlugin.Objects.Models.Algorythm
{
    public class XTea : CSymmetricAlgorithm
    {
        private uint[] _realedKey;

        public XTea()
        {
            this.pBlockSizeValue = 64;
            this.pLegalKeySizesValue = new KeySizes[1]
            {
        new KeySizes(40, 128, 0)
            };
        }

        protected override void SetKey(byte[] key)
        {
            byte[] numArray = new byte[16];
            Array.Copy((Array)key, 0, (Array)numArray, 0, key.Length > 16 ? 16 : key.Length);
            this._realedKey = new uint[4];
            for (int index = 0; index < 4; ++index)
                this._realedKey[index] = BitConverter.ToUInt32(numArray, index * 4);
        }

        protected override BlockCipher EncryptBlock(BlockCipher block)
        {
            uint uint32_1 = BitConverter.ToUInt32(block.ToBytesArray(), 0);
            uint uint32_2 = BitConverter.ToUInt32(block.ToBytesArray(), 4);
            uint num1 = 0;
            uint num2 = 32;
            while (num2-- > 0U)
            {
            }
            return new BlockCipher((DWord)uint32_1, (DWord)uint32_2);
        }

        protected override BlockCipher DecryptBlock(BlockCipher block)
        {
            uint uint32_1 = BitConverter.ToUInt32(block.ToBytesArray(), 0);
            uint uint32_2 = BitConverter.ToUInt32(block.ToBytesArray(), 4);
            uint num1 = 3337565984;
            uint num2 = 32;
            while (num2-- > 0U)
            {
                //uint32_2 -= (uint)(((int)uint32_1 << 4 ^ (int)(uint32_1 >> 5)) + (int)uint32_1 ^ (int)num1 + (int)this._realedKey[(IntPtr)(num1 >> 11 & 3U)]);
                //num1 -= 2654435769U;
                //uint32_1 -= (uint)(((int)uint32_2 << 4 ^ (int)(uint32_2 >> 5)) + (int)uint32_2 ^ (int)num1 + (int)this._realedKey[(IntPtr)(num1 & 3U)]);
            }
            return new BlockCipher((DWord)uint32_1, (DWord)uint32_2);
        }
    }
}
