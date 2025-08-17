using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;
using TextCard = RmsRetro.Grains.Rooms.Models.TextCard;

namespace RmsRetro.Grains.Rooms.Services;

public class OperationResolver : IOperationResolver
{
	public IReadOnlySet<CardOperationTypes> GetCardOperations(string userId, Room room, TextCard card)
	{
		var result = new HashSet<CardOperationTypes>();
		var user = room.Users.FirstOrDefault(x => x.Id == userId);
		if (user == null)
			return result;
		if (room.Owners.Contains(userId) || card.UserId == userId)
		{
			result.Add(CardOperationTypes.EditCard);
			result.Add(CardOperationTypes.DeleteCard);
			result.Add(CardOperationTypes.MoveCard);
		}
		
		if(room.IsVoteStarted && !card.UsersLiked.Contains(userId) && user.VotesCount > 0)
			result.Add(CardOperationTypes.AddLikeCard);
		if(room.IsVoteStarted && card.UsersLiked.Contains(userId))
			result.Add(CardOperationTypes.RemoveLikeCard);
		if(room.Owners.Contains(userId))
			result.Add(CardOperationTypes.CombineCards);
		return result;
	}

	public IReadOnlySet<RoomOperationTypes> GetRoomOperations(string userId, Room room)
	{
		var result = new HashSet<RoomOperationTypes>();
		result.Add(RoomOperationTypes.AddCard);
		if (!room.Owners.Contains(userId)) 
			return result;
		result.Add(RoomOperationTypes.StartTimer);
		result.Add(RoomOperationTypes.PauseTimer);
		result.Add(RoomOperationTypes.StartVoting);
		if (room.IsVoteStarted)
			result.Add(RoomOperationTypes.StopVoting);
		return result;
	}
}