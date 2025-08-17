using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using RmsRetro.Api.Options;
using StackExchange.Redis;

namespace RmsRetro.Api.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services, string name)
	{
		services.AddCors(options =>
		{
			options.AddPolicy(name, config => config
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowAnyOrigin()
				.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "X-Grpc-Web", "User-Agent")
				.SetPreflightMaxAge(TimeSpan.FromHours(1))
				.Build());
		});
		return services;
	}
}