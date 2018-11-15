using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class HeatRash : Disease
	{
		public const string ID = "HeatRash";

		public HeatRash()
			: base("HeatRash", DiseaseType.Ailment, Severity.Major, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 900f, 0, new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent(), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[4]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false, true),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false, true),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false, true),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false, true)
			}));
			AddDiseaseComponent(new AnimatedDisease(new HashedString[1]
			{
				"anim_idle_hot_kanim"
			}, "Hot"));
		}
	}
}
