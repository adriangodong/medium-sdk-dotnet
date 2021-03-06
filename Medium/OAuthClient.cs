﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Medium.Authentication;
using Medium.Helpers;

namespace Medium
{
    public class OAuthClient
    {

        private const string BaseUrl = "https://api.medium.com/v1";
        private readonly string _clientId;
        private readonly string _clientSecret;

        public OAuthClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public string GetAuthorizeUrl(
            string state,
            string redirectUri,
            Scope[] scope)
        {
            return "https://medium.com/m/oauth/authorize?" +
                $"client_id={_clientId}&" +
                $"scope={scope.Select(s => s.ToString().PascalCaseToCamelCase()).ConcatenateString(",")}&" +
                $"state={state}&" +
                $"response_type=code&" +
                $"redirect_uri={redirectUri}&";
        }

        public Token GetAccessToken(string code, string redirectUri)
        {
            var postParams = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"grant_type", "authorization_code"},
                {"redirect_uri", redirectUri}
            };

            return GetAccessToken(postParams);
        }

        public Token GetAccessToken(string refreshToken)
        {
            var postParams = new Dictionary<string, string>
            {
                {"refresh_token", refreshToken},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"grant_type", "refresh_token"},
            };

            return GetAccessToken(postParams);
        }

        private Token GetAccessToken(Dictionary<string, string> postParams)
        {
            return Helper.
                GetRequestWithToken(BaseUrl + "/tokens", HttpMethod.Post, null).
                SetRequestWwwFormUrlUrlencoded(postParams).
                GetResponse(Newtonsoft.Json.JsonConvert.DeserializeObject<Token>);
        }

    }
}
