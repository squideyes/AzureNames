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
using System.Linq;

namespace AzureNames.Common.Helpers
{
    public static class IEnumerableExtenders
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items) =>
            (items == null) || (!items.Any());

        public static bool IsUnique<T>(this IEnumerable<T> values)
        {
            var diffChecker = new HashSet<T>();

            return values.All(diffChecker.Add);
        }

        public static bool SamePropertyValue<T, R>(this IEnumerable<T> items, Func<T, R> getValue) =>
            items.Select(i => getValue(i)).Distinct().Count() < 2;

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

        public static Dictionary<K, List<V>> ToListMap<K, V>(
            this IEnumerable<V> items, Func<V, K> getKey)
        {
            var dict = new Dictionary<K, List<V>>();

            foreach (var item in items)
            {
                var key = getKey(item);

                if (!dict.ContainsKey(key))
                    dict.Add(key, new List<V>());

                dict[key].Add(item);
            }

            return dict;
        }

        public static IEnumerable<IEnumerable<T>> Chunked<T>(
            this IEnumerable<T> enumerable, int chunkSize)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (chunkSize < 1)
                throw new ArgumentOutOfRangeException(nameof(chunkSize));

            using var e = enumerable.GetEnumerator();

            while (e.MoveNext())
            {
                var remaining = chunkSize;

                var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                yield return e.GetChunk(innerMoveNext);

                while (innerMoveNext()) ;
            }
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e, Func<bool> innerMoveNext)
        {
            do
            {
                yield return e.Current;
            }
            while (innerMoveNext());
        }
    }
}
