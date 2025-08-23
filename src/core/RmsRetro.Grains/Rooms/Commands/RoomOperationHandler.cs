using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class RoomOperationHandler : IRoomOperationHandler
{
	private readonly IRoomCommandFactory _commandFactory;

	public RoomOperationHandler(
		IRoomCommandFactory commandFactory)
	{
		_commandFactory = commandFactory;
	}

	public async Task HandleAsync(Room room, InvokeRoomOperationRequest request)
	{
		room.RebuildCardsMap();
		var operation = _commandFactory.CreateCommand(request);
		await operation.ExecuteAsync(room);
		room.Version++;
	}
}