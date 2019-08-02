using System.IO;
using UnityEngine;

namespace Database
{
	public class TravelXUsingTransitTubes : ColonyAchievementRequirement
	{
		private int distanceToTravel;

		private NavType navType;

		public TravelXUsingTransitTubes(NavType navType, int distanceToTravel)
		{
			this.navType = navType;
			this.distanceToTravel = distanceToTravel;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Navigator component = item.GetComponent<Navigator>();
				if ((Object)component != (Object)null && component.distanceTravelledByNavType.ContainsKey(navType))
				{
					num += component.distanceTravelledByNavType[navType];
				}
			}
			return num >= distanceToTravel;
		}

		public override void Deserialize(IReader reader)
		{
			byte b = (byte)(navType = (NavType)reader.ReadByte());
			distanceToTravel = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			byte value = (byte)navType;
			writer.Write(value);
			writer.Write(distanceToTravel);
		}
	}
}
