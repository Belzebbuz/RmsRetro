using Google.Protobuf.WellKnownTypes;
using RmsRetro.Protos.Api;

namespace RmsRetro.Abstractions.Rooms;

public interface IRoomGrain : IGrainWithGuidKey
{
	Task<InitRoomResponse> InitAsync(InitRoomRequest request);
	Task<Empty> ConnectAsync();
	Task<Empty> HandleOperation(InvokeRoomOperationRequest request);
	Task<GetRoomInfoResponse> GetAsync();
}