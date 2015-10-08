using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Medium.Tests
{
    [TestClass]
    public class OAuthClient
    {

        private readonly string _clientId = ConfigurationManager.AppSettings["ClientId"];
        private readonly string _clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

        [TestMethod]
        public void GetAuthorizeUrl()
        {
            var client = new Medium.OAuthClient(_clientId, _clientSecret);
            var authorizeUrl = client.GetAuthorizeUrl("state", "uri",
                new[] {
                    Medium.Authentication.Scope.BasicProfile,
                    Medium.Authentication.Scope.PublishPost
                });
            Assert.AreEqual(string.Empty, authorizeUrl);
        }

        [TestMethod]
        public void GetAccessToken()
        {
            var client = new Medium.OAuthClient(_clientId, _clientSecret);
            Assert.IsNotNull(client.GetAccessToken("code", "uri"));
        }

    }
}
