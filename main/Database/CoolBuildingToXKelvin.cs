using System.IO;

namespace Database
{
	public class CoolBuildingToXKelvin : ColonyAchievementRequirement
	{
		private int kelvinToCoolTo;

		public CoolBuildingToXKelvin(int kelvinToCoolTo)
		{
			this.kelvinToCoolTo = kelvinToCoolTo;
		}

		public override bool Success()
		{
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				if (item.GetComponent<PrimaryElement>().Temperature <= (float)kelvinToCoolTo)
				{
					return true;
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
			kelvinToCoolTo = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(kelvinToCoolTo);
		}
	}
}
