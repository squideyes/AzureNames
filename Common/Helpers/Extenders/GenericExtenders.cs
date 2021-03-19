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
using System.Linq;

namespace AzureNames.Common.Helpers
{
    public static class GenericExtenders
    {
        public static bool HasMaskBits(this byte value, byte mask) =>
            (value & mask) == mask;

        public static bool HasMaskBits(this int value, int mask) =>
            (value & mask) == mask;

        public static T Validated<T>(
            this T value, Func<T, bool> isValid, Func<Exception> getError)
        {
            if (getError == null)
                throw new ArgumentNullException(nameof(getError));

            if (isValid(value))
                return value;
            else
                throw getError();
        }

        public static T Validated<T>(
            this T value, string fieldName, Func<T, bool> isValid)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (isValid(value))
                return value;
            else
                throw new ArgumentOutOfRangeException(fieldName);
        }

        public static R Validated<T, R>(this T value,
            string fieldName, Func<T, bool> isValid, Func<T, R> getResult)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (getResult == null)
                throw new ArgumentNullException(nameof(getResult));

            if (isValid(value))
                return getResult(value);
            else
                throw new ArgumentOutOfRangeException(nameof(fieldName));
        }

        public static R Validated<T, R>(this T value,
            Func<T, bool> isValid, Func<T, R> getResult, Func<Exception> getError)
        {
            if (getResult == null)
                throw new ArgumentNullException(nameof(getResult));

            if (getError == null)
                throw new ArgumentNullException(nameof(getError));

            if (isValid(value))
                return getResult(value);
            else
                throw getError();
        }

        public static bool In<T>(this T value, params T[] choices) =>
            choices.Contains(value);

        public static bool Between(this int value,
            int minValue, int maxValue, bool exclusive = false)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            if (exclusive)
                return value > minValue && value < maxValue;
            else
                return value >= minValue && value <= maxValue;
        }

        public static bool Between(this float value,
            float minValue, float maxValue, bool exclusive = false)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            if (exclusive)
                return value > minValue && value < maxValue;
            else
                return value >= minValue && value <= maxValue;
        }

        public static bool Between(this double value,
            double minValue, double maxValue, bool exclusive = false)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            if (exclusive)
                return value > minValue && value < maxValue;
            else
                return value >= minValue && value <= maxValue;
        }

        public static string ToUpper<T>(this T value) => value.ToString().ToUpper();

        public static string ToLower<T>(this T value) => value.ToString().ToLower();
    }
}
