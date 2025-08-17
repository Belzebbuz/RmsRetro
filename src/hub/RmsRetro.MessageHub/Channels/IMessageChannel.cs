using System.Collections.Concurrent;
using System.Threading.Channels;
using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.MessageHub.Channels;

public interface IMessageChannelReader
{
	public IAsyncEnumerable<Message> ReadAsync(string channelId);
}

public interface IMessageChannelWriter
{
	public Task WriteAsync(string channelId, Message message);
}

public class MessageChannel : IMessageChannelReader, IMessageChannelWriter
{
	private readonly ConcurrentDictionary<string, List<Channel<Message>>> _channels = new();
	public IAsyncEnumerable<Message> ReadAsync(string channelId)
	{
		var channels =  _channels.GetOrAdd(channelId, _ => new List<Channel<Message>>());
		var channel = Channel.CreateUnbounded<Message>();
		channels.Add(channel);
		return channel.Reader.ReadAllAsync();
	}

	public async Task WriteAsync(string channelId, Message message)
	{
		var channels =  _channels.GetOrAdd(channelId, _ => new List<Channel<Message>>());
		foreach (var channel in channels)
		{
			await channel.Writer.WriteAsync(message);
		}
	}
}