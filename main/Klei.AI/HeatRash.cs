using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class HeatRash : Sickness
	{
		public const string ID = "HeatSickness";

		public HeatRash()
			: base("HeatSickness", SicknessType.Ailment, Severity.Minor, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 180f)
		{
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[4]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.HEATSICKNESS.NAME, false, false, true),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.HEATSICKNESS.NAME, false, false, true),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.HEATSICKNESS.NAME, false, false, true),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.HEATSICKNESS.NAME, false, false, true)
			}));
			AddSicknessComponent(new AnimatedSickness(new HashedString[3]
			{
				"anim_idle_hot_kanim",
				"anim_loco_run_hot_kanim",
				"anim_loco_walk_hot_kanim"
			}, Db.Get().Expressions.SickFierySkin));
			AddSicknessComponent(new PeriodicEmoteSickness("anim_idle_hot_kanim", new HashedString[3]
			{
				"idle_pre",
				"idle_default",
				"idle_pst"
			}, 15f));
		}
	}
}
