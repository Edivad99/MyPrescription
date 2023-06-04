namespace MyPrescription.Common.Models;

public class NotificationSubscription
{
    public string Url { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}
