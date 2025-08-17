using System.Text.Json;
using Orleans.Configuration;
using Orleans.Serialization;
using Orleans.Serialization.WireProtocol;
using RmsRetro.Grains.Auth;
using RmsRetro.Grains.Consts;
using RmsRetro.Grains.Gateways;
using RmsRetro.Grains.Rooms.Commands;
using RmsRetro.Grains.Rooms.Commands.Abstractions;
using RmsRetro.Grains.Rooms.Services;
using RmsRetro.MessageHub.Protos.HubApi;
using RmsRetro.Silo.Options;

namespace RmsRetro.Silo.Extensions;

public static class HostExtensions
{
	internal static IHostBuilder AddOrleans(this IHostBuilder builder)
	{
		builder.UseOrleans((hostBuilder, silo) =>
		{
			var siloSettings = hostBuilder.Configuration.GetSection(nameof(SiloConfig)).Get<SiloConfig>();
			ArgumentNullException.ThrowIfNull(siloSettings);
			silo.Services.AddSerializer(sb => sb.AddProtobufSerializer(
				type => type.Namespace != null && (type.Namespace.StartsWith("RmsRetro.Protos") || type.Namespace.StartsWith("Google.Protobuf")),
				type =>  type.Namespace != null && (type.Namespace.StartsWith("RmsRetro.Protos") || type.Namespace.StartsWith("Google.Protobuf"))));

			silo.UseRedisClustering(options => options.ConfigurationOptions = new()
				{
					EndPoints = [new(siloSettings.ClusterConfig.ConnectionString)]
				})
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = siloSettings.ClusterConfig.ClusterId;
					options.ServiceId = siloSettings.ClusterConfig.ServiceId;
				})
				.AddRedisGrainStorage(StorageConstants.RedisStorage, options =>
				{
					options.ConfigurationOptions = new()
					{
						EndPoints = [new(siloSettings.RedisPersistenceConfig.ConnectionString)]
					};
				})
				.ConfigureLogging(logging => logging.AddConsole());
			
			var messageSettings = hostBuilder.Configuration.GetSection(nameof(MessageHubConfig)).Get<MessageHubConfig>();
			ArgumentNullException.ThrowIfNull(siloSettings);
			silo.Services.AddGrpcClient<HubApiService.HubApiServiceClient>(o =>
			{
				o.Address = new Uri(messageSettings!.Url);
			});
			silo.Services.AddTransient<INotificationHubGateway, NotificationHubGateway>();
			silo.Services.AddSingleton<IAuthService, AuthService>();
			silo.AddIncomingGrainCallFilter<AuthGrainFilter>();
			silo.Services.AddSingleton<IRoomCommandFactory, RoomCommandFactory>();
			silo.Services.AddTransient<IRoomOperationHandler, RoomOperationHandler>();
			silo.Services.AddSingleton<IOperationResolver, OperationResolver>();
			silo.Services.AddScoped<IRoomMapper, RoomMapper>();
		});
		return builder;
	}
}