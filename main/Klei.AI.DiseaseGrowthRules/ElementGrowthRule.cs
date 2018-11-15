namespace Klei.AI.DiseaseGrowthRules
{
	public class ElementGrowthRule : GrowthRule
	{
		public SimHashes element;

		public ElementGrowthRule(SimHashes element)
		{
			this.element = element;
		}

		public override bool Test(Element e)
		{
			return e.id == element;
		}

		public override string Name()
		{
			return ElementLoader.FindElementByHash(element).name;
		}
	}
}
