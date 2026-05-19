namespace MoodTracker.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoodTracker.Api.Common.Constants;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.LogMood;

public class LogMoodEndpointTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    private SqliteConnection _connection = default!;
    private MoodApiFactory _factory = default!;
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _factory = new MoodApiFactory(_connection);
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoodDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Post_ValidRequest_Returns201_WithLocation_AndCorrelationHeader()
    {
        var body = new { mood = "happy", note = "great day" };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Contains(HttpHeaders.CorrelationId).ShouldBeTrue();
    }

    [Fact]
    public async Task Post_ValidRequest_ReturnsExpectedShape()
    {
        var body = new { mood = "happy", note = "great day" };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);
        var dto = await response.Content.ReadFromJsonAsync<LogMoodResponse>(JsonOptions);

        dto.ShouldNotBeNull();
        dto!.Id.ShouldNotBe(Guid.Empty);
        dto.Mood.ShouldBe(Mood.Happy);
        dto.Note.ShouldBe("great day");
        dto.LoggedAt.ShouldNotBe(default);
        dto.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task Post_UnknownMoodString_Returns400_AtDeserialization()
    {
        var body = new { mood = "rainbow" };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_OutOfRangeMoodInt_Returns422_AtValidator()
    {
        var body = new { mood = 999, note = "x" };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.ShouldNotBeNull();
        problem!.Errors.Keys.ShouldContain(nameof(LogMoodRequest.Mood));
    }

    [Fact]
    public async Task Post_NoteOverMaxLength_Returns422()
    {
        var body = new { mood = "happy", note = new string('x', 281) };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Post_MalformedJson_Returns400()
    {
        using var content = new StringContent("{not json", Encoding.UTF8, MediaTypes.Json);

        var response = await _client.PostAsync(ApiRoutes.Moods, content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_EchoesCorrelationId_WhenProvided()
    {
        var requestedCorrelationId = Guid.NewGuid().ToString();
        using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Moods)
        {
            Content = JsonContent.Create(new { mood = "happy" }),
        };
        request.Headers.Add(HttpHeaders.CorrelationId, requestedCorrelationId);

        var response = await _client.SendAsync(request);

        response.Headers.GetValues(HttpHeaders.CorrelationId).ShouldContain(requestedCorrelationId);
    }

    [Fact]
    public async Task Post_GeneratesCorrelationId_WhenNotProvided()
    {
        var body = new { mood = "happy" };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Moods, body);

        var ids = response.Headers.GetValues(HttpHeaders.CorrelationId).ToList();
        ids.ShouldNotBeEmpty();
        ids[0].ShouldNotBeNullOrWhiteSpace();
    }
}
