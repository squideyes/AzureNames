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
    public class ConfigSet
    {
        private readonly Dictionary<string, IConvertible> dials =
            new Dictionary<string, IConvertible>();

        public void Upsert(string key, bool value) => DoUpsert(key, value, null);

        public void Upsert(string key, int value, Func<int, bool> validate = null) =>
            DoUpsert(key, value, validate);

        public void Upsert(string key, double value, Func<double, bool> validate = null) =>
            DoUpsert(key, value, validate);

        public void Upsert<T>(string key, T value, Func<T, bool> validate = null)
            where T : Enum
        {
            if (validate == null)
                DoUpsert(key, value, v => v.IsNonDefaultEnumValue());
            else
                DoUpsert(key, value, validate);
        }

        public void Upsert(string key, string value, Func<string, bool> validate = null) =>
            DoUpsert(key, value, validate);

        public void Upsert(string key, Uri value, Func<Uri, bool> validate = null)
        {
            if (!key.IsNonEmptyAndTrimmed())
                throw new ArgumentOutOfRangeException(nameof(key));

            if (value == null || !value.IsAbsoluteUri)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (validate != null && !validate(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (dials.ContainsKey(key))
                dials[key] = value.AbsoluteUri;
            else
                dials.Add(key, value.AbsoluteUri);
        }

        protected void DoUpsert<T>(string key, T value, Func<T, bool> validate)
        {
            if (!key.IsNonEmptyAndTrimmed())
                throw new ArgumentOutOfRangeException(nameof(key));

            if (validate != null && !validate(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (dials.ContainsKey(key))
                dials[key] = (IConvertible)value;
            else
                dials.Add(key, (IConvertible)value);
        }

        public int Count => dials.Count;

        public bool ContainsKey(string key) => dials.ContainsKey(key);

        public bool ContainsKeys(params string[] keys)
        {
            if (Count != keys.Length)
                return false;

            foreach (var key in keys)
            {
                if (!ContainsKey(key))
                    return false;
            }

            return true;
        }

        public Dictionary<string, IConvertible> ToDictionary()
        {
            var dict = new Dictionary<string, IConvertible>();

            foreach (var key in dials.Keys)
                dict.Add(key, dials[key]);

            return dict;
        }

        public bool GetBool(string key) =>
            (bool)Convert.ChangeType(dials[key], typeof(bool));

        public int GetInt32(string key) =>
            (int)Convert.ChangeType(dials[key], typeof(int));

        public float GetFloat(string key) =>
            (float)Convert.ChangeType(dials[key], typeof(float));

        public double GetDouble(string key) =>
            (double)Convert.ChangeType(dials[key], typeof(double));

        public T GetEnum<T>(string key)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentOutOfRangeException(nameof(T));

            return (T)Convert.ChangeType(dials[key], typeof(T));
        }

        public string GetString(string key) =>
            (string)Convert.ChangeType(dials[key], typeof(string));

        public Uri GetUri(string key) => new Uri(GetString(key));
    }
}
