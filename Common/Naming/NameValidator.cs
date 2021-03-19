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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static AzureNames.Common.Properties.Resources;

namespace AzureNames.Common.Naming
{
    public class NameValidator
    {
        private class Token
        {
            public string Value { get; set; }
            public bool IsOptional { get; set; }
        }

        private class Field
        {
            public Regex Regex { get; set; }
            public HashSet<string> Values { get; set; }

            public bool IsValid()
            {
                if (Regex == null && Values == null)
                    return false;
                else if (Regex != null && Values != null)
                    return false;
                else if (Regex != null)
                    return true;
                else
                    return Values.HasNonDefaultElements(v => v.IsNonEmptyAndTrimmed());
            }
        }

        private class Rule
        {
            private readonly Dictionary<string, bool> tokens = new Dictionary<string, bool>();

            public Regex Regex { get; set; }
            public NameTemplate Template { get; set; }
            public List<string> Samples { get; set; }

            public bool IsValid(NameKind nameKind, HashSet<string> validTokens)
            {
                if (!Samples.HasNonDefaultItems(e => e.IsAzureName(nameKind)))
                    return false;

                if (Regex == null && Template == null)
                {
                    return true;
                }
                else if (Regex != null)
                {
                    return true;
                }
                else
                {
                    if (!Template.IsValid)
                        return false;

                    foreach (var (token, _) in Template.Tokens)
                    {
                        if (!validTokens.Contains(token) && token != "NameCode")
                            return false;
                    }

                    return true;
                }
            }
        }

        private class RuleSet
        {
            public string Name { get; set; }
            public Dictionary<string, Field> Fields { get; set; }
            public Dictionary<NameKind, Rule> Rules { get; set; }

            public void Validate()
            {
                if (!Name.IsTokenName())
                    throw new ArgumentOutOfRangeException(nameof(Name));

                if (Fields.Keys.Any(k => !k.IsTokenName()))
                    throw new ArgumentOutOfRangeException(nameof(Fields));

                if (Rules.Keys.Any(k => !k.IsNonDefaultEnumValue()))
                    throw new ArgumentOutOfRangeException(nameof(Fields));

                if (!Fields.Values.HasNonDefaultElements(f => f.IsValid()))
                    throw new ArgumentOutOfRangeException(nameof(Fields));

                var tokens = new HashSet<string>(Fields.Keys);

                foreach (var nameKind in Rules.Keys)
                {
                    if (!Rules[nameKind].IsValid(nameKind, tokens))
                        throw new ArgumentOutOfRangeException(nameof(Rules));
                }
            }
        }

        private readonly RuleSet ruleSet;

        public NameValidator(RuleSetId ruleSetId = RuleSetId.Default)
        {
            string json = ruleSetId switch
            {
                RuleSetId.Default => Encoding.UTF8.GetString(DefaultRuleSet),
                _ => throw new ArgumentOutOfRangeException(nameof(ruleSetId)),
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new JsonStrinRegexConverter());
            options.Converters.Add(new JsonStringNameTemplateConverter());

            var ruleSet = JsonSerializer.Deserialize<RuleSet>(json, options);

            ruleSet.Validate();

            this.ruleSet = ruleSet;
        }

        public bool IsValid(string name, NameKind nameKind)
        {
            var rule = ruleSet.Rules[nameKind];

            if (rule.Regex != null)
            {
                return rule.Regex.IsMatch(name);
            }
            else if (rule.Template != null)
            {
                var chunks = name.Split('-');

                for (var i = 0; i < chunks.Length; i++)
                {
                    var (token, _) = rule.Template.Tokens[i];

                    if (token == "NameCode")
                    {
                        if (chunks[i] != nameKind.ToCode())
                            return false;
                    }
                    else
                    {
                        if (!ruleSet.Fields.TryGetValue(token, out Field field))
                            return false;

                        var chunk = chunks[i];

                        if (field.Regex != null && !field.Regex.IsMatch(chunk))
                            return false;
                        else if (field.Values != null && !field.Values.Contains(chunk))
                            return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public List<(string Sample, NameKind Namekind)> GetSamples(NameKind filter = default)
        {
            var samples = new List<(string Sample, NameKind Namekind)>();

            foreach (var rule in ruleSet.Rules)
            {
                if (filter != default && rule.Key != filter)
                    continue;

                foreach (var sample in rule.Value.Samples)
                    samples.Add((sample, rule.Key));
            }

            return samples;
        }
    }
}
