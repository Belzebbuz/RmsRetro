using RmsRetro.Protos.Api;

namespace RmsRetro.Abstractions.Auth;

public interface IUserGrain : IGrainWithGuidKey
{
	Task<ActivateUserResponse> ActivateAsync();
	Task<GetUserStatusResponse> GetStatusAsync();
}