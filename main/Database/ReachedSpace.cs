using KSerialization;
using STRINGS;
using System.IO;

namespace Database
{
	public class ReachedSpace : VictoryColonyAchievementRequirement
	{
		private SpaceDestinationType destinationType;

		public ReachedSpace(SpaceDestinationType destinationType = null)
		{
			this.destinationType = destinationType;
		}

		public override string Name()
		{
			if (destinationType == null)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION;
			}
			return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION, destinationType.Name);
		}

		public override string Description()
		{
			if (destinationType == null)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION;
			}
			return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION_DESCRIPTION, destinationType.Name);
		}

		public override bool Success()
		{
			foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
			{
				if (item.state != 0 && item.state != Spacecraft.MissionState.Destroyed)
				{
					SpaceDestination destination = SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.savedSpacecraftDestinations[item.id]);
					if (destinationType == null || destination.GetDestinationType() == destinationType)
					{
						if (destinationType == Db.Get().SpaceDestinationTypes.Wormhole)
						{
							Game.Instance.unlocks.Unlock("temporaltear");
						}
						return true;
					}
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((byte)((destinationType == null) ? 1 : 0));
			if (destinationType != null)
			{
				writer.WriteKleiString(destinationType.Id);
			}
		}

		public override void Deserialize(IReader reader)
		{
			if (reader.ReadByte() == 0)
			{
				string id = reader.ReadKleiString();
				destinationType = Db.Get().SpaceDestinationTypes.Get(id);
			}
		}
	}
}
