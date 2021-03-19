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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureNames.Common.Helpers
{
    public class HttpHelper
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly List<string> segments = new List<string>();

        private readonly Dictionary<string, string> queryParams =
            new Dictionary<string, string>();

        private readonly Uri baseUri;

        public HttpHelper(Uri baseUri, string authToken)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));

            if (string.IsNullOrWhiteSpace(authToken))
                throw new ArgumentNullException(nameof(authToken));

            this.baseUri = new Uri(baseUri
                .GetLeftPart(UriPartial.Authority));

            segments.AddRange(baseUri.LocalPath.Split('/')
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
        }

        public HttpClient GetHttpClient() => client;

        public HttpHelper AppendPathSegment(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                throw new ArgumentOutOfRangeException(nameof(segment));

            if (!Uri.IsWellFormedUriString(segment, UriKind.Relative))
                throw new ArgumentOutOfRangeException(nameof(segment));

            segments.Add(segment);

            return this;
        }

        public HttpHelper SetQueryParam(string token, string value = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentOutOfRangeException(nameof(token));

            if (!Uri.IsWellFormedUriString(value, UriKind.Relative))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (!Uri.IsWellFormedUriString(value, UriKind.Relative))
                    throw new ArgumentOutOfRangeException(nameof(value));
            }

            queryParams.Add(token, value);

            return this;
        }

        public Uri GetUri()
        {
            var sb = new StringBuilder();

            sb.Append(baseUri.AbsoluteUri);
            sb.Append(string.Join("/", segments));

            int count = 0;

            foreach (var key in queryParams.Keys)
            {
                sb.Append(count++ == 0 ? "?" : "&");
                sb.Append(key);

                if (queryParams[key] != null)
                {
                    sb.Append("=");
                    sb.Append(queryParams[key]);
                }
            }

            return new Uri(sb.ToString());
        }

        public async Task<string> GetStringAsync() =>
            await client.GetStringAsync(GetUri());

        public async Task<T> GetJsonAsync<T>()
            where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(
                await client.GetStringAsync(GetUri()));
        }
    }
}
