namespace IsolatedFunction.Models.OrderFunction;

public class TimerTriggerInfo
{
    public ScheduleStatus ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}

public class ScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}