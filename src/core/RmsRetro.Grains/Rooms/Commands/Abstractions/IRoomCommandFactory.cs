using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands.Abstractions;

public interface IRoomCommandFactory
{
	public IRoomCommand CreateCommand(InvokeRoomOperationRequest request);
}