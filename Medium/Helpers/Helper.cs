using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly Regex IsUpperCaseRegex = new Regex("([A-Z])", RegexOptions.Compiled);
        private static readonly Regex IsSetOfDigitsRegex = new Regex("(\\d+)", RegexOptions.Compiled);

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
            request.Accept = "application/json";
            request.Headers[HttpRequestHeader.AcceptCharset] = "utf-8";

            if (token != null)
                request.Headers["Authorization"] = "Bearer " + token.AccessToken;

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestJson(this WebRequest request, object obj)
        {
            var requestBodyString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBodyString);

            request.ContentType = "application/json";
            request.GetRequestStream().Write(requestBodyBytes, 0, requestBodyBytes.Length);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestMultipartFormData(
            this WebRequest request,
            string contentType,
            byte[] content,
            string boundary = null)
        {
            // if boundary is still empty, generate one
            if (string.IsNullOrWhiteSpace(boundary))
            {
                boundary = Guid.NewGuid().ToString("N");
            }

            var sb = new StringBuilder();
            sb.AppendLine($"--{boundary}");
            sb.AppendLine("Content-Disposition: form-data; name=\"image\"; filename=\"image\"");
            sb.AppendLine($"Content-Type: {contentType}");
            sb.AppendLine();

            var requestBodyPrefixBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var requestBodySuffixBytes = Encoding.UTF8.GetBytes($"{Environment.NewLine}--{boundary}--");

            request.ContentType = $"multipart/form-data; boundary={boundary}";
            
            var requestStream = request.GetRequestStream();
            requestStream.Write(requestBodyPrefixBytes, 0, requestBodyPrefixBytes.Length);
            requestStream.Write(content, 0, content.Length);
            requestStream.Write(requestBodySuffixBytes, 0, requestBodySuffixBytes.Length);

            return request;
        }

        // Core.WebRequestExtensions.cs
        public static WebRequest SetRequestWwwFormUrlUrlencoded(
            this WebRequest request,
            Dictionary<string, string> postParams)
        {
            var requestBodyString = GenerateWwwFormUrlEncodedString(postParams);
            var requestBodyBytes = Encoding.UTF8.GetBytes(requestBodyString);

            request.ContentType = "application/x-www-form-urlencoded";
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
                    var responseBody = responseStream.ReadToEnd();
                    throw new InvalidOperationException(responseBody, ex);
                }
                throw;
            }
        }

        // Core.StreamExtensions.cs
        public static string ReadToEnd(
            this Stream stream,
            bool seekToStart = true,
            System.Text.Encoding encoding = null)
        {
            var buffer = new byte[stream.Length];
            if (stream.CanSeek && seekToStart)
                stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, (int)stream.Length);
            return (encoding ?? Encoding.UTF8).GetString(buffer);
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
                Select(p => $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}").
                ConcatenateString("&");
        }

        // Core.TimeUtilities.cs
        public static DateTime? FromUnixTimestampMs(long? timestamp)
        {
            if (!timestamp.HasValue) return null;
            return new DateTime(1970, 1, 1).AddMilliseconds(timestamp.Value);
        }

        // Core.StringExtensions.cs
        public static string Replace(this string source, Regex regex, string replacement)
        {
            return regex.Replace(source, replacement);
        }

        // Core.StringExtensions.cs
        public static string PascalCaseToCamelCase(this string source)
        {
            if (source.Length > 0 &&
                IsUpperCaseRegex.IsMatch(source[0].ToString()))
            {
                return source[0].ToString().ToLowerInvariant() + source.Substring(1);
            }
            return source;
        }

        // Core.StringExtensions.cs
        public static string CamelCaseToSpinalCase(this string source)
        {
            return source.
                PascalCaseToCamelCase().
                Replace(IsUpperCaseRegex, "-$1").
                Replace(IsSetOfDigitsRegex, "-$1").
                ToLowerInvariant();
        }

        private static Stream GetRequestStream(this WebRequest request)
        {
            System.Threading.Tasks.Task<Stream> task = request.GetRequestStreamAsync();
            while (!task.IsCompleted) { }
            return task.Result;
        }
        
        private static WebResponse GetResponse(this WebRequest request)
        {
            System.Threading.Tasks.Task<WebResponse> task = request.GetResponseAsync();
            while (!task.IsCompleted) { }
            return task.Result;
        }
        
    }
}
