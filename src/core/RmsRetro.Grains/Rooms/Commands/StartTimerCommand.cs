using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Grains.Timers;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class StartTimerCommand (StartTimerOperation operation, IAuthService authService, IGrainFactory factory, IOperationResolver resolver)
	: RoomCommandBaseHandler<StartTimerOperation>(operation, authService)
{
	protected override async Task ExecuteCoreAsync(Room state)
	{
		var timerGrain = factory.GetGrain<IRoomTimerGrain>(state.Id);
		await timerGrain.StartAsync(Operation.Minutes);
	}

	protected override bool CanHandle(Room state)
	{
		return  resolver.GetRoomOperations(AuthService.UserId, state).Contains(RoomOperationTypes.StartTimer);
	}
}