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

            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.Username);
            Assert.IsNotNull(user.Name);
            Assert.IsNotNull(user.Url);
            Assert.IsNotNull(user.ImageUrl);
        }

        [TestMethod]
        public void CreatePost()
        {
            var client = new Medium.Client();
            var title = "Liverpool FC";
            var tags = new[] { "football", "sport", "liverpool" };
            var canonicalUrl = "http://jamietalbot.com/posts/liverpool-fc";
            var publishStatus = PublishStatus.Unlisted;
            var license = License.Cc40By;

            var author = client.GetCurrentUser(new Token { AccessToken = _accessToken });
            Assert.IsNotNull(author);

            var body = new CreatePostRequestBody
            {
                Title = title,
                ContentFormat = ContentFormat.Html,
                Content = "<h1>Liverpool FC</h1><p>You’ll never walk alone.</p>",
                Tags = tags,
                CanonicalUrl = canonicalUrl,
                PublishStatus = publishStatus,
                License = license
            };

            var post = client.CreatePost(author.Id, body, new Token { AccessToken = _accessToken });
            Assert.IsNotNull(post);

            Assert.IsNotNull(post.Id);
            Assert.AreEqual(title, post.Title);
            Assert.AreEqual(author.Id, post.AuthorId);
            Assert.IsTrue(tags.All(post.Tags.Contains));
            Assert.IsTrue(post.Tags.All(tags.Contains));
            Assert.IsNotNull(post.Url);
            Assert.AreEqual(canonicalUrl, post.CanonicalUrl);
            Assert.AreEqual(publishStatus, post.PublishStatus);
            Assert.IsNotNull(post.PublishedAt);
            Assert.AreEqual(license, post.License);
            // Assert.IsNotNull(post.LicenseUrl); // Looks like Medium does not return this field.
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

            string md5 = System.Convert.ToBase64String(
                System.Security.Cryptography.MD5.Create().ComputeHash(body.ContentBytes)).TrimEnd('=');

            var image = client.UploadImage(body, new Token {AccessToken = _accessToken});
            Assert.IsNotNull(image);
            Assert.IsNotNull(image.Url);
            Assert.AreEqual(md5, image.Md5);
        }

    }
}
