namespace MoodTracker.Api.Common.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodTracker.Api.Features.Moods;

internal sealed class MoodEntryConfiguration : IEntityTypeConfiguration<MoodEntry>
{
    public void Configure(EntityTypeBuilder<MoodEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(Tables.MoodEntries);
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Mood)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.MoodEnum)
            .IsRequired();

        builder.Property(entry => entry.Note);
        builder.Property(entry => entry.LoggedAt).IsRequired();
        builder.Property(entry => entry.CreatedAt).IsRequired();
        builder.Property(entry => entry.UpdatedAt);

        builder.HasIndex(entry => entry.LoggedAt)
            .HasDatabaseName(Indexes.MoodEntriesLoggedAt);
    }
}
