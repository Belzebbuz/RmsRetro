using RmsRetro.MessageHub.Channels;

namespace RmsRetro.MessageHub.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddSingleton<IMessageChannelWriter, MessageChannel>();
		services.AddSingleton<IMessageChannelReader>(pr => (IMessageChannelReader)pr.GetRequiredService<IMessageChannelWriter>());
		return services;
	}
}