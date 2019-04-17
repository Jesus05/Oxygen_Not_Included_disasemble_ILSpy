using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class Allergies : Sickness
	{
		public const string ID = "Allergies";

		public const float STRESS_OVER_ATTACK = 20f;

		public Allergies()
			: base("Allergies", SicknessType.Pathogen, Severity.Minor, 0.00025f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 60f)
		{
			float value = 20f / base.SicknessDuration;
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AnimatedSickness(new HashedString[1]
			{
				"anim_idle_allergies_kanim"
			}, Db.Get().Expressions.Uncomfortable));
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[2]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, value, DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME, false, false, true),
				new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 10f, DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME, false, false, true)
			}));
		}
	}
}
