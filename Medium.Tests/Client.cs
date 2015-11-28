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
        private readonly string _testPublicationId = ConfigurationManager.AppSettings["TestPublicationId"];

        private Token GetToken()
        {
            return new Token { AccessToken = _accessToken };
        }

        [TestMethod]
        public void GetCurrentUser()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(GetToken());
            Assert.IsNotNull(user);

            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.Username);
            Assert.IsNotNull(user.Name);
            Assert.IsNotNull(user.Url);
            Assert.IsNotNull(user.ImageUrl);
        }

        [TestMethod]
        public void GetPublications()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(GetToken());
            Assert.IsNotNull(user);

            var publications = client.GetPublications(user.Id, GetToken());
            Assert.IsNotNull(publications);

            foreach (var publication in publications)
            {
                Assert.IsNotNull(publication.Id);
                Assert.IsNotNull(publication.Name);
                Assert.IsNotNull(publication.Description);
                Assert.IsNotNull(publication.Url);
                Assert.IsNotNull(publication.ImageUrl);
            }
        }

        [TestMethod]
        public void GetContributors()
        {
            var client = new Medium.Client();
            var contributors = client.GetContributors(_testPublicationId, GetToken());
            Assert.IsNotNull(contributors);

            foreach (var contributor in contributors)
            {
                Assert.IsNotNull(contributor.PublicationId);
                Assert.IsNotNull(contributor.UserId);
                Assert.IsNotNull(contributor.Role);
            }
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

            var author = client.GetCurrentUser(GetToken());
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

            var post = client.CreatePost(author.Id, body, GetToken());
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
            Assert.IsNotNull(post.LicenseUrl);
            Assert.IsNull(post.PublicationId);
        }

        [TestMethod]
        public void CreatePostUnderPublication()
        {
            var client = new Medium.Client();
            var title = "Liverpool FC";
            var tags = new[] { "football", "sport", "liverpool" };
            var canonicalUrl = "http://jamietalbot.com/posts/liverpool-fc";
            var publishStatus = PublishStatus.Public;
            var license = License.Cc40By;

            var author = client.GetCurrentUser(GetToken());
            Assert.IsNotNull(author);

            var body = new CreatePostRequestBody
            {
                Title = title,
                ContentFormat = ContentFormat.Html,
                Content = "<h1>Liverpool FC (Under Publication)</h1><p>You’ll never walk alone.</p>",
                Tags = tags,
                CanonicalUrl = canonicalUrl,
                PublishStatus = publishStatus,
                License = license
            };

            var post = client.CreatePostUnderPublication(_testPublicationId, body, GetToken());
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
            Assert.IsNotNull(post.LicenseUrl);

            Assert.IsNotNull(post.PublicationId);
            Assert.AreEqual(_testPublicationId, post.PublicationId);
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

            var image = client.UploadImage(body, GetToken());
            Assert.IsNotNull(image);
            Assert.IsNotNull(image.Url);
            Assert.AreEqual(md5, image.Md5);
        }

    }
}
