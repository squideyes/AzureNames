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

using AzureNames.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static AzureNames.Common.Naming.NameChars;
using static AzureNames.Common.Naming.NameKind;

namespace AzureNames.Common.Naming
{
    public static class AzureRules
    {
        private class NameRule
        {
            public NameRule(int minLength, int maxLength, NameChars nameChars)
            {
                MinLength = minLength;
                MaxLength = maxLength;
                NameChars = nameChars;
            }

            public int MinLength { get; }
            public int MaxLength { get; }
            public NameChars NameChars { get; }
        }

        private static readonly Dictionary<NameKind, NameRule> nameRules =
            new Dictionary<NameKind, NameRule>();

        static AzureRules()
        {
            static void AddRule(NameKind nameKind, int minLength, int maxLength, NameChars nameChars) =>
                nameRules.Add(nameKind, new NameRule(minLength, maxLength, nameChars));

            AddRule(ManagementGroup, 1, 90, Alphanumeric | Hyphen | Underscore);
            AddRule(Subscription, 1, 90, Alphanumeric | Hyphen | Parenthesis | Underscore);
            AddRule(ResourceGroup, 1, 64, Alphanumeric | Hyphen | Parenthesis | Period | Underscore);
            AddRule(WindowsVM, 1, 15, Alphanumeric | Hyphen | Underscore);
            AddRule(LinuxVM, 1, 64, Alphanumeric | Hyphen | Underscore);
            AddRule(StorageAccount, 3, 24, LowerAlpha | Digit);
            AddRule(VirtualNetwork, 2, 64, Alphanumeric | Hyphen | Period | Underscore);
            AddRule(Subnet, 1, 80, Alphanumeric | Hyphen | Period | Underscore);
            AddRule(NetworkInterface, 1, 80, Alphanumeric | Hyphen | Period | Underscore);
            AddRule(NetworkSecurityGroup, 1, 80, Alphanumeric | Hyphen | Period | Underscore);
            AddRule(PublicIPAddress, 1, 80, Alphanumeric | Hyphen | Period | Underscore);
        }

        public static bool IsAzureName(this string name, NameKind nameKind)
        {
            if (name == null)
                throw new ArgumentOutOfRangeException(nameof(name));

            if (!nameKind.IsNonDefaultEnumValue())
                throw new ArgumentOutOfRangeException(nameof(nameKind));

            var nameRule = nameRules[nameKind];

            char c;

            for (int i = 1; i < name.Length - 1; i++)
            {
                c = name[i];

                var isValid = true;

                if (c >= 'a' && c <= 'z')
                    isValid = nameRule.NameChars.HasFlag(LowerAlpha);
                else if (c >= 'A' && c <= 'Z')
                    isValid = nameRule.NameChars.HasFlag(UpperAlpha);
                else if (c.IsZeroToNine())
                    isValid = nameRule.NameChars.HasFlag(Digit);
                else if (c == '-')
                    isValid = nameRule.NameChars.HasFlag(Hyphen);
                else if (c == '_')
                    isValid = nameRule.NameChars.HasFlag(Underscore);
                else if (c == '.')
                    isValid = nameRule.NameChars.HasFlag(Period);
                else if (c == '(' || c == ')')
                    isValid = nameRule.NameChars.HasFlag(Parenthesis);

                if (!isValid)
                    return false;
            }

            if (nameRule.MaxLength >= 2)
            {
                c = name.Last();

                if (c.IsValidLetter(nameRule.NameChars) || c.IsZeroToNine())
                    return true;
            }

            return false;
        }

        private static bool IsValidLetter(this char c, NameChars nameChars)
        {
            if (c >= 'a' && c <= 'z')
                return nameChars.HasFlag(LowerAlpha);
            else
                return false;
        }
    }
}
