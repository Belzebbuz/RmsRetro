using Orleans.Configuration;
using Orleans.Serialization;
using RmsRetro.Api.Options;

namespace RmsRetro.Api.Extensions;

public static class HostExtensions
{
	public static IHostBuilder AddOrleansClient(this IHostBuilder builder, IConfiguration config)
	{

		builder.UseOrleansClient(client =>
		{
			var orleansSettings = config.GetSection(nameof(ClusterConfig)).Get<ClusterConfig>()
			                      ?? throw new ArgumentNullException(nameof(ClusterConfig));
			client.Services.AddSerializer(sb => sb.AddProtobufSerializer(
				type => type.Namespace != null && (type.Namespace.StartsWith("RmsRetro.Protos") || type.Namespace.StartsWith("Google.Protobuf")),
				type =>  type.Namespace != null && (type.Namespace.StartsWith("RmsRetro.Protos") || type.Namespace.StartsWith("Google.Protobuf"))));
			client
				.UseRedisClustering(options => options.ConfigurationOptions = new()
				{
					EndPoints = [new(orleansSettings.ConnectionString)]
				})
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = orleansSettings.ClusterId;
					options.ServiceId = orleansSettings.ServiceId;
				});
		});
		return builder;
	}
}