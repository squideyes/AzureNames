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
using System.Reflection;
using System.Text;

namespace AzureNames.Common.Helpers
{
    public class AppInfo
    {
        public AppInfo(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            Company = GetCompany(assembly);
            Product = GetProduct(assembly);
            PackageId = GetPackageId(assembly);
            Version = assembly.GetName().Version;
            Copyright = GetCopyright(assembly).Replace("©", "(c)");
        }

        public string Product { get; private set; }
        public string PackageId { get; private set; }
        public Version Version { get; private set; }
        public string Company { get; private set; }
        public string Copyright { get; private set; }

        public string Title
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(PackageId);

                sb.Append(" v");
                sb.Append(Version.Major);
                sb.Append('.');
                sb.Append(Version.Minor);

                if ((Version.Build != 0) || (Version.Revision != 0))
                {
                    sb.Append('.');
                    sb.Append(Version.Build);
                }

                if (Version.Revision != 0)
                {
                    sb.Append('.');
                    sb.Append(Version.Revision);
                }

                return sb.ToString();
            }
        }

        private static string GetCompany(Assembly assembly) =>
            assembly.GetAttribute<AssemblyCompanyAttribute>().Company;

        private static string GetCopyright(Assembly assembly) =>
            assembly.GetAttribute<AssemblyCopyrightAttribute>().Copyright;

        private static string GetProduct(Assembly assembly) =>
            assembly.GetAttribute<AssemblyProductAttribute>().Product;

        private static string GetPackageId(Assembly assembly) =>
            assembly.GetAttribute<AssemblyTitleAttribute>().Title;
    }
}
