using System.Text.Json;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flowtap_Infrastructure.Persistence.Configurations.Identity;

public class NotificationQueueConfiguration : IEntityTypeConfiguration<NotificationQueue>
{
    public void Configure(EntityTypeBuilder<NotificationQueue> b)
    {
        b.HasKey(x => x.Id);

        // Postgres expects valid JSON in a jsonb column.
        // Use a ValueConverter so any plain string is serialised as a JSON
        // string token (e.g. "hi" → "\"hi\"") on write and deserialised back
        // transparently on read.
        var payloadConverter = new ValueConverter<string, string>(
            v  => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v  => JsonSerializer.Deserialize<string>(v, (JsonSerializerOptions?)null) ?? string.Empty
        );
        b.Property(x => x.Payload)
         .HasColumnType("jsonb")
         .HasConversion(payloadConverter);

        b.Property(x => x.Recipient).HasMaxLength(512);
        b.Property(x => x.Subject).HasMaxLength(512);
    }
}
