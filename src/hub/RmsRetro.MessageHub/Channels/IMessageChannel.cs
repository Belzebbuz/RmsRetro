using System.Collections.Concurrent;
using System.Threading.Channels;
using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.MessageHub.Channels;
public interface IMessageChannelReader
{
	public IAsyncEnumerable<NotificationEvent> ReadAsync(string subId, string channelId);
	void Unsubscribe(string subId, string channelId);
}

public interface IMessageChannelWriter
{
	public Task WriteAsync(string channelId, NotificationEvent message);
}

public record NotificationChannel(string Id, Channel<NotificationEvent> Channel);
public class MessageChannel : IMessageChannelReader, IMessageChannelWriter
{
	private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, NotificationChannel>> _subscriptions = new();
	public IAsyncEnumerable<NotificationEvent> ReadAsync(string subId, string channelId)
	{
		var channels =  _subscriptions.GetOrAdd(channelId, _ => new ());
		var channel = Channel.CreateUnbounded<NotificationEvent>();
		var newChannel = channels.GetOrAdd(subId, new NotificationChannel(subId, channel));
		return newChannel.Channel.Reader.ReadAllAsync();
	}

	public void Unsubscribe(string subId, string channelId)
	{
		var channels = _subscriptions.GetValueOrDefault(channelId);
		channels?.TryRemove(subId,  out var _);
	}

	public async Task WriteAsync(string channelId, NotificationEvent message)
	{
		var subscriptions =  _subscriptions.GetOrAdd(channelId, _ => new ());
		foreach (var sub in subscriptions.Values)
		{
			await sub.Channel.Writer.WriteAsync(message);
		}
	}
}