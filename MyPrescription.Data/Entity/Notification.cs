namespace MyPrescription.Data.Entity;

public class Notification
{
    public string IDUser { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}
