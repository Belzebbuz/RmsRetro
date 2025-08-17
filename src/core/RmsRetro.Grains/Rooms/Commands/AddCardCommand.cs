using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.Protos.Api;
using TextCard = RmsRetro.Grains.Rooms.Models.TextCard;

namespace RmsRetro.Grains.Rooms.Commands;

public class AddCardCommand(AddCardOperation operation, IAuthService authService, IOperationResolver resolver)
	: RoomCommandBaseHandler<AddCardOperation>(operation, authService)
{
	protected override bool CanHandle(Room state) => resolver.GetRoomOperations(AuthService.UserId, state).Contains(RoomOperationTypes.AddCard);

	protected override Task ExecuteCoreAsync(Room state)
	{
		var column = state.Columns[Guid.Parse(Operation.ColumnId)];
		var orderId = column.Cards.Count == 0 ? 1 : column.Cards.Max(x => x.OrderId) + 1;
		var card = new TextCard
		{
			Id = Guid.NewGuid(),
			OrderId = orderId,
			Text = Operation.Text,
			UserId = AuthService.UserId,
			ColumnId = column.Id
		};
		column.Cards.Add(card);
		return Task.CompletedTask;
	}
}