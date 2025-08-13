using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using InsuranceSystem.Shared.Infrastructure.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProposalService.Tests.Shared;

public class ResilientHttpClientTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }

    private static ResilientHttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var handler = new StubHandler(responder);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
        return new ResilientHttpClient(http, NullLogger<ResilientHttpClient>.Instance);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnResponse()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var resp = await client.GetAsync("/ping");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostAsync_Raw_ShouldReturnResponse()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.Created));
        var resp = await client.PostAsync("/items", new StringContent("{}", Encoding.UTF8, "application/json"));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PutAsync_ShouldReturnResponse()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.Accepted));
        var resp = await client.PutAsync("/items/1", new StringContent("{}", Encoding.UTF8, "application/json"));
        resp.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnResponse()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var resp = await client.DeleteAsync("/items/1");
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private sealed record Item(string Name);

    [Fact]
    public async Task GetAsync_Generic_Success_ShouldDeserialize()
    {
        var json = "{\"name\":\"abc\"}";
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var item = await client.GetAsync<Item>("/items/1");
        item.Should().NotBeNull();
        item!.Name.Should().Be("abc");
    }

    [Fact]
    public async Task GetAsync_Generic_Failure_ShouldReturnDefault()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
        var item = await client.GetAsync<Item>("/items/1");
        item.Should().BeNull();
    }

    [Fact]
    public async Task PostAsync_Generic_Success_ShouldDeserialize()
    {
        var json = "{\"name\":\"xyz\"}";
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var item = await client.PostAsync<Item>("/items", new { name = "xyz" });
        item.Should().NotBeNull();
        item!.Name.Should().Be("xyz");
    }

    [Fact]
    public async Task PostAsync_Generic_Failure_ShouldReturnDefault()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.BadGateway));
        var item = await client.PostAsync<Item>("/items", new { name = "xyz" });
        item.Should().BeNull();
    }
} 