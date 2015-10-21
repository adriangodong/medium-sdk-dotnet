using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Medium.Tests
{
    [TestClass]
    public class OAuthClient
    {

        [TestMethod]
        public void GetAuthorizeUrl()
        {
            var client = new Medium.OAuthClient("clientId", "clientSecret");
            var authorizeUrl = client.GetAuthorizeUrl("state", "uri",
                new[] {
                    Medium.Authentication.Scope.BasicProfile,
                    Medium.Authentication.Scope.PublishPost
                });

            var expectedUrl =
                "https://medium.com/m/oauth/authorize?" +
                "client_id=clientId&" +
                "scope=basicProfile,publishPost&" +
                "state=state&" +
                "response_type=code&" +
                "redirect_uri=uri&";

            Assert.AreEqual(expectedUrl, authorizeUrl);
        }

    }
}
