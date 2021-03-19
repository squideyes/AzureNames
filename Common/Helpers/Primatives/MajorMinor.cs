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
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AzureNames.Common.Helpers
{
    public struct MajorMinor : IEquatable<MajorMinor>
    {
        [Required]
        [Range(0, 255)]
        public byte Major { get; set; }

        [Required]
        [Range(0, 255)]
        public byte Minor { get; set; }

        public MajorMinor(int major, int minor)
        {
            if (major < 0 || major > 255)
                throw new ArgumentOutOfRangeException(nameof(major));

            if (minor < 0 || minor > 255)
                throw new ArgumentOutOfRangeException(nameof(minor));

            Major = (byte)major;
            Minor = (byte)minor;
        }

        public override string ToString() => $"{Major}.{Minor}";

        public void Write(BinaryWriter writer)
        {
            writer.Write(Major);
            writer.Write(Minor);
        }

        public static MajorMinor Read(BinaryReader reader)
        {
            var major = reader.ReadByte();
            var minor = reader.ReadByte();

            return new MajorMinor(major, minor);
        }

        public static bool TryParse(string value, out MajorMinor majorMinor)
        {
            majorMinor = default;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            var fields = value.Split('.');

            if (fields.Length > 2)
                return false;

            if (!byte.TryParse(fields[0], out byte major))
                return false;

            byte minor = 0;

            if (fields.Length == 2 && !byte.TryParse(fields[1], out minor))
                return false;

            majorMinor = new MajorMinor(major, minor);

            return true;
        }

        public static MajorMinor Parse(string value)
        {
            if (!TryParse(value, out MajorMinor majorMinor))
                throw new ArgumentOutOfRangeException(nameof(value));

            return majorMinor;
        }

        public static implicit operator MajorMinor(string value) => Parse(value);

        public bool Equals(MajorMinor other) =>
            Major == other.Major && Minor == other.Minor;

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(MajorMinor))
                return false;

            return Equals((MajorMinor)other);
        }

        public override int GetHashCode()=> HashCode.Combine(Major, Minor);

        public static bool operator ==(MajorMinor left, MajorMinor right) =>
            left.Equals(right);

        public static bool operator !=(MajorMinor left, MajorMinor right) =>
            !(left == right);
    }
}
