using System;

namespace Klei.AI
{
	public class FertilityModifier : Resource
	{
		public delegate void FertilityModFn(FertilityMonitor.Instance inst, Tag eggTag);

		public string Description;

		public Tag TargetTag;

		public Func<string, string> TooltipCB;

		public FertilityModFn ApplyFunction;

		public FertilityModifier(string id, Tag targetTag, string name, string description, Func<string, string> tooltipCB, FertilityModFn applyFunction)
			: base(id, name)
		{
			Description = description;
			TargetTag = targetTag;
			TooltipCB = tooltipCB;
			ApplyFunction = applyFunction;
		}

		public string GetTooltip()
		{
			if (TooltipCB == null)
			{
				return Description;
			}
			return TooltipCB(Description);
		}
	}
}
