using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Grains.Timers;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class StartVotingCommand(StartVotingOperation operation, IAuthService authService, IGrainFactory factory, IOperationResolver resolver)
	: RoomCommandBaseHandler<StartVotingOperation>(operation, authService)
{
	protected override async Task ExecuteCoreAsync(Room state)
	{
		state.IsVoteStarted = true;
		state.Users.ForEach(x => x.VotesCount = Operation.VotesPerUser);
		if (Operation.TimerMinutes > 0)
		{
			var timerGrain = factory.GetGrain<IRoomTimerGrain>(state.Id);
			await timerGrain.StartVoteAsync(Operation.TimerMinutes);
		}
	}

	protected override bool CanHandle(Room state)
	{
		return  resolver.GetRoomOperations(AuthService.UserId, state).Contains(RoomOperationTypes.PauseTimer);
	}
}