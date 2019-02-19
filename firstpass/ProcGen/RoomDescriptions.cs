using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class RoomDescriptions : YamlIO<RoomDescriptions>
	{
		public Dictionary<string, Room> rooms
		{
			get;
			private set;
		}

		public RoomDescriptions()
		{
			rooms = new Dictionary<string, Room>();
		}

		public Room GetDesription(Tag item)
		{
			if (rooms.ContainsKey(item.Name))
			{
				return rooms[item.Name];
			}
			return null;
		}
	}
}
