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
using System.Reflection;

namespace AzureNames.Common.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NonDefaultEnumValueAttribute : ValidationAttribute
    {
        private readonly Type type;

        public NonDefaultEnumValueAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.GetTypeInfo().IsEnum)
                throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is null)
                return ValidationResult.Success;
            else if (value is Enum instance && !Enum.IsDefined(type, instance))
                return context.MustBeSetTo($"a pre-defined {type.FullName}");
            else
                return ValidationResult.Success;
        }
    }
}
