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
using System.ComponentModel.DataAnnotations;

namespace AzureNames.Common.Helpers
{
    public static class ValidationExtenders
    {
        public static bool IsValidValue<T>(this ValidationAttribute attrib, T value)
        {
            if (attrib == null)
                throw new ArgumentNullException(nameof(attrib));

            var result = attrib.GetValidationResult(value, new ValidationContext(value));

            return result == ValidationResult.Success;
        }

        public static ValidationResult MustBeSetTo(
           this ValidationContext context, string format, params object[] args)
        {
            var suffix = string.Format(format, args);

            return new ValidationResult(
                $"The {context.MemberName} field must be set to {suffix}.",
                new List<string> { context.MemberName });
        }

        public static void Validate<T>(this T instance, bool allProperties = false) =>
            Validator.ValidateObject(instance, new ValidationContext(instance), allProperties);

        public static bool TryValidate<T>(
            this TimeOfDayAttribute instance, out List<ValidationResult> results)
        {
            return instance.TryValidate<T>(true, out results);
        }

        public static bool TryValidate<T>(this TimeOfDayAttribute instance, 
            bool allProperties, out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(
                instance, new ValidationContext(instance), results, allProperties);
        }

        public static ValidationResult IsValidStruct<T>(
            this ValidationContext context, object value, Func<T, bool> isValid, string message)
        {
            if (value is null)
                return ValidationResult.Success;
            else if (value is T instance && !isValid(instance))
                return context.MustBeSetTo(message);
            else
                return ValidationResult.Success;
        }
    }
}
