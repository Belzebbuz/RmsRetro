namespace RmsRetro.Grains.Rooms.Models;

public class Column
{
	public required Guid Id { get; set; }
	public required int OrderId { get; set; }
	public required string Name { get; set; }
	public required string Color { get; set; }
	public List<TextCard> Cards { get; set; } = new ();
}