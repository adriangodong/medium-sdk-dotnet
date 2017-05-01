using System.Linq;
using Microsoft.Extensions.Configuration;
using Medium.Authentication;
using Medium.Models;
using Xunit;

namespace Medium.Tests
{
    public class Client
    {

        private readonly string _accessToken;
        private readonly string _testPublicationId;

        public Client() {
            var configuration = new ConfigurationBuilder().
                AddJsonFile("config.json").
                Build();
            _accessToken =  configuration["AccessToken"];
            _testPublicationId = configuration["TestPublicationId"];
        }

        private Token GetToken()
        {
            return new Token { AccessToken = _accessToken };
        }

        [Fact]
        public void GetCurrentUser()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(GetToken());
            Assert.NotEqual(null, user);

            Assert.NotEqual(null, user.Id);
            Assert.NotEqual(null, user.Username);
            Assert.NotEqual(null, user.Name);
            Assert.NotEqual(null, user.Url);
            Assert.NotEqual(null, user.ImageUrl);
        }

        [Fact]
        public void GetPublications()
        {
            var client = new Medium.Client();
            var user = client.GetCurrentUser(GetToken());
            Assert.NotEqual(null, user);

            var publications = client.GetPublications(user.Id, GetToken());
            Assert.NotEqual(null, publications);

            foreach (var publication in publications)
            {
                Assert.NotEqual(null, publication.Id);
                Assert.NotEqual(null, publication.Name);
                Assert.NotEqual(null, publication.Description);
                Assert.NotEqual(null, publication.Url);
                Assert.NotEqual(null, publication.ImageUrl);
            }
        }

        [Fact]
        public void GetContributors()
        {
            var client = new Medium.Client();
            var contributors = client.GetContributors(_testPublicationId, GetToken());
            Assert.NotEqual(null, contributors);

            foreach (var contributor in contributors)
            {
                Assert.NotEqual(null, contributor.PublicationId);
                Assert.NotEqual(null, contributor.UserId);
                Assert.NotEqual(null, contributor.Role);
            }
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
            var publishedAt = new DateTime(2015, 1, 1);

            var author = client.GetCurrentUser(GetToken());
            Assert.NotEqual(null, author);

            var body = new CreatePostRequestBody
            {
                Title = title,
                ContentFormat = ContentFormat.Html,
                Content = "<h1>Liverpool FC</h1><p>You’ll never walk alone.</p>",
                Tags = tags,
                CanonicalUrl = canonicalUrl,
                PublishStatus = publishStatus,
                License = license,
                PublishedAt = publishedAt,
                NotifyFollowers = false,
            };

            var post = client.CreatePost(author.Id, body, GetToken());
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
            Assert.Equal(publishedAt, post.PublishedAt);
            Assert.Equal(license, post.License);
            Assert.NotEqual(null, post.LicenseUrl);
        }

        [Fact]
        public void CreatePostUnderPublication()
        {
            var client = new Medium.Client();
            var title = "Liverpool FC";
            var tags = new[] { "football", "sport", "liverpool" };
            var canonicalUrl = "http://jamietalbot.com/posts/liverpool-fc";
            var publishStatus = PublishStatus.Public;
            var license = License.Cc40By;

            var author = client.GetCurrentUser(GetToken());
            Assert.NotEqual(null, author);

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
            Assert.NotEqual(null, post.LicenseUrl);

            Assert.NotEqual(null, post.PublicationId);
            Assert.Equal(_testPublicationId, post.PublicationId);
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

            var image = client.UploadImage(body, GetToken());
            Assert.NotEqual(null, image);
            Assert.NotEqual(null, image.Url);
            Assert.Equal(md5, image.Md5);
        }

    }
}
