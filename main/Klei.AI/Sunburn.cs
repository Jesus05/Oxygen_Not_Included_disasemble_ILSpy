using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
	public class Sunburn : Sickness
	{
		public const string ID = "SunburnSickness";

		public Sunburn()
			: base("SunburnSickness", SicknessType.Ailment, Severity.Minor, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Exposure
			}, 1020f)
		{
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[1]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0333333351f, DUPLICANTS.DISEASES.SUNBURNSICKNESS.NAME, false, false, true)
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
			}, 5f));
		}
	}
}
