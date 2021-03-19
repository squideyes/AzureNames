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
using System.Text.RegularExpressions;

namespace AzureNames.Common.Naming
{
    public static class NameExtenders
    {
        private static readonly Regex contextValidator =
            new Regex("^[A-Z][A-Za-z0-9]*$", RegexOptions.Compiled);

        public static bool IsTokenName(this string value, int maxLength = 50) =>
            contextValidator.IsMatch(value) && value.Length <= maxLength;

        public static string ToCode(this NameKind nameKind)
        {
            return nameKind switch
            {
                NameKind.ManagementGroup => "mg",
                NameKind.Subscription => "sub",
                NameKind.ResourceGroup => "rg",
                NameKind.WindowsVM => "vm",
                NameKind.LinuxVM => "vm",
                NameKind.StorageAccount => "st",
                NameKind.VirtualNetwork => "vnet",
                NameKind.Subnet => "snet",
                NameKind.NetworkInterface => "nic",
                NameKind.NetworkSecurityGroup => "nsg",
                NameKind.PublicIPAddress => "pip",
                _ => throw new ArgumentOutOfRangeException(nameof(nameKind))
            };
        }     
    }
}
