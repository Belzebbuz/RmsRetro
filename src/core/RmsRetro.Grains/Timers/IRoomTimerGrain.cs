using Google.Protobuf.WellKnownTypes;
using RmsRetro.Abstractions.Rooms;
using RmsRetro.Common.Extensions;
using RmsRetro.Common.OrleansKeys;
using RmsRetro.Grains.Gateways;
using RmsRetro.MessageHub.Protos.HubApi;
using RmsRetro.Protos.Api;

namespace RmsRetro.Grains.Timers;

public interface IRoomTimerGrain : IGrainWithGuidKey
{
	Task<Empty> StartAsync(int minutes, bool stopVoteOnEnd = false);
	Task<Empty> StopAsync();
}

public class RoomTimerGrain(INotificationHubGateway gateway) : Grain, IRoomTimerGrain, IDisposable
{
	private IGrainTimer? _timer;
	private int? _currentValue;
	private int? _startValue;

	public Task<Empty> StartAsync(int minutes, bool stopVoteOnEnd = false)
	{
		_currentValue = minutes * 60;
		_startValue = minutes * 60;
		_timer = this.RegisterGrainTimer(() => TimerTick(stopVoteOnEnd), new GrainTimerCreationOptions
		{
			DueTime = TimeSpan.Zero,
			Period = TimeSpan.FromSeconds(1),
			KeepAlive = true
		});
		return new Empty().AsTask();
	}
	
	public async Task<Empty> StopAsync()
	{
		Dispose();
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
			Dispose();
			if (stopVoteOnEnd)
			{
				await StopVotingAsync();
			}
		}
	}

	private async Task StopVotingAsync()
	{
		var id = this.GetPrimaryKey().ToString();
		RequestContext.Set(RequestKeys.UserId, id);
		var request = new InvokeRoomOperationRequest()
		{
			RoomId = id,
			StopVoting = new StopVotingOperation()
		};
		await GrainFactory.GetGrain<IRoomGrain>(this.GetPrimaryKey()).HandleOperation(request);
	}

	public void Dispose()
	{
		_timer?.Dispose();
		_timer = null;
	}
}