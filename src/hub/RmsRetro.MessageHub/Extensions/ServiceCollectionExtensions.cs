namespace RmsRetro.MessageHub.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHubCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("default",config => config
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