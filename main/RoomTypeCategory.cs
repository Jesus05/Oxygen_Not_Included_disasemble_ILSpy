using UnityEngine;

public class RoomTypeCategory : Resource
{
	public Color color
	{
		get;
		private set;
	}

	public RoomTypeCategory(string id, string name, Color color)
		: base(id, name)
	{
		this.color = color;
	}
}
