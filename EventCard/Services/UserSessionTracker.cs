public class UserSessionTracker
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime SessionStartTime { get; } = DateTime.Now;

    public List<int> RegisteredEvents { get; } = new();

    public bool IsRegistered(int eventId)
    {
        return RegisteredEvents.Contains(eventId);
    }

    public void RegisterEvent(int eventId)
    {
        if (!RegisteredEvents.Contains(eventId))
        {
            RegisteredEvents.Add(eventId);
        }
    }
}