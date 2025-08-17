using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class EditCardCommand(EditCardOperation operation, IAuthService authService, IOperationResolver resolver)
	: RoomCommandBaseHandler<EditCardOperation>(operation, authService)
{
	protected override Task ExecuteCoreAsync(Room state)
	{
		var card = state.Cards[Guid.Parse(Operation.CardId)];
		card.Text = Operation.Text;
		return Task.CompletedTask;
	}

	protected override bool CanHandle(Room state)
	{
		var card = state.Cards.GetValueOrDefault(Guid.Parse(Operation.CardId));
		if (card == null)
			throw DomainException.NotFound(Operation.CardId);
		return resolver.GetCardOperations(AuthService.UserId, state, card).Contains(CardOperationTypes.EditCard);
	}
}