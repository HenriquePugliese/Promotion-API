namespace Acropolis.Application.Base.Notifications;

public class Notification
{
    public Notification(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public string Code { get; }
    public string Description { get; }

    public override string ToString()
    {
        return $"{Code}: {Description}";
    }
}
