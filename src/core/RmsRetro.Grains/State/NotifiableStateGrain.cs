using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Orleans.Core;
using RmsRetro.Abstractions.Exceptions;
using RmsRetro.Grains.Gateways;
using RmsRetro.MessageHub.Protos.HubApi;

namespace RmsRetro.Grains.State;

public class SaveStateAttribute : Attribute;
public class InitAttribute : Attribute;

public abstract class NotifiableStateGrain<T>(
	IStorage<T> state, 
	INotificationHubGateway notificationGateway,
	ILogger<NotifiableStateGrain<T>> logger) : Grain, IGrainWithGuidKey, IIncomingGrainCallFilter
{
	public async Task Invoke(IIncomingGrainCallContext context)
	{
		ThrowIfRecordNotExists(context);
		try
		{
			await context.Invoke();
			await SaveChangesAsync(context);
		}
		catch (Exception e)
		{
			logger.LogError(e, $"Grain state error id: {this.GetPrimaryKey().ToString()}");
			await state.ReadStateAsync();
			throw;
		}
	}
	private async Task SaveChangesAsync(IIncomingGrainCallContext context)
	{
		if (context.ImplementationMethod.GetCustomAttribute<SaveStateAttribute>() is null) return;
		await state.WriteStateAsync();
		await notificationGateway.NotifyAsync(GetUpdateChannel(), new Message
		{
			EventType = EventType.StateUpdated,
			Value = GetVersion()
		});
	}
	private void ThrowIfRecordNotExists(IIncomingGrainCallContext context)
	{
		var init = context.ImplementationMethod.GetCustomAttribute<InitAttribute>();
		if(init == null && state.RecordExists is false)
			throw DomainException.NotFound(this.GetPrimaryKey().ToString());
	}
	
	protected virtual string GetUpdateChannel() => 	this.GetPrimaryKey().ToString();
	protected abstract string GetVersion();
}