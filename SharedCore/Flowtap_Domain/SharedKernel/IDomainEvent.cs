namespace Flowtap_Domain.SharedKernel;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
