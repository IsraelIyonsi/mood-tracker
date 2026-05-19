namespace MoodTracker.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoodTracker.Api.Common.Constants;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.GetRecentMoods;
using MoodTracker.Api.Tests.Builders;

public class GetRecentMoodsEndpointTests : IAsyncLifetime
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
    public async Task Get_ReturnsEmpty_WhenNoEntries()
    {
        var response = await _client.GetAsync(ApiRoutes.Moods);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<GetRecentMoodsResponse>(JsonOptions);
        dto.ShouldNotBeNull();
        dto!.Entries.ShouldBeEmpty();
        dto.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Get_DefaultTake_ReturnsConfiguredDefault()
    {
        await SeedAsync(10);

        var response = await _client.GetAsync(ApiRoutes.Moods);
        var dto = await response.Content.ReadFromJsonAsync<GetRecentMoodsResponse>(JsonOptions);

        dto!.Count.ShouldBe(7);
    }

    [Fact]
    public async Task Get_HonorsTakeQueryParameter()
    {
        await SeedAsync(10);

        var response = await _client.GetAsync($"{ApiRoutes.Moods}?take=3");
        var dto = await response.Content.ReadFromJsonAsync<GetRecentMoodsResponse>(JsonOptions);

        dto!.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Get_OrdersByLoggedAtDescending()
    {
        var oldest  = new DateTimeOffset(2026, 5, 17, 0, 0, 0, TimeSpan.Zero);
        var middle  = new DateTimeOffset(2026, 5, 18, 0, 0, 0, TimeSpan.Zero);
        var newest  = new DateTimeOffset(2026, 5, 19, 0, 0, 0, TimeSpan.Zero);
        await SeedExplicitAsync(
            A.MoodEntry().WithMood(Mood.Happy).WithLoggedAt(oldest),
            A.MoodEntry().WithMood(Mood.Sad).WithLoggedAt(newest),
            A.MoodEntry().WithMood(Mood.Neutral).WithLoggedAt(middle));

        var response = await _client.GetAsync(ApiRoutes.Moods);
        var dto = await response.Content.ReadFromJsonAsync<GetRecentMoodsResponse>(JsonOptions);

        dto!.Entries[0].Mood.ShouldBe(Mood.Sad);
        dto.Entries[1].Mood.ShouldBe(Mood.Neutral);
        dto.Entries[2].Mood.ShouldBe(Mood.Happy);
    }

    private async Task SeedAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoodDbContext>();
        for (var i = 0; i < count; i++)
        {
            db.MoodEntries.Add(A.MoodEntry().WithLoggedAt(DateTimeOffset.UtcNow.AddHours(-i)).Build());
        }
        await db.SaveChangesAsync();
    }

    private async Task SeedExplicitAsync(params MoodEntryBuilder[] builders)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoodDbContext>();
        foreach (var builder in builders)
        {
            db.MoodEntries.Add(builder.Build());
        }
        await db.SaveChangesAsync();
    }
}
