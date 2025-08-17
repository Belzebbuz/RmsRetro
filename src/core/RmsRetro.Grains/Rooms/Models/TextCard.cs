namespace RmsRetro.Grains.Rooms.Models;

public class TextCard
{
	public required Guid Id { get; set; }
	public required Guid ColumnId { get; set; }
	public required int OrderId { get; set; }
	public required string Text { get; set; }
	public required string UserId { get; set; }
	public HashSet<string> UsersLiked { get; set; } = new();
}