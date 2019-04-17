using Database;

public class SkillListable : IListableOption
{
	public LocString name;

	public string skillName
	{
		get;
		private set;
	}

	public string skillHat
	{
		get;
		private set;
	}

	public SkillListable(string name)
	{
		skillName = name;
		Skill skill = Db.Get().Skills.TryGet(skillName);
		if (skill != null)
		{
			this.name = skill.Name;
			skillHat = skill.hat;
		}
	}

	public string GetProperName()
	{
		return name;
	}
}
