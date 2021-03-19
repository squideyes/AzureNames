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

namespace AzureNames.Common.Helpers
{
    public static class IEnumerableExtenders
    {
        public static bool HasNonDefaultElements<T>(
            this IEnumerable<T> items, Func<T, bool> isValid = null)
        {
            return items.HasNonDefaultElements(1, int.MaxValue, isValid);
        }

        public static bool HasNonDefaultElements<T>(this IEnumerable<T> items, 
            int minElements, int maxElements, Func<T, bool> isValid = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (minElements < 1)
                throw new ArgumentOutOfRangeException(nameof(minElements));

            if (maxElements < minElements)
                throw new ArgumentOutOfRangeException(nameof(maxElements));

            int count = 0;

            foreach (var item in items)
            {
                if (Equals(item, default(T)))
                    return false;

                if (isValid != null && !isValid(item))
                    return false;

                if (++count > maxElements)
                    return false;
            }

            return count >= minElements;
        }
    }
}
