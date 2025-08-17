using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Services;

public interface IOperationResolver
{
	IReadOnlySet<CardOperationTypes> GetCardOperations(string authServiceUserId, Room room, Models.TextCard card);
	IReadOnlySet<RoomOperationTypes> GetRoomOperations(string authServiceUserId, Room room);
}