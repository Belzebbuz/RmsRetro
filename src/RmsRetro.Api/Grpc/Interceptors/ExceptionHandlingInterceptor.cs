using Grpc.Core;
using Grpc.Core.Interceptors;
using RmsRetro.Abstractions.Exceptions;

namespace RmsRetro.Api.Grpc.Interceptors;

public class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
{
	public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		try
		{
			return await continuation(request, context);
		}
		catch (DomainException de)
		{
			logger.LogError(de.Message);
			throw new RpcException(new Status(de.Status,de.Message));
		}
		catch (Exception e)
		{
			logger.LogError(e.Message);
			throw new RpcException(new Status(StatusCode.Internal, "Произошла внутренняя ошибка сервера"));
		}
	}
}