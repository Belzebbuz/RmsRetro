namespace RmsRetro.Api.Options;

public class ClusterConfig
{
	public required string ConnectionString { get; init; }
	public required string ClusterId { get; init; }
	public required string ServiceId { get; init; }
}