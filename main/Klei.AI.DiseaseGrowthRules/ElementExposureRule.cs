namespace Klei.AI.DiseaseGrowthRules
{
	public class ElementExposureRule : ExposureRule
	{
		public SimHashes element;

		public ElementExposureRule(SimHashes element)
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
