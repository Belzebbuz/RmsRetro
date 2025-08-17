using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Grains.Timers;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class StopVotingCommand(
	StopVotingOperation operation, 
	IAuthService authService, 
	IGrainFactory factory, 
	IOperationResolver resolver)
	: RoomCommandBaseHandler<StopVotingOperation>(operation, authService)
{
	protected override async Task ExecuteCoreAsync(Room state)
	{
		state.IsVoteStarted = false;
		await factory.GetGrain<IRoomTimerGrain>(state.Id).StopAsync();
	}

	protected override bool CanHandle(Room state)
	{
		return resolver.GetRoomOperations(AuthService.UserId, state).Contains(RoomOperationTypes.StopVoting);
	}
}