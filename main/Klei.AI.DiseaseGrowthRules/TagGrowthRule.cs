namespace Klei.AI.DiseaseGrowthRules
{
	public class TagGrowthRule : GrowthRule
	{
		public Tag tag;

		public TagGrowthRule(Tag tag)
		{
			this.tag = tag;
		}

		public override bool Test(Element e)
		{
			return e.HasTag(tag);
		}

		public override string Name()
		{
			return tag.ProperName();
		}
	}
}
