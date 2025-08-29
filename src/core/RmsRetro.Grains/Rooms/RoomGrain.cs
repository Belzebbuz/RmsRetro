using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Orleans.Core;
using RmsRetro.Abstractions.Rooms;
using RmsRetro.Common.Extensions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Consts;
using RmsRetro.Grains.Gateways;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Grains.State;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms;

[AuthorizeGrain]
public class RoomGrain(
	[PersistentState("room-state", StorageConstants.RedisStorage)]
	IPersistentState<Room> state,
	ILogger<RoomGrain> logger,
	IAuthService authService,
	INotificationHubGateway gateway,
	IRoomOperationHandler operationHandler,
	IRoomMapper mapper) : NotifiableStateGrain<Room>(state, gateway, logger), IRoomGrain
{
	private readonly IStorage<Room> _state = state;
	protected override string GetVersion() => _state.State.Version.ToString();

	[Init, SaveState]
	public Task<InitRoomResponse> InitAsync(InitRoomRequest request)
	{
		InitRoom();
		return new InitRoomResponse()
		{
			RoomId = this.GetPrimaryKey().ToString(),
		}.AsTask();
	}

	[SaveState]
	public Task<Empty> ConnectAsync()
	{
		if(_state.State.Users.Any(x => x.Id == authService.UserId))
			return new Empty().AsTask();
		_state.State.Users.Add(new User
		{
			Id = authService.UserId
		});
		logger.LogInformation("Пользователь присоединился к комнате. {_state.State.Id}", _state.State.Id);
		return new Empty().AsTask();
	}

	[SaveState]
	public async Task<Empty> HandleOperation(InvokeRoomOperationRequest request)
	{
		await operationHandler.HandleAsync(_state.State, request);
		return new Empty();
	}

	public Task<GetRoomInfoResponse> GetAsync() 
		=> mapper.ToInfo(_state.State).AsTask();

	private void InitRoom()
	{
		_state.State = new Room()
		{
			Id = this.GetPrimaryKey(),
		};
		_state.State.Owners.Add(this.GetPrimaryKey().ToString());
		_state.State.Owners.Add(authService.UserId);
		_state.State.Users.Add(new User
		{
			Id = authService.UserId
		});
		var column1 = new Column()
		{
			Id = Guid.NewGuid(),
			Name = "Went Well",
			Color = "positive",
			OrderId = 1
		};
		var column2 = new Column
		{
			Id = Guid.NewGuid(),
			Name = "To Improve",
			OrderId = 2,
			Color = "negative"
		};
		var column3 = new Column
		{
			Id = Guid.NewGuid(),
			Name = "Action Items",
			OrderId = 3,
			Color = "info"
		};
		_state.State.Columns.Add(column1.Id, column1);
		_state.State.Columns.Add(column2.Id, column2);
		_state.State.Columns.Add(column3.Id, column3);
	}
}