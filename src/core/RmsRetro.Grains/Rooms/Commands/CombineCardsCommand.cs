using System.Text;
using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class CombineCardsCommand(CombineCardsOperation operation, IAuthService authService, IOperationResolver resolver)
	: RoomCommandBaseHandler<CombineCardsOperation>(operation, authService)
{
	protected override Task ExecuteCoreAsync(Room state)
	{
		var newText = new StringBuilder();
		var deleteCard = state.Cards[Guid.Parse(Operation.DeleteCardId)];
		var targetCard = state.Cards[Guid.Parse(Operation.TargetCardId)];

		targetCard.UserId = AuthService.UserId;
		state.Columns[deleteCard.ColumnId].Cards.Remove(deleteCard);

		newText.AppendLine(targetCard.Text);
		newText.AppendLine("--------------");
		newText.Append(deleteCard.Text);
		targetCard.Text = newText.ToString();
	
		foreach (var userId in deleteCard.UsersLiked)
		{
			var userAdded = targetCard.UsersLiked.Add(userId);
			if (state.IsVoteStarted)
			{
				var user = state.Users.First(x => x.Id == userId);
				if(!userAdded)
					user.VotesCount++;
			}
		}
		return Task.CompletedTask;
	}

	protected override bool CanHandle(Room state)
	{
		var deleteCard = state.Cards.GetValueOrDefault(Guid.Parse(Operation.DeleteCardId));
		var targetCard = state.Cards.GetValueOrDefault(Guid.Parse(Operation.TargetCardId));
		if (deleteCard == null)
			throw DomainException.NotFound(Operation.DeleteCardId);
		if (targetCard == null)
			throw DomainException.NotFound(Operation.TargetCardId);
		return resolver.GetCardOperations(AuthService.UserId, state, deleteCard).Contains(CardOperationTypes.CombineCards);
	}
}