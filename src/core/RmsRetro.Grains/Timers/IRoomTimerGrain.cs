using Google.Protobuf.WellKnownTypes;
using RmsRetro.Abstractions.Rooms;
using RmsRetro.Common.Extensions;
using RmsRetro.Grains.Gateways;
using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.Grains.Timers;

public interface IRoomTimerGrain : IGrainWithGuidKey
{
	Task<bool> IsStartedAsync();
	Task<Empty> StartAsync(int minutes);
	Task<Empty> StopAsync();
	Task StartVoteAsync(int operationMinutes);
}

public class RoomTimerGrain(INotificationHubGateway gateway) : Grain, IRoomTimerGrain
{
	private IGrainTimer? _timer;
	private int? _currentValue;
	private int? _startValue;
	public Task<bool> IsStartedAsync()
	{
		return (_timer is not null).AsTask();
	}

	public Task<Empty> StartAsync(int minutes)
	{
		_currentValue = minutes * 60;
		_startValue = minutes * 60;
		_timer = this.RegisterGrainTimer(() => TimerTick(), new GrainTimerCreationOptions
		{
			DueTime = TimeSpan.Zero,
			Period = TimeSpan.FromSeconds(1),
			KeepAlive = true
		});
		return new Empty().AsTask();
	}
	
	public async Task StartVoteAsync(int minutes)
	{
		await StopAsync();
		_currentValue = minutes * 60;
		_startValue = minutes * 60;
		_timer = this.RegisterGrainTimer(_ => TimerTick(true), new GrainTimerCreationOptions
		{
			DueTime = TimeSpan.Zero,
			Period = TimeSpan.FromSeconds(1),
			KeepAlive = true
		});
	}
	
	public async Task<Empty> StopAsync()
	{
		_timer?.Dispose();
		await gateway.NotifyAsync(this.GetPrimaryKey().ToString(), new ()
		{
			TimerTick = new TimerTickEvent()
			{
				CurrentValue = 0,
				TotalValue = 0
			}
		});
		return new Empty();
	}

	private async Task TimerTick(bool stopVoteOnEnd = false)
	{
		if(!_currentValue.HasValue || !_startValue.HasValue)
			return;
		
		_currentValue--;
		await gateway.NotifyAsync(this.GetPrimaryKey().ToString(), new ()
		{
			TimerTick = new TimerTickEvent()
			{
				CurrentValue = _currentValue.Value,
				TotalValue = _startValue.Value
			}
		});
		
		if (_currentValue == 0)
		{
			_timer?.Dispose();
			if (stopVoteOnEnd)
			{
				await GrainFactory.GetGrain<IRoomGrain>(this.GetPrimaryKey()).StopVoteAsync();
			}
		}
	}
}