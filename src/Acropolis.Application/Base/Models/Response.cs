using System.Collections.ObjectModel;
using Acropolis.Application.Base.Notifications;

namespace Acropolis.Application.Base.Models;

public class Response
{
    // Fields

    private readonly List<Notification> _notifications = new List<Notification>();

    // Constructors

    public Response()
    {
    }

    public Response(IEnumerable<Notification> notificaions)
    {
        AddNotifications(notificaions);
    }

    public Response(string code, string description)
    {
        AddNotification(code, description);
    }

    public Response(Notification notification)
    {
        AddNotification(notification);
    }

    // Properties

    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    // Methods

    public void AddNotification(string code, string description) =>
        _notifications.Add(new Notification(code, description));

    public void AddNotification(Notification notification) =>
        _notifications.Add(notification);

    public void AddNotifications(IEnumerable<Notification> notifications) =>
        _notifications.AddRange(notifications);

    public ResponseErrors ToValidationErrors() =>
        new ResponseErrors("VALIDATION_ERRORS", _notifications.AsReadOnly());

    public bool IsValid() => !_notifications.Any();


    // Static Methods

    public static Response Valid() => new Response();
    public static Response Invalid(IEnumerable<Notification> notifications) => new Response(notifications);
    public static Response Invalid(Notification notification) => new Response(notification);
    public static Response Invalid(string code, string description) => new Response(code, description);
}

public class ResponseErrors
{

    public ResponseErrors(string type, IReadOnlyCollection<Notification> notifications)
    {
        Type = type;
        Notifications = notifications;
    }

    public string Type { get; private set; }
    public IReadOnlyCollection<Notification> Notifications { get; private set; }



}