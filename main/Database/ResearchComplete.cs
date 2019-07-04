using STRINGS;
using System.IO;

namespace Database
{
	public class ResearchComplete : ColonyAchievementRequirement
	{
		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH;
		}

		public override string Description()
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH_DESCRIPTION;
		}

		public override bool Success()
		{
			foreach (Tech resource in Db.Get().Techs.resources)
			{
				if (!resource.IsComplete())
				{
					return false;
				}
			}
			return true;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
