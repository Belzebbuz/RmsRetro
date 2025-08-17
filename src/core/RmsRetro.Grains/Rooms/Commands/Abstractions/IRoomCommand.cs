using RmsRetro.Grains.Rooms.Models;

namespace RmsRetro.Grains.Rooms.Commands.Abstractions;

public interface IRoomCommand
{
	Task ExecuteAsync(Room state);
}