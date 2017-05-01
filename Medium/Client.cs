using System.Collections.Generic;
using Medium.Authentication;
using Medium.Helpers;
using Medium.Models;

namespace Medium
{
    public class Client
    {

        private const string BaseUrl = "https://api.medium.com/v1";

        public User GetCurrentUser(Token token)
        {
            var request = Helper.GetRequestWithToken(
                BaseUrl + "/me",
                System.Net.Http.HttpMethod.Get,
                token);

            return request.GetResponseJson<User>();
        }

        public IEnumerable<Publication> GetPublications(string userId, Token token)
        {
            var request = Helper.GetRequestWithToken(
                BaseUrl + $"/users/{userId}/publications",
                System.Net.Http.HttpMethod.Get,
                token);

            return request.GetResponseJson<IEnumerable<Publication>>();
        }

        public IEnumerable<Contributor> GetContributors(string publicationId, Token token)
        {
            var request = Helper.GetRequestWithToken(
                BaseUrl + $"/publications/{publicationId}/contributors",
                System.Net.Http.HttpMethod.Get,
                token);

            return request.GetResponseJson<IEnumerable<Contributor>>();
        }

        public Post CreatePost(string authorId, CreatePostRequestBody createPostRequestBody, Token token)
        {
            return CreatePostInternal($"/users/{authorId}/posts", createPostRequestBody, token);
        }

        public Post CreatePostUnderPublication(string publicationId, CreatePostRequestBody createPostRequestBody, Token token)
        {
            return CreatePostInternal($"/publications/{publicationId}/posts", createPostRequestBody, token);
        }

        private Post CreatePostInternal(string endpointUrl, CreatePostRequestBody createPostRequestBody, Token token)
        {
            var request = Helper.
                GetRequestWithToken(
                    BaseUrl + endpointUrl,
                    System.Net.Http.HttpMethod.Post,
                    token).
                SetRequestJson(new
                {
                    title = createPostRequestBody.Title,
                    contentFormat = createPostRequestBody.ContentFormat.ToString().ToLowerInvariant(),
                    content = createPostRequestBody.Content,
                    tags = createPostRequestBody.Tags,
                    canonicalUrl = createPostRequestBody.CanonicalUrl,
                    publishStatus = createPostRequestBody.PublishStatus.ToString().ToLowerInvariant(),
                    license = createPostRequestBody.License.ToString().CamelCaseToSpinalCase(),
                    publishedAt = createPostRequestBody.PublishedAt,
                    notifyFollowers = createPostRequestBody.NotifyFollowers,
                });

            return request.GetResponseJson<Post>();
        }

        public Image UploadImage(UploadImageRequestBody uploadImageRequestBody, Token token)
        {
            var request = Helper.
                GetRequestWithToken(
                    BaseUrl + "/images",
                    System.Net.Http.HttpMethod.Post,
                    token).
                SetRequestMultipartFormData(
                    uploadImageRequestBody.ContentType,
                    uploadImageRequestBody.ContentBytes);

            return request.GetResponseJson<Image>();
        }

    }
}
