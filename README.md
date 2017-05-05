# Medium SDK for .NET

This repository contains the open source SDK for integrating [Medium's OAuth2 API](https://github.com/Medium/medium-api-docs) into your .NET app.

[![Build status](https://ci.appveyor.com/api/projects/status/u1lmowf82gdckxmo?svg=true)](https://ci.appveyor.com/project/adriangodong/medium-sdk-dotnet)
[![NuGet](https://img.shields.io/nuget/v/MediumSdk.svg)](https://www.nuget.org/packages/MediumSdk/)

Install
-------

Use Nuget and search for '[MediumSdk](https://www.nuget.org/packages/MediumSdk/)'.

Usage
-----

If you are onboarding a new user, follow the OAuth workflow to get an access token.
Create an instance of OAuthClient to perform this function.

    // Contact developers@medium.com to get your client ID and client secret.
    var oAuthClient = new Medium.OAuthClient("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET");

    // Build the URL where you can send the user to obtain an authorization code.
    var url = oAuthClient.GetAuthorizeUrl(
        "secretstate",
        "https://yoursite.com/callback/medium",
        new[]
        {
            Medium.Authentication.Scope.BasicProfile,
            Medium.Authentication.Scope.PublishPost
        });

    // (Send the user to the authorization URL to obtain an authorization code.)

    // Exchange the authorization code for an access token.
    var accessToken = oAuthClient.GetAccessToken("YOUR_AUTHORIZATION_CODE", "https://yoursite.com/callback/medium");

    // When your access token expires, use the refresh token to get a new one.
    var newAccessToken = oAuthClient.GetAccessToken(accessToken.RefreshToken);

Once you have this access token, you may store it and reuse it in the future.
To call Medium API, create an instance of Client and call the methods.

    var client = new Medium.Client();

    // Get profile details of the user identified by the access token.
    var user = client.GetCurrentUser(accessToken);

    // Create a draft post.
    var post = client.CreatePost(user.Id, new Medium.Models.CreatePostRequestBody
    {
        Title =         "Title",
        ContentFormat = Medium.Models.ContentFormat.Html,
        Tags =          new[] { "title", "content", "tag" },
        Content =       "<h2>Title</h2><p>Content</p>",
        PublishStatus = Medium.Models.PublishStatus.Draft,
    })

Contributing
------------

Questions, comments, bug reports, and pull requests are all welcomed.

Authors
-------

[Adrian Godong](https://github.com/adriangodong)

License
-------

Licensed under [The MIT License](https://github.com/adriangodong/medium-sdk-dotnet/blob/master/LICENSE).
