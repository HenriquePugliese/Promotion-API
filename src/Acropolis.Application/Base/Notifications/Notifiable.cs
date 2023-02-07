using FluentValidation;

namespace Acropolis.Application.Base.Notifications;

public abstract class Notifiable
{
    // Fields

    private readonly List<Notification> _notifications = new List<Notification>();

    // Abstract Methods

    public abstract bool IsValid();

    // Public Methods

    public bool HasNotifications =>
        _notifications.Any();

    public IEnumerable<Notification> GetNotifications() =>
        _notifications.AsReadOnly();

    public void AddNotification(string code, string description) =>
        _notifications.Add(new Notification(code, description));

    // Protected Methods

    protected bool Validate<T>(IValidator<T> validator)
    {
        var context = new ValidationContext<object>(this);
        var result = validator.Validate(context);

        foreach (var item in result.Errors)
            AddNotification(item.PropertyName, item.ErrorMessage);

        return !HasNotifications;
    }
}
