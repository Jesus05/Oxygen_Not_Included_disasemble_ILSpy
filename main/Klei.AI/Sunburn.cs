using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class Sunburn : Disease
	{
		public const string ID = "Sunburn";

		public Sunburn()
			: base("Sunburn", DiseaseType.Ailment, Severity.Major, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Exposure
			}, 900f, 0, new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent(), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[1]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0333333351f, DUPLICANTS.DISEASES.SUNBURN.NAME, false, false, true)
			}));
			AddDiseaseComponent(new AnimatedDisease(new HashedString[1]
			{
				"anim_idle_hot_kanim"
			}, "Hot"));
		}
	}
}
