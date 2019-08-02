using KSerialization;
using System.IO;

namespace Database
{
	public class BuildNRoomTypes : ColonyAchievementRequirement
	{
		private RoomType roomType;

		private int numToCreate;

		public BuildNRoomTypes(RoomType roomType, int numToCreate = 1)
		{
			this.roomType = roomType;
			this.numToCreate = numToCreate;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == roomType)
				{
					num++;
				}
			}
			return num >= numToCreate;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(roomType.Id);
			writer.Write(numToCreate);
		}

		public override void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			roomType = Db.Get().RoomTypes.Get(id);
			numToCreate = reader.ReadInt32();
		}
	}
}