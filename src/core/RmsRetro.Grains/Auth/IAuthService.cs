using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Common.OrleansKeys;

namespace RmsRetro.Grains.Auth;

public interface IAuthService
{
	public string UserId { get; }
	public bool IsAuthenticated { get; }
}

public class AuthService : IAuthService
{
	public string UserId => RequestContext.Get(RequestKeys.UserId) as string ?? throw DomainException.Unauthenticated();
	public bool IsAuthenticated => RequestContext.Get(RequestKeys.UserId) is not null;
}