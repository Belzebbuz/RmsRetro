using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using RmsRetro.MessageHub.Channels;
using RmsRetro.MessageHub.Protos.HubApi;
using RmsRetro.Protos.Api;

namespace RmsRetro.MessageHub.Grpc;

public class HubApiGrpcService(
	IMessageChannelWriter writer, 
	IMessageChannelReader reader, 
	ApiService.ApiServiceClient client) : HubApiService.HubApiServiceBase
{
	public override async Task<Empty> SendNotification(Notification request, ServerCallContext context)
	{
		await writer.WriteAsync(request.ChannelId, request.Message);
		return new Empty();
	}

	public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<NotificationEvent> responseStream, ServerCallContext context)
	{
		var userId = context.RequestHeaders.GetValue("x-player-id") ?? throw new UnauthorizedAccessException();
		var userInfo = await client.GetUserStatusAsync(new(), new Metadata()
		{
			{ "x-player-id", userId }
		});
		if (!userInfo.IsActive)
			throw new UnauthorizedAccessException();
		await foreach (var message in reader.ReadAsync(request.ChannelId))
		{
			await responseStream.WriteAsync(message);
		}
	}
}