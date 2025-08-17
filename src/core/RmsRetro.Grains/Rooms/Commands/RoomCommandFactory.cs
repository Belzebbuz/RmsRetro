using Microsoft.Extensions.DependencyInjection;
using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Rooms.Commands;

public class RoomCommandFactory : IRoomCommandFactory
{
	private readonly IServiceProvider _serviceProvider;

	public RoomCommandFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IRoomCommand CreateCommand(InvokeRoomOperationRequest request)
	{
		return request.OperationCase switch
		{
			InvokeRoomOperationRequest.OperationOneofCase.Add => ActivatorUtilities
				.CreateInstance<AddCardCommand>(_serviceProvider, request.Add),
			InvokeRoomOperationRequest.OperationOneofCase.Edit => ActivatorUtilities
				.CreateInstance<EditCardCommand>(_serviceProvider, request.Edit),
			InvokeRoomOperationRequest.OperationOneofCase.Combine => ActivatorUtilities
				.CreateInstance<CombineCardsCommand>(_serviceProvider, request.Combine),
			InvokeRoomOperationRequest.OperationOneofCase.Delete => ActivatorUtilities
				.CreateInstance<DeleteCardCommand>(_serviceProvider, request.Delete),
			InvokeRoomOperationRequest.OperationOneofCase.AddLike => ActivatorUtilities
				.CreateInstance<AddLikeCardCommand>(_serviceProvider, request.AddLike),
			InvokeRoomOperationRequest.OperationOneofCase.RemoveLike => ActivatorUtilities
				.CreateInstance<RemoveLikeCardCommand>(_serviceProvider, request.RemoveLike),
			InvokeRoomOperationRequest.OperationOneofCase.Move => ActivatorUtilities
				.CreateInstance<MoveCardCommand>(_serviceProvider, request.Move),
			InvokeRoomOperationRequest.OperationOneofCase.StartTimer => ActivatorUtilities
				.CreateInstance<StartTimerCommand>(_serviceProvider, request.StartTimer),
			InvokeRoomOperationRequest.OperationOneofCase.PauseTimer => ActivatorUtilities
				.CreateInstance<PauseTimerCommand>(_serviceProvider, request.PauseTimer),
			InvokeRoomOperationRequest.OperationOneofCase.StartVoting => ActivatorUtilities
				.CreateInstance<StartVotingCommand>(_serviceProvider, request.StartVoting),
			InvokeRoomOperationRequest.OperationOneofCase.StopVoting => ActivatorUtilities
				.CreateInstance<StopVotingCommand>(_serviceProvider, request.StopVoting),
			_ => throw DomainException.Internal()
		};
	}
}