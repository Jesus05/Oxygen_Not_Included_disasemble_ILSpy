using STRINGS;
using System;
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

		public override string GetProgress(bool complete)
		{
			float num = 3.40282347E+38f;
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				num = Math.Min(num, item.GetComponent<PrimaryElement>().Temperature);
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.KELVIN_COOLING, num);
		}
	}
}
