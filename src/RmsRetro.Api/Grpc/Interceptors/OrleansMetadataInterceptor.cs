using System.Security.Claims;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authorization;
using RmsRetro.Abstractions.Auth;
using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Common.OrleansKeys;

namespace RmsRetro.Api.Grpc.Interceptors;

public class OrleansMetadataInterceptor(IClusterClient client) : Interceptor
{
	public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		var httpContext = context.GetHttpContext();
		var endpoint = httpContext.GetEndpoint();
		var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null;
		var id = httpContext.User.FindFirstValue("sub") ??
		         context.RequestHeaders.FirstOrDefault(x => x.Key == "x-player-id")?.Value;
		
		if(Guid.TryParse(id, out var _))
			RequestContext.Set(RequestKeys.UserId, id);
		
		if (allowAnonymous)
			return await continuation(request, context);
		
		if (id == null)
			throw DomainException.Unauthenticated();
		
		var userStatus =  await client
			.GetGrain<IUserGrain>(Guid.Parse(id))
			.GetStatusAsync();
		if (!userStatus.IsActive)
			throw DomainException.Unauthenticated();
		
		return await continuation(request, context);
	}
}