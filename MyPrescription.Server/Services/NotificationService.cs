using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyPrescription.Common.Models;
using MyPrescription.Data.Entity;
using MyPrescription.Data.Repository;
using MyPrescription.Server.Settings;
using WebPush;

namespace MyPrescription.Server.Services;

public class NotificationService
{
    private readonly NotificationRepository repository;
    private readonly NotificationSettings settings;

    public NotificationService(NotificationRepository repository, IOptions<NotificationSettings> notificationSettings)
    {
        this.repository = repository;
        settings = notificationSettings.Value;
    }

    public string GetPublicKey() => settings.PublicKey;

    public async Task SubscribePatientAsync(Guid patientId, NotificationSubscription notification)
    {
        var notificationDb = new Notification()
        {
            IDUser = patientId.ToString(),
            Auth = notification.Auth,
            P256dh = notification.P256dh,
            Url = notification.Url,
            CreationDate = DateTime.Now
        };
        await repository.AddNotificationSubscriptionAsync(notificationDb);
    }

    public Task NotificationNewPrescription(string patientId) => FetchSubscriptions(patientId, "You have a new prescription");

    public Task NotificationPrescriptionDelivered(string patientId) => FetchSubscriptions(patientId, "Prescription delivered");

    private async Task FetchSubscriptions(string patientId, string message)
    {
        var notificationSubscriptions = await repository.GetNotificationSubscriptionAsync(patientId);
        var subscriptionsTask = notificationSubscriptions.Select(x => new NotificationSubscription()
        {
            Auth = x.Auth,
            P256dh = x.P256dh,
            Url = x.Url
        }).Select(x => SendNotificationAsync(x, message, "myprescriptions"));

        await Task.WhenAll(subscriptionsTask);
    }

    private async Task SendNotificationAsync(NotificationSubscription subscription, string message, string url)
    {
        //https://vapidkeys.com/
        var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
        var vapidDetails = new VapidDetails(settings.Mail, settings.PublicKey, settings.PrivateKey);
        var webPushClient = new WebPushClient();
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                message,
                url
            });
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
        }
        catch (WebPushException e)
        {
            var statusCode = e.StatusCode;
            if (statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.Gone)
            {
                // Subscription no longer valid
                await repository.RemoveNotificationSubscriptionByUrlAsync(e.PushSubscription.Endpoint);
            }
        }
    }
}

