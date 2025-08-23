using System.Collections.Concurrent;
using System.Threading.Channels;
using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.MessageHub.Channels;

public interface IMessageChannelReader
{
	public IAsyncEnumerable<NotificationEvent> ReadAsync(string channelId);
}

public interface IMessageChannelWriter
{
	public Task WriteAsync(string channelId, NotificationEvent message);
}

public class MessageChannel : IMessageChannelReader, IMessageChannelWriter
{
	private readonly ConcurrentDictionary<string, List<Channel<NotificationEvent>>> _channels = new();
	public IAsyncEnumerable<NotificationEvent> ReadAsync(string channelId)
	{
		var channels =  _channels.GetOrAdd(channelId, _ => new List<Channel<NotificationEvent>>());
		var channel = Channel.CreateUnbounded<NotificationEvent>();
		channels.Add(channel);
		return channel.Reader.ReadAllAsync();
	}

	public async Task WriteAsync(string channelId, NotificationEvent message)
	{
		var channels =  _channels.GetOrAdd(channelId, _ => new List<Channel<NotificationEvent>>());
		foreach (var channel in channels)
		{
			await channel.Writer.WriteAsync(message);
		}
	}
}