using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class AddLikeCardCommand(AddLikeCardOperation operation, IAuthService authService, IOperationResolver resolver)
	: RoomCommandBaseHandler<AddLikeCardOperation>(operation, authService)
{
	protected override Task ExecuteCoreAsync(Room state)
	{
		var user = state.Users.First(x => x.Id == AuthService.UserId);
		user.VotesCount--;
		var card = state.Cards[Guid.Parse(Operation.CardId)];
		card.UsersLiked.Add(AuthService.UserId);
		return Task.CompletedTask;
	}

	protected override bool CanHandle(Room state)
	{
		var card = state.Cards.GetValueOrDefault(Guid.Parse(Operation.CardId));
		if (card == null)
			throw DomainException.NotFound(Operation.CardId);
		var operations = resolver.GetCardOperations(AuthService.UserId, state, card);
		return operations.Contains(CardOperationTypes.AddLikeCard);
	}
}