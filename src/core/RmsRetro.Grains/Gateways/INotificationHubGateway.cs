using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.Grains.Gateways;

public interface INotificationHubGateway
{
	public Task NotifyAsync(string channelId, NotificationEvent message);
}

public class NotificationHubGateway(HubApiService.HubApiServiceClient client) : INotificationHubGateway
{
	public async Task NotifyAsync(string channelId, NotificationEvent message)
	{
		var request = new Notification
		{
			ChannelId = channelId,
			Message = message
		};
		await client.SendNotificationAsync(request);
	}
}

