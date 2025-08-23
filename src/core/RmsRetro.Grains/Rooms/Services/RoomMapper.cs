using Microsoft.Extensions.DependencyInjection;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;
using TextCard = RmsRetro.Protos.Api.TextCard;

namespace RmsRetro.Grains.Rooms.Services;

public class RoomMapper(IAuthService authService, IOperationResolver resolver) : IRoomMapper
{
	public GetRoomInfoResponse ToInfo(Room room)
	{
		var columns = room.Columns.Values
			.OrderBy(x => x.OrderId)
			.Select(c => MapColumn(room, c));

		return new GetRoomInfoResponse
		{
			Info = new RoomInfo
			{
				RoomId = room.Id.ToString(),
				Version = room.Version,
				AvailableOperations = { resolver.GetRoomOperations(authService.UserId, room) },
				Columns = { columns },
				IsVoteStarted = room.IsVoteStarted,
				VotesCount = room.VotesCount,
				VotesLeft = room.Users.FirstOrDefault(x => x.Id == authService.UserId)?.VotesCount ?? 0
			}
		};
	}

	private RoomColumn MapColumn(Room room, Column c)
	{
		return new RoomColumn
		{
			Id = c.Id.ToString(),
			ColumnName = c.Name,
			Order = c.OrderId,
			Color = c.Color,
			Cards =
			{
				c.Cards.OrderBy(x => x.OrderId)
					.Select(card => MapCard(room, card))
			}
		};
	}

	private TextCard MapCard(Room room, Models.TextCard card)
	{
		return new TextCard()
		{
			Id = card.Id.ToString(),
			Text = card.Text,
			Order = card.OrderId,
			LikesCount = room.IsVoteStarted ? 0 : card.UsersLiked.Count,
			IsUserLiked = card.UsersLiked.Contains(authService.UserId),
			AvailableOperations = { resolver.GetCardOperations(authService.UserId, room, card) }
		};
	}
}