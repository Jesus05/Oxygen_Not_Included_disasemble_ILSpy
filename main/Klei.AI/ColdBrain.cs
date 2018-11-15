using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class ColdBrain : Disease
	{
		public const string ID = "ColdBrain";

		public ColdBrain()
			: base("ColdBrain", DiseaseType.Ailment, Severity.Major, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 900f, 0, new RangeInfo(0f, 0f, 1000f, 1000f), new RangeInfo(1f, 1f, 1f, 1f), new RangeInfo(0f, 0f, 1000f, 1000f), new RangeInfo(1f, 1f, 1f, 1f))
		{
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[5]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true)
			}));
			AddDiseaseComponent(new AnimatedDisease(new HashedString[3]
			{
				"anim_idle_cold_kanim",
				"anim_loco_run_cold_kanim",
				"anim_loco_walk_cold_kanim"
			}, "Cold"));
		}
	}
}
