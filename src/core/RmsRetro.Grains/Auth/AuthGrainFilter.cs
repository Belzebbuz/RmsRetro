using System.Reflection;
using RmsRetro.Abstractions.Exceptions;

namespace RmsRetro.Grains.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeGrainAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousGrainAttribute : Attribute;
public class AuthGrainFilter(IAuthService authService) : IIncomingGrainCallFilter
{
	public async Task Invoke(IIncomingGrainCallContext context)
	{
		var allowAnonymous = context.ImplementationMethod.GetCustomAttribute<AllowAnonymousGrainAttribute>() != null;
		if (allowAnonymous)
		{
			await context.Invoke();
			return;
		}
		
		var attribute = context.ImplementationMethod.GetCustomAttribute<AuthorizeGrainAttribute>()
		                 ?? context.ImplementationMethod.DeclaringType?.GetCustomAttribute<AuthorizeGrainAttribute>();
		if(attribute is not null && !authService.IsAuthenticated)
			throw DomainException.Unauthorized();
		
		await context.Invoke();
	}
}