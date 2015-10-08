using System.Configuration;
using System.Linq;
using Medium.Authentication;
using Medium.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Medium.Tests
{
    [TestClass]
    public class Client
    {

        private readonly string _accessToken = ConfigurationManager.AppSettings["AccessToken"];

        [TestMethod]
        public void GetCurrentUser()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(new Token { AccessToken = _accessToken });
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void CreatePost()
        {
            var client = new Medium.Client();
            var title = "Liverpool FC";
            var tags = new[] { "football", "sport", "liverpool" };
            var publishStatus = PublishStatus.Unlisted;
            var license = License.PublicDomain;

            var author = client.GetCurrentUser(new Token { AccessToken = _accessToken });
            Assert.IsNotNull(author);

            var body = new CreatePostRequestBody
            {
                Title = title,
                ContentFormat = ContentFormat.Html,
                Content = "<h1>Liverpool FC</h1><p>You’ll never walk alone.</p>",
                Tags = tags,
                PublishStatus = publishStatus,
                License = license
            };

            var post = client.CreatePost(author.Id, body, new Token { AccessToken = _accessToken });
            Assert.IsNotNull(post);

            Assert.AreEqual(title, post.Title);
            Assert.AreEqual(author.Id, post.AuthorId);
            Assert.IsTrue(tags.All(post.Tags.Contains));
            Assert.IsTrue(post.Tags.All(tags.Contains));
            Assert.AreEqual(publishStatus, post.PublishStatus);
            Assert.IsNotNull(post.PublishedAt);
            Assert.AreEqual(license, post.License);
        }

        [TestMethod]
        public void UploadImage()
        {
            var client = new Medium.Client();

            var body = new UploadImageRequestBody
            {
                ContentType = "image/gif",
                ContentBytes = System.Convert.FromBase64String("R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=")
            };

            Assert.IsNotNull(client.UploadImage(body, new Token { AccessToken = _accessToken }));
        }

    }
}
