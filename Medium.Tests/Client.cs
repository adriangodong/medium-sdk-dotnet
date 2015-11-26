using Microsoft.Extensions.Configuration;
using System.Linq;
using Medium.Authentication;
using Medium.Models;
using Xunit;

namespace Medium.Tests
{
    public class Client
    {

        private readonly string _accessToken;

        public Client() {
            var configuration = new ConfigurationBuilder().
                AddJsonFile("config.json").
                Build();
            _accessToken =  configuration["AccessToken"];
        }

        [Fact]
        public void GetCurrentUser()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(new Token { AccessToken = _accessToken });
            Assert.NotEqual(null, user);

            Assert.NotEqual(null, user.Id);
            Assert.NotEqual(null, user.Username);
            Assert.NotEqual(null, user.Name);
            Assert.NotEqual(null, user.Url);
            Assert.NotEqual(null, user.ImageUrl);
        }

        [Fact]
        public void CreatePost()
        {
            var client = new Medium.Client();
            var title = "Liverpool FC";
            var tags = new[] { "football", "sport", "liverpool" };
            var canonicalUrl = "http://jamietalbot.com/posts/liverpool-fc";
            var publishStatus = PublishStatus.Unlisted;
            var license = License.Cc40By;

            var author = client.GetCurrentUser(new Token { AccessToken = _accessToken });
            Assert.NotEqual(null, author);

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
            Assert.NotEqual(null, post);

            Assert.NotEqual(null, post.Id);
            Assert.Equal(title, post.Title);
            Assert.Equal(author.Id, post.AuthorId);
            Assert.Equal(true, tags.All(t => post.Tags.Contains(t)));
            Assert.Equal(true, post.Tags.All(t => tags.Contains(t)));
            Assert.NotEqual(null, post.Url);
            Assert.Equal(canonicalUrl, post.CanonicalUrl);
            Assert.Equal(publishStatus, post.PublishStatus);
            Assert.NotEqual(null, post.PublishedAt);
            Assert.Equal(license, post.License);
            // Assert.NotEqual(null, post.LicenseUrl); // Looks like Medium does not return this field.
        }

        [Fact]
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
            Assert.NotEqual(null, image);
            Assert.NotEqual(null, image.Url);
            Assert.Equal(md5, image.Md5);
        }

    }
}
