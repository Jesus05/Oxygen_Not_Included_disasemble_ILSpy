using STRINGS;
using System.IO;
using UnityEngine;

namespace Database
{
	public class ActivateLorePOI : ColonyAchievementRequirement
	{
		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override bool Success()
		{
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				KPrefabID component = item.GetComponent<KPrefabID>();
				if (component.HasTag(GameTags.TemplateBuilding))
				{
					Unsealable component2 = item.GetComponent<Unsealable>();
					if ((Object)component2 != (Object)null && component2.unsealed)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.INVESTIGATE_A_POI;
		}
	}
}
