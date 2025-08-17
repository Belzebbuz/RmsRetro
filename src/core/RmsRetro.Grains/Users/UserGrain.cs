using Google.Protobuf.WellKnownTypes;
using RmsRetro.Abstractions.Auth;
using RmsRetro.Common.Extensions;
using RmsRetro.Grains.Consts;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Users;

public class UserGrain(
	[PersistentState("user", StorageConstants.RedisStorage)]
	IPersistentState<User> user) : Grain, IUserGrain
{
	public async Task<ActivateUserResponse> ActivateAsync()
	{
		user.State ??= new User();
		user.State.IsActive = true;
		await user.WriteStateAsync();
		return new ActivateUserResponse()
		{
			UserId = this.GetPrimaryKey().ToString()
		};
	}

	public Task<GetUserStatusResponse> GetStatusAsync()
	{
		return new GetUserStatusResponse()
		{
			IsActive = user.State?.IsActive ?? false
		}.AsTask();
	}
}