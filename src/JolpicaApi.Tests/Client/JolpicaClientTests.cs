﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JolpicaApi.Abstractions;
using JolpicaApi.Client;
using JolpicaApi.Client.Caching;
using JolpicaApi.Requests;
using JolpicaApi.Responses;
using NSubstitute;
using Xunit;

namespace JolpicaApi.Tests.Client
{
    public class JolpiClientTests
    {
        private JolpiClient Client { get; }

        private IJolpiCache Cache { get; }

        private IUrlBuilder UrlBuilder { get; }

        private IHttpClient HttpClient { get; }

        private HttpResponseMessage ResponseMessage { get; }

        private JolpiRequest<JolpiResponse> NullRequest { get; } = null;

        public JolpiClientTests()
        {
            Cache = Substitute.For<IJolpiCache>();
            UrlBuilder = Substitute.For<IUrlBuilder>();
            HttpClient = Substitute.For<IHttpClient>();
            ResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("{ Data: {} }")
            };

            HttpClient.GetAsync(null).ReturnsForAnyArgs(x => ResponseMessage);

            Client = new JolpiClient
            {
                Cache = Cache,
                UrlBuilder = UrlBuilder,
                HttpClient = HttpClient
            };
        }

        [Fact]
        public void Constructor_WithApiBase_SetsApiBase()
        {
            var apiRoot = "http://example.com";
            var client = new JolpiClient(apiRoot);
            client.ApiBase.Should().Be(apiRoot);
        }

        [Theory]
        [AutoMockedData("invalid string")]
        [AutoMockedData("C:\\")]
        [AutoMockedData("C:")]
        [AutoMockedData("ftp://example.com")]
        [AutoMockedData("ftp://example.com/")]
        [AutoMockedData("C:\\example.txt")]
        [AutoMockedData("/example/api")]
        [AutoMockedData("example/api")]
        public void ApiBase_Set_NonUrlShouldThrowArgumentException(string url)
        {
            Action act = () => Client.ApiBase = url;
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [AutoMockedData("http://example.com")]
        [AutoMockedData("https://example.com")]
        public void ApiBase_Set_ShouldAcceptHttpAndHttpsUrls(string url)
        {
            Client.ApiBase = url;
            Client.ApiBase.Should().Be(url);
        }

        [Theory]
        [AutoMockedData("http://example.com/api/")]
        [AutoMockedData("https://example.com/")]
        public void ApiBase_Set_ShouldRemoveTrailingSlash(string url)
        {
            Client.ApiBase = url;
            Client.ApiBase.Should().Be(url.TrimEnd('/'));
        }

        [Theory]
        [AutoMockedData]
        public void GetResponseAsync_RequestWithRoundWithoutSeason_ThrowsInvalidOperationException(JolpiRequest<JolpiResponse> request)
        {
            // Arrange
            request.Season = null;
            request.Round = "1";

            // Act
            Func<Task> act = async () => await Client.GetResponseAsync(request);

            // Assert
            act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [AutoMockedData]
        public async Task GetResponseAsync_ReturnsCachedResponse(JolpiResponse expectedResponse)
        {
            Cache.Get<JolpiResponse>(null).ReturnsForAnyArgs(expectedResponse);

            var response = await Client.GetResponseAsync(NullRequest);

            response.Should().Be(expectedResponse);
        }

        [Theory]
        [AutoMockedData]
        public async Task GetResponseAsync_CallsTheUrlFromUrlBuilder(string url)
        {
            // Arrange
            var expectedUrl = Client.ApiBase + url;
            UrlBuilder.Build(NullRequest).Returns(url);

            // Act
            await Client.GetResponseAsync(NullRequest);

            // Assert
            await HttpClient.Received().GetAsync(expectedUrl);
        }

        [Theory]
        [AutoMockedData]
        public async Task GetResponseAsync_AddsTheResponseToTheCache(JolpiRequest<JolpiResponse> request, string url)
        {
            // Arrange
            var expectedUrl = Client.ApiBase + url;
            UrlBuilder.Build(request).Returns(url);

            // Act
            await Client.GetResponseAsync(request);

            // Assert
            Cache.Received().AddOrReplace(expectedUrl, Arg.Any<JolpiResponse>());
        }

        [Theory]
        [AutoMockedData(HttpStatusCode.BadRequest)]
        [AutoMockedData(HttpStatusCode.ServiceUnavailable)]
        [AutoMockedData(HttpStatusCode.InternalServerError)]
        [AutoMockedData(HttpStatusCode.Forbidden)]
        [AutoMockedData(HttpStatusCode.Unauthorized)]
        [AutoMockedData(HttpStatusCode.NotFound)]
        public void GetResponseAsync_ThrowsHttpRequestExceptionIfNotSuccessStatusCode(HttpStatusCode statusCode)
        {
            ResponseMessage.StatusCode = statusCode;

            Func<Task> act = async () => await Client.GetResponseAsync(NullRequest);

            act.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public void GetResponseAsync_ThrowsExceptionIfInvalidResponse()
        {
            ResponseMessage.Content = new StringContent("");

            Func<Task> act = async () => await Client.GetResponseAsync(NullRequest);

            act.Should().ThrowExactlyAsync<Exception>();
        }

        [Fact]
        public void Dispose_DisposesOfHttpClient()
        {
            Client.Dispose();
            HttpClient.Received().Dispose();
        }

        [Fact]
        public void Dispose_DisposesOfCache()
        {
            Client.Dispose();
            Cache.Received().Dispose();
        }
    }
}
