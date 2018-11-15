namespace Klei.AI.DiseaseGrowthRules
{
	public class StateGrowthRule : GrowthRule
	{
		public Element.State state;

		public StateGrowthRule(Element.State state)
		{
			this.state = state;
		}

		public override bool Test(Element e)
		{
			return e.IsState(state);
		}

		public override string Name()
		{
			return Element.GetStateString(state);
		}
	}
}
