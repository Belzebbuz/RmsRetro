using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class MoveCardCommand(MoveCardOperation operation, IAuthService authService, IOperationResolver resolver)
	: RoomCommandBaseHandler<MoveCardOperation>(operation, authService)
{
	protected override Task ExecuteCoreAsync(Room state)
	{
		var card = state.Cards[Guid.Parse(Operation.CardId)];
		var newColumn = state.Columns[Guid.Parse(Operation.NewColumnId)];
		var oldColumn = state.Columns[card.ColumnId];
		oldColumn.Cards.Remove(card);
		newColumn.Cards.Add(card);
		card.ColumnId = newColumn.Id;
		return Task.CompletedTask;
	}

	protected override bool CanHandle(Room state)
	{
		var card = state.Cards.GetValueOrDefault(Guid.Parse(Operation.CardId));
		if (card == null)
			throw DomainException.NotFound(Operation.CardId);
		var newColumn = state.Columns.GetValueOrDefault(Guid.Parse(Operation.NewColumnId));
		if (newColumn == null)
			throw DomainException.NotFound(Operation.NewColumnId);
		return resolver.GetCardOperations(AuthService.UserId, state, card).Contains(CardOperationTypes.MoveCard);
	}
}