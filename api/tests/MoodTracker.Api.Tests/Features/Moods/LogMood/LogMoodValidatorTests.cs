namespace MoodTracker.Api.Tests.Features.Moods.LogMood;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.LogMood;
using MoodTracker.Api.Tests.Builders;
using MoodTracker.Api.Tests.Scenarios;

public class LogMoodValidatorTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 5, 19, 12, 0, 0, TimeSpan.Zero);

    private static readonly MoodOptions DefaultMoodOptions = new()
    {
        NoteMaxLength = 280,
        RecentDefaultTake = 7,
        RecentMaxTake = 30,
        ClockSkewTolerance = TimeSpan.FromSeconds(30),
        MaxBackdateWindow = TimeSpan.FromDays(30),
    };

    private readonly LogMoodValidator _validator;

    public LogMoodValidatorTests()
    {
        var clock = new FakeTimeProvider(Now);
        var options = Options.Create(DefaultMoodOptions);
        _validator = new LogMoodValidator(clock, options);
    }

    [Theory]
    [MemberData(nameof(ValidScenarios))]
    public async Task Validate_AcceptsRequest(ValidationScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        var result = await _validator.ValidateAsync(scenario.Request);

        result.IsValid.ShouldBeTrue(
            $"{scenario.Description}: {string.Join(", ", result.Errors.Select(error => error.ErrorMessage))}");
    }

    [Theory]
    [MemberData(nameof(InvalidScenarios))]
    public async Task Validate_RejectsRequest_WithExpectedFieldError(ValidationScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        var result = await _validator.ValidateAsync(scenario.Request);

        result.IsValid.ShouldBeFalse(scenario.Description);
        result.Errors.ShouldContain(
            error => error.PropertyName == scenario.ExpectedErrorProperty,
            scenario.Description);
    }

    public static TheoryData<ValidationScenario> ValidScenarios() => new()
    {
        new("happy mood",                       A.LogMoodRequest().WithMood(Mood.Happy)),
        new("excited mood",                     A.LogMoodRequest().WithMood(Mood.Excited)),
        new("neutral mood",                     A.LogMoodRequest().WithMood(Mood.Neutral)),
        new("anxious mood",                     A.LogMoodRequest().WithMood(Mood.Anxious)),
        new("sad mood",                         A.LogMoodRequest().WithMood(Mood.Sad)),
        new("null note",                        A.LogMoodRequest().WithNote(null)),
        new("empty note",                       A.LogMoodRequest().WithNote(string.Empty)),
        new("note at max length boundary",      A.LogMoodRequest().WithNoteOfLength(DefaultMoodOptions.NoteMaxLength)),
        new("loggedAt at clock-skew edge",      A.LogMoodRequest().WithLoggedAt(Now.Add(DefaultMoodOptions.ClockSkewTolerance))),
        new("backdated at limit boundary",      A.LogMoodRequest().WithLoggedAt(Now.Subtract(DefaultMoodOptions.MaxBackdateWindow))),
        new("null loggedAt defaults to server", A.LogMoodRequest().WithLoggedAt(null)),
    };

    public static TheoryData<ValidationScenario> InvalidScenarios() => new()
    {
        new("undefined mood enum value",
            A.LogMoodRequest().WithMood((Mood)999),
            ExpectedErrorProperty: nameof(LogMoodRequest.Mood)),

        new("note one char over max",
            A.LogMoodRequest().WithNoteOfLength(DefaultMoodOptions.NoteMaxLength + 1),
            ExpectedErrorProperty: nameof(LogMoodRequest.Note)),

        new("loggedAt one second past clock-skew",
            A.LogMoodRequest().WithLoggedAt(Now.Add(DefaultMoodOptions.ClockSkewTolerance).AddSeconds(1)),
            ExpectedErrorProperty: nameof(LogMoodRequest.LoggedAt)),

        new("backdated one second past limit",
            A.LogMoodRequest().WithLoggedAt(Now.Subtract(DefaultMoodOptions.MaxBackdateWindow).AddSeconds(-1)),
            ExpectedErrorProperty: nameof(LogMoodRequest.LoggedAt)),
    };
}
