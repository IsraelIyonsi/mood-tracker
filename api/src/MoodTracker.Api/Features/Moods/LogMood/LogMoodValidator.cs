namespace MoodTracker.Api.Features.Moods.LogMood;

using FluentValidation;
using Microsoft.Extensions.Options;

internal sealed class LogMoodValidator : AbstractValidator<LogMoodRequest>
{
    public LogMoodValidator(TimeProvider clock, IOptions<MoodOptions> options)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(options);

        var moodOptions = options.Value;

        RuleFor(request => request.Mood)
            .IsInEnum()
            .WithMessage($"{nameof(LogMoodRequest.Mood)} must be one of the defined values.");

        RuleFor(request => request.Note)
            .MaximumLength(moodOptions.NoteMaxLength)
            .When(request => request.Note is not null)
            .WithMessage(_ =>
                $"{nameof(LogMoodRequest.Note)} cannot exceed {moodOptions.NoteMaxLength} characters.");

        RuleFor(request => request.LoggedAt!.Value)
            .LessThanOrEqualTo(_ => clock.GetUtcNow().Add(moodOptions.ClockSkewTolerance))
            .WithMessage($"{nameof(LogMoodRequest.LoggedAt)} cannot be in the future.")
            .OverridePropertyName(nameof(LogMoodRequest.LoggedAt))
            .When(request => request.LoggedAt is not null);

        RuleFor(request => request.LoggedAt!.Value)
            .GreaterThanOrEqualTo(_ => clock.GetUtcNow().Subtract(moodOptions.MaxBackdateWindow))
            .WithMessage(_ =>
                $"{nameof(LogMoodRequest.LoggedAt)} cannot be older than {moodOptions.MaxBackdateWindow.TotalDays} days.")
            .OverridePropertyName(nameof(LogMoodRequest.LoggedAt))
            .When(request => request.LoggedAt is not null);
    }
}
