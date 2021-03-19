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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AzureNames.Common.Helpers
{
    public static class StringExtenders
    {
        private static readonly Dictionary<string, Regex> regexes =
            new Dictionary<string, Regex>();

        public static bool IsMatch(this string value,
            string pattern, RegexOptions options = RegexOptions.Compiled)
        {
            if (regexes.ContainsKey(pattern))
                return regexes[pattern].IsMatch(value);

            if (!pattern.IsRegexPattern())
                throw new ArgumentOutOfRangeException(nameof(pattern));

            var regex = new Regex(pattern, options);

            regexes.Add(pattern, regex);

            return regex.IsMatch(value);
        }

        public static bool IsCode(this string value, int maxLength = 8)
        {
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength));

            return IsMatch(value, $@"^[A-Z][A-Z0-9]{{0,{maxLength - 1}}}$");
        }

        public static bool IsRegexPattern(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var regex = new Regex(value, RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            try
            {
                regex.IsMatch("");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static double ToDoubleOrNaN(this string value) =>
            string.IsNullOrWhiteSpace(value) ? double.NaN : double.Parse(value);

        public static string ToSingleLine(
            this string value, string separator = "; ", bool trimmed = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value?.Trim();

            return string.Join(separator, value.ToLines()
                .Select(v => trimmed ? v.Trim() : v));
        }

        public static string ToDelimitedString<T>(this List<T> items,
            Func<T, string> getValue = null,
            string delimiter = ", ", string finalDelimiter = " or ")
        {
            if (!items.HasNonDefaultItems())
                throw new ArgumentOutOfRangeException(nameof(items));

            if (getValue == null)
                getValue = value => value.ToString();

            if (delimiter == null)
                throw new ArgumentNullException(nameof(delimiter));

            if (finalDelimiter == null)
                throw new ArgumentNullException(nameof(delimiter));

            var sb = new StringBuilder();

            for (int i = 0; i < items.Count - 1; i++)
            {
                if (i > 0)
                    sb.Append(delimiter);

                sb.Append(getValue(items[i]));
            }

            if (sb.Length > 0)
                sb.Append(finalDelimiter);

            sb.Append(items.Last());

            return sb.ToString();
        }

        public static List<string> ToLines(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            var reader = new StringReader(value);

            var lines = new List<string>();

            string line;

            while ((line = reader.ReadLine()) != null)
                lines.Add(line.Trim());

            return lines;
        }

        public static bool IsGuid(this string value) =>
            Guid.TryParse(value, out _);

        public static bool IsNonEmptyAndTrimmed(this string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && !char.IsWhiteSpace(value[0])
                && !char.IsWhiteSpace(value[^1]);
        }

        public static string WithTrailingSlash(this string value)
        {
            return value.EndsWith(Path.DirectorySeparatorChar.ToString()) ?
                value : value + Path.DirectorySeparatorChar;
        }

        public static bool EnsurePathExists(this string value, bool valueIsPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                var path = value;

                if (!valueIsPath)
                    path = Path.GetDirectoryName(path);

                path = Path.GetFullPath(path.WithTrailingSlash());

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFolderName(
            this string value, bool mustBeRooted = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                new DirectoryInfo(value);

                if (!mustBeRooted)
                    return true;
                else
                    return Path.IsPathRooted(value);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public static Stream ToStream(this string value)
        {
            var stream = new MemoryStream();

            var writer = new StreamWriter(stream, Encoding.UTF8, -1, true);

            writer.Write(value);

            writer.Flush();

            stream.Position = 0;

            return stream;
        }

        public static Stream ToStream(this byte[] value)
        {
            var stream = new MemoryStream();

            var writer = new BinaryWriter(stream);

            writer.Write(value);

            writer.Flush();

            stream.Position = 0;

            return stream;
        }

        public static bool IsFileName(this string value, bool mustBeRooted = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return false;

                var dummy = new FileInfo(value);

                return !mustBeRooted || Path.IsPathRooted(value);
            }
            catch
            {
                return false;
            }
        }

        public static List<string> Wrap(this string text, int margin)
        {
            int start = 0;

            int end;

            var lines = new List<string>();

            text = text.Trim();

            while ((end = start + margin) < text.Length)
            {
                while ((text[end]) != ' ' && (end > start))
                    end -= 1;

                if (end == start)
                    end = start + margin;

                lines.Add(text[start..end]);

                start = end + 1;
            }

            if (start < text.Length)
                lines.Add(text.Substring(start));

            return lines;
        }

        public static bool InChars(this string value, string chars) =>
            Enumerable.All(value, c => chars.Contains(c));

        public static List<string> CamelCaseToWords(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            return Regex.Matches(value, "(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][a-z]+)")
                .OfType<Match>()
                .Select(m => m.Value)
                .ToList();
        }
    }
}