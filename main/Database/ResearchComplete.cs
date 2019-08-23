using STRINGS;
using System.IO;

namespace Database
{
	public class ResearchComplete : ColonyAchievementRequirement
	{
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

		public override string GetProgress(bool complete)
		{
			if (complete)
			{
				return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TECH_RESEARCHED, Db.Get().Techs.resources.Count, Db.Get().Techs.resources.Count);
			}
			int num = 0;
			foreach (Tech resource in Db.Get().Techs.resources)
			{
				if (resource.IsComplete())
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TECH_RESEARCHED, num, Db.Get().Techs.resources.Count);
		}
	}
}
