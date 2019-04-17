using System.Collections.Generic;

namespace Database
{
	public class Skill : Resource
	{
		public string description;

		public string skillGroup;

		public string hat;

		public int tier;

		public List<SkillPerk> perks;

		public List<string> priorSkills;

		public Skill(string id, string name, string description, int tier, string hat, string skillGroup)
			: base(id, name)
		{
			this.description = description;
			this.tier = tier;
			this.hat = hat;
			this.skillGroup = skillGroup;
			perks = new List<SkillPerk>();
			priorSkills = new List<string>();
		}

		public bool GivesPerk(SkillPerk perk)
		{
			return perks.Contains(perk);
		}

		public bool GivesPerk(HashedString perkId)
		{
			foreach (SkillPerk perk in perks)
			{
				if (perk.IdHash == perkId)
				{
					return true;
				}
			}
			return false;
		}
	}
}
