using KSerialization;
using STRINGS;
using System.IO;

namespace Database
{
	public class BuildRoomType : ColonyAchievementRequirement
	{
		private RoomType roomType;

		public BuildRoomType(RoomType roomType)
		{
			this.roomType = roomType;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_ROOM_TYPE, roomType.Name);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_ROOM_TYPE_DESCRIPTION, roomType.Name);
		}

		public override bool Success()
		{
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == roomType)
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(roomType.Id);
		}

		public override void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			roomType = Db.Get().RoomTypes.Get(id);
		}
	}
}
