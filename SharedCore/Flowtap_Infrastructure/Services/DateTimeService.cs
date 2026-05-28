using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
