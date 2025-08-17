using RmsRetro.Grains.Rooms.Models;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Services;

public interface IRoomMapper
{
	public GetRoomInfoResponse ToInfo(Room room);
}