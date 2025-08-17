using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands.Abstractions;

public interface IRoomOperationHandler
{
	Task HandleAsync(Room room, InvokeRoomOperationRequest request);
}