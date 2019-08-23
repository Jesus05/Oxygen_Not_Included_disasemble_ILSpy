using STRINGS;
using System.IO;
using UnityEngine;

namespace Database
{
	public class CreateMasterPainting : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				if (item.isArtable)
				{
					Painting component = item.GetComponent<Painting>();
					if ((Object)component != (Object)null && component.CurrentStatus == Artable.Status.Great)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CREATE_A_PAINTING;
		}
	}
}
