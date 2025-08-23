using Newtonsoft.Json;

namespace RmsRetro.Grains.Rooms.Models;

public class Room
{
	public Guid Id { get; set; }
	public int Version { get; set; }
	public HashSet<string> Owners { get; set; } = new ();
	public List<User> Users { get; set; } = new ();
	public Dictionary<Guid, Column> Columns { get; set; } = new ();
	
	[JsonIgnore]
	public IReadOnlyDictionary<Guid, TextCard> Cards => _cards.AsReadOnly();

	public bool IsVoteStarted { get; set; }
	public int VotesCount { get; set; }
	private Dictionary<Guid, TextCard> _cards  = new ();

	public void RebuildCardsMap()
	{
		_cards = Columns.SelectMany(x => x.Value.Cards).ToDictionary(c => c.Id, c => c);
	}
}

public class User
{
	public required string Id { get; set; }
	public int VotesCount { get; set; }
}