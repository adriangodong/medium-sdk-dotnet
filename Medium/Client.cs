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

        public Post CreatePost(string authorId, CreatePostRequestBody createPostRequestBody, Token token)
        {
            var request = Helper.
                GetRequestWithToken(
                    BaseUrl + $"/users/{authorId}/posts",
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
                SetRequestMultipartFormData(new KeyValuePair<string, byte[]>(
                    uploadImageRequestBody.ContentType,
                    uploadImageRequestBody.ContentBytes));

            return request.GetResponseJson<Image>();
        }

    }
}
