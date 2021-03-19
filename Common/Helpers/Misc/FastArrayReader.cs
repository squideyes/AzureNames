#region Copyright & License
// Copyright © 2021 Louis S. Berman
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;

namespace AzureNames.Common.Helpers
{
    public class FastArrayReader
    {
        private readonly byte[] bytes;
        private int index;

        public FastArrayReader(byte[] bytes)
        {
            this.bytes = bytes.Validated(
                value => value != null,
                () => new ArgumentNullException(nameof(bytes)));
        }

        public byte ReadByte() => bytes[index++];

        public sbyte ReadSByte() => unchecked((sbyte)bytes[index++]);

        public ushort ReadUInt16()
        {
            var value = BitConverter.ToUInt16(bytes, index);

            index += 2;

            return value;
        }

        public short ReadInt16()
        {
            var value = BitConverter.ToInt16(bytes, index);

            index += 2;

            return value;
        }

        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(bytes, index);

            index += 4;

            return value;
        }

        public float ReadSingle()
        {
            var value = BitConverter.ToSingle(bytes, index);

            index += 4;

            return value;
        }
    }
}
