using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.Grains.Gateways;

public interface INotificationHubGateway
{
	public Task NotifyAsync(string channelId, Message mesage);
}

public class NotificationHubGateway(HubApiService.HubApiServiceClient client) : INotificationHubGateway
{
	public async Task NotifyAsync(string channelId, Message message)
	{
		var request = new Notification
		{
			ChannelId = channelId,
			Message = message
		};
		await client.SendNotificationAsync(request);
	}
}

