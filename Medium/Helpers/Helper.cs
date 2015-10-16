using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Medium.Authentication;
using Medium.Models;

namespace Medium.Helpers
{
    internal static class Helper
    {

        public static HttpWebRequest GetRequestWithToken(
            string endpointUrl,
            HttpMethod method,
            Token token)
        {
            var request = WebRequest.Create(endpointUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new ArgumentException("URL is not a valid HTTP URL.", endpointUrl);
            }

            request.Method = method.Method.ToUpper();
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");

            if (token != null)
                request.Headers.Add("Authorization", "Bearer " + token.AccessToken);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestJson(this WebRequest request, object obj)
        {
            var requestBodyString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBodyString);

            request.GetRequestStream().Write(requestBodyBytes, 0, requestBodyBytes.Length);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestMultipartFormData(
            this WebRequest request,
            KeyValuePair<string, byte[]> content,
            string boundary = null)
        {
            // if boundary is still empty, generate one
            if (string.IsNullOrWhiteSpace(boundary))
            {
                boundary = Guid.NewGuid().ToString("N");
            }

            request.ContentType = $"multipart/form-data; boundary={boundary}";

            var sb = new StringBuilder();
            sb.AppendLine($"--{boundary}");
            sb.AppendLine("Content-Disposition: form-data; name=\"image\"; filename=\"image\"");
            sb.AppendLine($"Content-Type: {content.Key}");
            sb.AppendLine();

            var requestBodyPrefixBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var requestBodySuffixBytes = Encoding.UTF8.GetBytes($"--{boundary}--");

            request.GetRequestStream().Write(requestBodyPrefixBytes, 0, requestBodyPrefixBytes.Length);
            request.GetRequestStream().Write(content.Value, 0, content.Value.Length);
            request.GetRequestStream().Write(requestBodySuffixBytes, 0, requestBodySuffixBytes.Length);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestWwwFormUrlUrlencoded(this WebRequest request, Dictionary<string, string> postParams)
        {
            var requestBodyString = GenerateWwwFormUrlEncodedString(postParams);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBodyString);

            request.GetRequestStream().Write(requestBodyBytes, 0, requestBodyBytes.Length);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static T GetResponseJson<T>(this WebRequest request) where T : class
        {
            return request.GetResponse(Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResponse<T>>)?.Data;
        }

        // Core.WebRequestExtensions.cs
        public static T GetResponse<T>(
            this WebRequest request,
            Func<string, T> responseBodyParser)
        {
            try
            {
                using (var response = request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    if (responseStream == null)
                        return default(T);

                    var responseBodyByteArray = new byte[response.ContentLength];
                    for (var i = 0; i < response.ContentLength; i++)
                    {
                        responseBodyByteArray[i] = (byte)responseStream.ReadByte();
                    }

                    var responseBody = Encoding.UTF8.GetString(responseBodyByteArray);

                    return responseBodyParser(responseBody);
                }
            }
            catch (WebException ex)
            {
                var responseStream = ex.Response?.GetResponseStream();
                if (responseStream != null)
                {
                    //logger.Error(responseStream.ReadToEnd(), ex);
                }
                else
                {
                    //logger.Error(ex.Status, ex);
                }
            }
            return default(T);
        }

        // Core.Common.cs
        public static string ConcatenateString(this IEnumerable<string> strings, string delimiter)
        {
            var sb = new StringBuilder();
            foreach (var str in strings)
            {
                if (sb.Length > 0)
                    sb.Append(delimiter);
                sb.Append(str);
            }
            return sb.ToString();
        }

        // Core.Common.cs
        public static string GenerateWwwFormUrlEncodedString(Dictionary<string, string> parameters)
        {
            return parameters.
                Select(p => $"{p.Key}={System.Web.HttpUtility.UrlEncode(p.Value)}").
                ConcatenateString("&");
        }

        // Core.TimeUtilities.cs
        public static DateTime? FromUnixTimestampMs(long? timestamp)
        {
            if (!timestamp.HasValue) return null;
            return new DateTime(1970, 1, 1).AddMilliseconds(timestamp.Value);
        }

        // Core.StringExtensions.cs
        public static string PascalCaseToCamelCase(this string source)
        {
            if (source.Length > 0 &&
                new Regex("[A-Z]", RegexOptions.Compiled).IsMatch(source[0].ToString()))
            {
                return source[0].ToString().ToLowerInvariant() + source.Substring(1);
            }
            return source;
        }

        // Core.StringExtensions.cs
        public static string CamelCaseToSpinalCase(this string source)
        {
            return new Regex("\\B([A-Z])", RegexOptions.Compiled).
                Replace(source, "-$1").
                ToLowerInvariant();
        }

    }
}
