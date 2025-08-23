using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RmsRetro.Abstractions.Auth;
using RmsRetro.Abstractions.Rooms;
using RmsRetro.Common.Extensions;
using RmsRetro.Common.OrleansKeys;
using RmsRetro.Common.Templates;
using RmsRetro.Protos.Api;

namespace RmsRetro.Api.Grpc;

public class ApiGrpcService(IClusterClient client) : ApiService.ApiServiceBase
{
	public override Task<InitRoomResponse> InitRoom(InitRoomRequest request, ServerCallContext context)
		=> client.GetGrain<IRoomGrain>(Guid.NewGuid()).InitAsync(request);

	public override Task<Empty> Connect(ConnectRequest request, ServerCallContext context)
		=> client.GetGrain<IRoomGrain>(Guid.Parse(request.RoomId)).ConnectAsync();

	public override Task<GetRoomTemplatesResponse> GetRoomTemplates(Empty request, ServerCallContext context)
	{
		return new GetRoomTemplatesResponse()
		{
			 TemplateIds = { RoomTemplates.CommonTemplate }
		}.AsTask();
	}

	public override Task<Empty> InvokeRoomOperation(InvokeRoomOperationRequest request, ServerCallContext context)
		=> client.GetGrain<IRoomGrain>(Guid.Parse(request.RoomId)).HandleOperation(request);

	public override Task<GetRoomInfoResponse> GetRoomInfo(GetRoomInfoRequest request, ServerCallContext context)
		=> client.GetGrain<IRoomGrain>(Guid.Parse(request.RoomId)).GetAsync();

	[AllowAnonymous]
	public override async Task<ActivateUserResponse> ActivateUser(Empty request, ServerCallContext context)
	{
		if (RequestContext.Get(RequestKeys.UserId) is string existsKey)
		{
			var status = await client.GetGrain<IUserGrain>(Guid.Parse(existsKey)).GetStatusAsync();
			if (status.IsActive)
				return new ActivateUserResponse()
				{
					UserId = existsKey
				};
		}
		return await client.GetGrain<IUserGrain>(Guid.NewGuid()).ActivateAsync();
	}

	public override Task<GetUserStatusResponse> GetUserStatus(Empty request, ServerCallContext context)
		=> client.GetGrain<IUserGrain>(Guid.Parse(RequestContext.Get(RequestKeys.UserId) as string ?? throw new UnauthorizedAccessException()))
			.GetStatusAsync();
}