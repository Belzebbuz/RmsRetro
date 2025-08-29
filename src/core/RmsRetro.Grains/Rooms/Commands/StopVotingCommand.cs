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
		state.VotesCount = 0;
		state.Users.ForEach(x => x.VotesCount = 0);
		state.IsVoteStarted = false;

		foreach (var column in state.Columns)
		{
			column.Value.Cards = column.Value.Cards.OrderByDescending(x => x.UsersLiked.Count).ToList();
			for (int i = 0; i < column.Value.Cards.Count; i++)
			{
				column.Value.Cards[i].OrderId = i;
			}
		}
		//Если останавливает комната, значит это вызывал таймер, который уже остановлен
		if(AuthService.UserId != state.Id.ToString())
			await factory.GetGrain<IRoomTimerGrain>(state.Id).StopAsync();
	}

	protected override bool CanHandle(Room state)
	{
		return resolver.GetRoomOperations(AuthService.UserId, state).Contains(RoomOperationTypes.StopVoting);
	}
}