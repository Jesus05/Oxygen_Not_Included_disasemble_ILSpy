using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TUNING
{
	public class TRAITS
	{
		public static float EARLYBIRD_MODIFIER = 2f;

		public static int EARLYBIRD_SCHEDULEBLOCK = 5;

		public static float NIGHTOWL_MODIFIER = 3f;

		public const float FLATULENCE_EMIT_MASS = 0.1f;

		public static float FLATULENCE_EMIT_INTERVAL_MIN = 10f;

		public static float FLATULENCE_EMIT_INTERVAL_MAX = 40f;

		public static float STINKY_EMIT_INTERVAL_MIN = 10f;

		public static float STINKY_EMIT_INTERVAL_MAX = 30f;

		public static float NARCOLEPSY_INTERVAL_MIN = 300f;

		public static float NARCOLEPSY_INTERVAL_MAX = 600f;

		public static float NARCOLEPSY_SLEEPDURATION_MIN = 15f;

		public static float NARCOLEPSY_SLEEPDURATION_MAX = 30f;

		public const float INTERRUPTED_SLEEP_STRESS_DELTA = 10f;

		public const float INTERRUPTED_SLEEP_ATHLETICS_DELTA = -2f;

		public static int NO_ATTRIBUTE_BONUS = 0;

		public static int GOOD_ATTRIBUTE_BONUS = 3;

		public static int GREAT_ATTRIBUTE_BONUS = 5;

		public static int BAD_ATTRIBUTE_PENALTY = -3;

		public static int HORRIBLE_ATTRIBUTE_PENALTY = -5;

		public static readonly List<System.Action> TRAIT_CREATORS = new List<System.Action>
		{
			CreateAttributeEffectTrait("None", DUPLICANTS.CONGENITALTRAITS.NONE.NAME, DUPLICANTS.CONGENITALTRAITS.NONE.DESC, string.Empty, (float)NO_ATTRIBUTE_BONUS, false, null),
			CreateComponentTrait<Stinky>("Stinky", DUPLICANTS.CONGENITALTRAITS.STINKY.NAME, DUPLICANTS.CONGENITALTRAITS.STINKY.DESC, false, null),
			CreateAttributeEffectTrait("Ellie", DUPLICANTS.CONGENITALTRAITS.ELLIE.NAME, DUPLICANTS.CONGENITALTRAITS.ELLIE.DESC, "AirConsumptionRate", -0.0449999981f, "DecorExpectation", -5f, false),
			CreateDisabledTaskTrait("Joshua", DUPLICANTS.CONGENITALTRAITS.JOSHUA.NAME, DUPLICANTS.CONGENITALTRAITS.JOSHUA.DESC, "Combat", true),
			CreateComponentTrait<Stinky>("Liam", DUPLICANTS.CONGENITALTRAITS.LIAM.NAME, DUPLICANTS.CONGENITALTRAITS.LIAM.DESC, false, null),
			CreateDisabledTaskTrait("CantResearch", DUPLICANTS.TRAITS.CANTRESEARCH.NAME, DUPLICANTS.TRAITS.CANTRESEARCH.DESC, "Research", true),
			CreateDisabledTaskTrait("CantBuild", DUPLICANTS.TRAITS.CANTBUILD.NAME, DUPLICANTS.TRAITS.CANTBUILD.DESC, "Build", false),
			CreateDisabledTaskTrait("CantCook", DUPLICANTS.TRAITS.CANTCOOK.NAME, DUPLICANTS.TRAITS.CANTCOOK.DESC, "Cook", true),
			CreateDisabledTaskTrait("CantDig", DUPLICANTS.TRAITS.CANTDIG.NAME, DUPLICANTS.TRAITS.CANTDIG.DESC, "Dig", false),
			CreateDisabledTaskTrait("Hemophobia", DUPLICANTS.TRAITS.HEMOPHOBIA.NAME, DUPLICANTS.TRAITS.HEMOPHOBIA.DESC, "MedicalAid", true),
			CreateDisabledTaskTrait("ScaredyCat", DUPLICANTS.TRAITS.SCAREDYCAT.NAME, DUPLICANTS.TRAITS.SCAREDYCAT.DESC, "Combat", true),
			CreateAttributeEffectTrait("MouthBreather", DUPLICANTS.TRAITS.MOUTHBREATHER.NAME, DUPLICANTS.TRAITS.MOUTHBREATHER.DESC, "AirConsumptionRate", 0.1f, false, null),
			CreateAttributeEffectTrait("CalorieBurner", DUPLICANTS.TRAITS.CALORIEBURNER.NAME, DUPLICANTS.TRAITS.CALORIEBURNER.DESC, "CaloriesDelta", -833.3333f, false, null),
			CreateAttributeEffectTrait("SmallBladder", DUPLICANTS.TRAITS.SMALLBLADDER.NAME, DUPLICANTS.TRAITS.SMALLBLADDER.DESC, "BladderDelta", 0.000277777785f, false, null),
			CreateAttributeEffectTrait("Anemic", DUPLICANTS.TRAITS.ANEMIC.NAME, DUPLICANTS.TRAITS.ANEMIC.DESC, "Athletics", (float)HORRIBLE_ATTRIBUTE_PENALTY, false, null),
			CreateAttributeEffectTrait("SlowLearner", DUPLICANTS.TRAITS.SLOWLEARNER.NAME, DUPLICANTS.TRAITS.SLOWLEARNER.DESC, "Learning", (float)BAD_ATTRIBUTE_PENALTY, false, null),
			CreateAttributeEffectTrait("NoodleArms", DUPLICANTS.TRAITS.NOODLEARMS.NAME, DUPLICANTS.TRAITS.NOODLEARMS.DESC, "Strength", (float)BAD_ATTRIBUTE_PENALTY, false, null),
			CreateAttributeEffectTrait("InteriorDecorator", DUPLICANTS.TRAITS.INTERIORDECORATOR.NAME, DUPLICANTS.TRAITS.INTERIORDECORATOR.DESC, "Art", (float)GOOD_ATTRIBUTE_BONUS, "DecorExpectation", -5f, true),
			CreateAttributeEffectTrait("SimpleTastes", DUPLICANTS.TRAITS.SIMPLETASTES.NAME, DUPLICANTS.TRAITS.SIMPLETASTES.DESC, "FoodExpectation", 1f, true, null),
			CreateAttributeEffectTrait("Foodie", DUPLICANTS.TRAITS.FOODIE.NAME, DUPLICANTS.TRAITS.FOODIE.DESC, "Cooking", (float)GOOD_ATTRIBUTE_BONUS, "FoodExpectation", -1f, true),
			CreateAttributeEffectTrait("Regeneration", DUPLICANTS.TRAITS.REGENERATION.NAME, DUPLICANTS.TRAITS.REGENERATION.DESC, "HitPointsDelta", 0.0333333351f, false, null),
			CreateAttributeEffectTrait("DeeperDiversLungs", DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.NAME, DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.DESC, "AirConsumptionRate", -0.05f, false, null),
			CreateAttributeEffectTrait("SunnyDisposition", DUPLICANTS.TRAITS.SUNNYDISPOSITION.NAME, DUPLICANTS.TRAITS.SUNNYDISPOSITION.DESC, "StressDelta", -0.0333333351f, false, delegate(GameObject go)
			{
				go.GetComponent<KBatchedAnimController>().AddAnimOverrides(Assets.GetAnim("anim_loco_happy_kanim"), 0f);
			}),
			CreateAttributeEffectTrait("RockCrusher", DUPLICANTS.TRAITS.ROCKCRUSHER.NAME, DUPLICANTS.TRAITS.ROCKCRUSHER.DESC, "Strength", 10f, false, null),
			CreateTrait("Uncultured", DUPLICANTS.TRAITS.UNCULTURED.NAME, DUPLICANTS.TRAITS.UNCULTURED.DESC, "DecorExpectation", 20f, new string[1]
			{
				"Art"
			}, true, true),
			CreateNamedTrait("Archaeologist", DUPLICANTS.TRAITS.ARCHAEOLOGIST.NAME, DUPLICANTS.TRAITS.ARCHAEOLOGIST.DESC, false),
			CreateAttributeEffectTrait("WeakImmuneSystem", DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.DESC, "ImmuneLevelDelta", -0.00625f, false, null),
			CreateAttributeEffectTrait("IrritableBowel", DUPLICANTS.TRAITS.IRRITABLEBOWEL.NAME, DUPLICANTS.TRAITS.IRRITABLEBOWEL.DESC, "ToiletEfficiency", -0.5f, false, null),
			CreateComponentTrait<Flatulence>("Flatulence", DUPLICANTS.TRAITS.FLATULENCE.NAME, DUPLICANTS.TRAITS.FLATULENCE.DESC, false, null),
			CreateComponentTrait<Snorer>("Snorer", DUPLICANTS.TRAITS.SNORER.NAME, DUPLICANTS.TRAITS.SNORER.DESC, false, null),
			CreateComponentTrait<Narcolepsy>("Narcolepsy", DUPLICANTS.TRAITS.NARCOLEPSY.NAME, DUPLICANTS.TRAITS.NARCOLEPSY.DESC, false, null),
			CreateAttributeEffectTrait("Twinkletoes", DUPLICANTS.TRAITS.TWINKLETOES.NAME, DUPLICANTS.TRAITS.TWINKLETOES.DESC, "Athletics", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateAttributeEffectTrait("Greasemonkey", DUPLICANTS.TRAITS.GREASEMONKEY.NAME, DUPLICANTS.TRAITS.GREASEMONKEY.DESC, "Machinery", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateAttributeEffectTrait("MoleHands", DUPLICANTS.TRAITS.MOLEHANDS.NAME, DUPLICANTS.TRAITS.MOLEHANDS.DESC, "Digging", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateAttributeEffectTrait("FastLearner", DUPLICANTS.TRAITS.FASTLEARNER.NAME, DUPLICANTS.TRAITS.FASTLEARNER.DESC, "Learning", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateAttributeEffectTrait("DiversLung", DUPLICANTS.TRAITS.DIVERSLUNG.NAME, DUPLICANTS.TRAITS.DIVERSLUNG.DESC, "AirConsumptionRate", -0.025f, true, null),
			CreateAttributeEffectTrait("StrongArm", DUPLICANTS.TRAITS.STRONGARM.NAME, DUPLICANTS.TRAITS.STRONGARM.DESC, "Strength", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateEffectModifierTrait("IronGut", DUPLICANTS.TRAITS.IRONGUT.NAME, DUPLICANTS.TRAITS.IRONGUT.DESC, new string[1]
			{
				"Diarrhea"
			}, true),
			CreateAttributeEffectTrait("StrongImmuneSystem", DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.DESC, "ImmuneLevelDelta", 0.0125f, true, null),
			CreateAttributeEffectTrait("BedsideManner", DUPLICANTS.TRAITS.BEDSIDEMANNER.NAME, DUPLICANTS.TRAITS.BEDSIDEMANNER.DESC, "Caring", (float)GOOD_ATTRIBUTE_BONUS, true, null),
			CreateTrait("Aggressive", DUPLICANTS.TRAITS.AGGRESSIVE.NAME, DUPLICANTS.TRAITS.AGGRESSIVE.DESC, OnAddAggressive, null, false, () => DUPLICANTS.TRAITS.AGGRESSIVE.NOREPAIR),
			CreateTrait("UglyCrier", DUPLICANTS.TRAITS.UGLYCRIER.NAME, DUPLICANTS.TRAITS.UGLYCRIER.DESC, OnAddUglyCrier, null, false, null),
			CreateTrait("BingeEater", DUPLICANTS.TRAITS.BINGEEATER.NAME, DUPLICANTS.TRAITS.BINGEEATER.DESC, OnAddBingeEater, null, false, null),
			CreateTrait("StressVomiter", DUPLICANTS.TRAITS.STRESSVOMITER.NAME, DUPLICANTS.TRAITS.STRESSVOMITER.DESC, OnAddStressVomiter, null, false, null),
			CreateComponentTrait<EarlyBird>("EarlyBird", DUPLICANTS.TRAITS.EARLYBIRD.NAME, DUPLICANTS.TRAITS.EARLYBIRD.DESC, true, () => string.Format(DUPLICANTS.TRAITS.EARLYBIRD.EXTENDED_DESC, GameUtil.AddPositiveSign(EARLYBIRD_MODIFIER.ToString(), true))),
			CreateComponentTrait<NightOwl>("NightOwl", DUPLICANTS.TRAITS.NIGHTOWL.NAME, DUPLICANTS.TRAITS.NIGHTOWL.DESC, true, null),
			CreateComponentTrait<Claustrophobic>("Claustrophobic", DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.DESC, false, null),
			CreateComponentTrait<PrefersWarmer>("PrefersWarmer", DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.DESC, false, null),
			CreateComponentTrait<PrefersColder>("PrefersColder", DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.DESC, false, null),
			CreateComponentTrait<SensitiveFeet>("SensitiveFeet", DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.NAME, DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.DESC, false, null),
			CreateComponentTrait<Fashionable>("Fashionable", DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.NAME, DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.DESC, false, null),
			CreateComponentTrait<Climacophobic>("Climacophobic", DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.DESC, false, null),
			CreateComponentTrait<SolitarySleeper>("SolitarySleeper", DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.NAME, DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.DESC, false, null),
			CreateComponentTrait<Workaholic>("Workaholic", DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.NAME, DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.DESC, false, null)
		};

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache3;

		private static System.Action CreateDisabledTaskTrait(string id, string name, string desc, string disabled_chore_group, bool is_valid_starter_trait)
		{
			return delegate
			{
				ChoreGroup[] disabled_chore_groups = new ChoreGroup[1]
				{
					Db.Get().ChoreGroups.Get(disabled_chore_group)
				};
				Db.Get().CreateTrait(id, name, desc, null, true, disabled_chore_groups, false, is_valid_starter_trait);
			};
		}

		private static System.Action CreateTrait(string id, string name, string desc, string attributeId, float delta, string[] chore_groups, bool positiveTrait = false, bool is_refusing_to_do_job = false)
		{
			return delegate
			{
				List<ChoreGroup> list = new List<ChoreGroup>();
				string[] array = chore_groups;
				foreach (string id2 in array)
				{
					list.Add(Db.Get().ChoreGroups.Get(id2));
				}
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, list.ToArray(), positiveTrait, true);
				trait.isTaskBeingRefused = is_refusing_to_do_job;
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
			};
		}

		private static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, string attributeId2, float delta2, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.Add(new AttributeModifier(attributeId2, delta2, name, false, false, true));
			};
		}

		private static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, bool positiveTrait = false, Action<GameObject> on_add = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.OnAddTrait = on_add;
			};
		}

		private static System.Action CreateEffectModifierTrait(string id, string name, string desc, string[] ignoredEffects, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.AddIgnoredEffects(ignoredEffects);
			};
		}

		private static System.Action CreateNamedTrait(string id, string name, string desc, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
			};
		}

		private static System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, disabled_chore_groups, positiveTrait, true);
				trait.OnAddTrait = on_add;
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static System.Action CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait = false, Func<string> extendedDescFn = null) where T : KMonoBehaviour
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.OnAddTrait = delegate(GameObject go)
				{
					Util.FindOrAddUnityComponent<T>(go);
				};
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static void OnAddStressVomiter(GameObject go)
		{
			Notification notification = new Notification(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalVomiter", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			StressBehaviourMonitor.Instance instance = new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_vomiter_kanim", new HashedString[1]
			{
				"interrupt_vomiter"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new VomitChore(Db.Get().ChoreTypes.StressVomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, notification, null), "anim_loco_vomiter_kanim", 3f);
			instance.StartSM();
		}

		private static void OnAddAggressive(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalAggresive", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			StressBehaviourMonitor.Instance instance = new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_destructive_kanim", new HashedString[1]
			{
				"interrupt_destruct"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new AggressiveChore(chore_provider, null), "anim_loco_destructive_kanim", 3f);
			instance.StartSM();
		}

		private static void OnAddUglyCrier(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalUglyCrier", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			StressBehaviourMonitor.Instance instance = new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_cry_kanim", new HashedString[3]
			{
				"working_pre",
				"working_loop",
				"working_pst"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new UglyCryChore(Db.Get().ChoreTypes.UglyCry, chore_provider, null), "anim_loco_cry_kanim", 3f);
			instance.StartSM();
		}

		private static void OnAddBingeEater(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalBingeEater", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			StressBehaviourMonitor.Instance instance = new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_binge_eat_kanim", new HashedString[1]
			{
				"interrupt_binge_eat"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new BingeEatChore(chore_provider, null), "anim_loco_binge_eat_kanim", 8f);
			instance.StartSM();
		}
	}
}
