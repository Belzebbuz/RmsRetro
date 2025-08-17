using Google.Protobuf;
using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Rooms.Models;

namespace RmsRetro.Grains.Rooms.Commands.Abstractions;

public abstract class RoomCommandBaseHandler<T> : IRoomCommand where T : IMessage
{
	protected readonly T Operation;
	protected readonly IAuthService AuthService;
	protected RoomCommandBaseHandler(T operation, IAuthService authService)
	{
		Operation = operation;
		AuthService = authService;
	}

	public async Task ExecuteAsync(Room state)
	{
		if (!CanHandle(state))
			throw DomainException.Internal();
		await ExecuteCoreAsync(state);
	}

	protected abstract Task ExecuteCoreAsync(Room state);
	protected abstract bool CanHandle(Room state);
}