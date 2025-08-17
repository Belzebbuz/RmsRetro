namespace RmsRetro.Common.Extensions;

public static class TaskExtensions
{
	public static Task<T> AsTask<T>(this T obj)
	{
		return Task.FromResult(obj);
	}
}