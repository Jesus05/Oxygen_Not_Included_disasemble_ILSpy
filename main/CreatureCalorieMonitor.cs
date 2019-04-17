using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CreatureCalorieMonitor : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>
{
	public struct CaloriesConsumedEvent
	{
		public Tag tag;

		public float calories;
	}

	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public Diet diet;

		public float minPoopSizeInCalories = 100f;

		public float minimumTimeBeforePooping = 10f;

		public float deathTimer = 6000f;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.DIET_HEADER, Descriptor.DescriptorType.Effect, false));
			if (diet.consumedTags.Count > 0)
			{
				float calorie_loss_per_second = 0f;
				Trait trait = Db.Get().traits.Get(obj.GetComponent<Modifiers>().initialTraits[0]);
				foreach (AttributeModifier selfModifier in trait.SelfModifiers)
				{
					if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						calorie_loss_per_second = selfModifier.Value;
					}
				}
				string newValue = string.Join(", ", (from t in diet.consumedTags
				select t.Key.ProperName()).ToArray());
				string newValue2 = string.Join("\n", (from t in diet.consumedTags
				select UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass((0f - calorie_loss_per_second) / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"))).ToArray());
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue2), Descriptor.DescriptorType.Effect, false));
			}
			if (diet.producedTags.Count > 0)
			{
				string newValue3 = string.Join(", ", (from t in diet.producedTags
				select t.Key.ProperName()).ToArray());
				string newValue4 = string.Join("\n", (from t in diet.producedTags
				select UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f, GameUtil.TimeSlice.None))).ToArray());
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", newValue3), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", newValue4), Descriptor.DescriptorType.Effect, false));
			}
			return list;
		}
	}

	public class HungryStates : State
	{
		public class OutOfCaloriesState : State
		{
			public State wild;

			public State tame;

			public State starvedtodeath;
		}

		public State hungry;

		public OutOfCaloriesState outofcalories;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class Stomach
	{
		[Serializable]
		public struct CaloriesConsumedEntry
		{
			public Tag tag;

			public float calories;
		}

		[Serialize]
		private List<CaloriesConsumedEntry> caloriesConsumed = new List<CaloriesConsumedEntry>();

		private float minPoopSizeInCalories;

		private Diet diet;

		private GameObject owner;

		public Stomach(Diet diet, GameObject owner, float min_poop_size_in_calories)
		{
			this.diet = diet;
			this.owner = owner;
			minPoopSizeInCalories = min_poop_size_in_calories;
		}

		public void Poop()
		{
			float num = 0f;
			Tag tag = Tag.Invalid;
			byte disease_idx = byte.MaxValue;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry value = caloriesConsumed[i];
				if (!(value.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(value.tag);
					if (dietInfo != null && (!(tag != Tag.Invalid) || !(tag != dietInfo.producedElement)))
					{
						num += dietInfo.ConvertConsumptionMassToProducedMass(dietInfo.ConvertCaloriesToConsumptionMass(value.calories));
						tag = dietInfo.producedElement;
						disease_idx = dietInfo.diseaseIdx;
						num2 = (int)(dietInfo.diseasePerKgProduced * num);
						value.calories = 0f;
						caloriesConsumed[i] = value;
						flag = (flag || dietInfo.produceSolidTile);
					}
				}
			}
			if (!(num <= 0f) && !(tag == Tag.Invalid))
			{
				Element element = ElementLoader.GetElement(tag);
				Debug.Assert(element != null, "TODO: implement non-element tag spawning");
				int num3 = Grid.PosToCell(owner.transform.GetPosition());
				float temperature = owner.GetComponent<PrimaryElement>().Temperature;
				if (element.IsLiquid)
				{
					FallingWater.instance.AddParticle(num3, element.idx, num, temperature, disease_idx, num2, true, false, false, false);
				}
				else if (element.IsGas)
				{
					SimMessages.AddRemoveSubstance(num3, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, disease_idx, num2, true, -1);
				}
				else if (flag)
				{
					Facing component = owner.GetComponent<Facing>();
					int frontCell = component.GetFrontCell();
					SimMessages.AddRemoveSubstance(frontCell, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, disease_idx, num2, true, -1);
				}
				else
				{
					element.substance.SpawnResource(Grid.CellToPosCCC(num3, Grid.SceneLayer.Ore), num, temperature, disease_idx, num2, false, false, false);
				}
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, owner.transform, 1.5f, false);
			}
		}

		public List<CaloriesConsumedEntry> GetCalorieEntries()
		{
			return caloriesConsumed;
		}

		public float GetTotalConsumedCalories()
		{
			float num = 0f;
			foreach (CaloriesConsumedEntry item in caloriesConsumed)
			{
				CaloriesConsumedEntry current = item;
				if (!(current.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(current.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						num += current.calories;
					}
				}
			}
			return num;
		}

		public float GetFullness()
		{
			float totalConsumedCalories = GetTotalConsumedCalories();
			return totalConsumedCalories / minPoopSizeInCalories;
		}

		public bool IsReadyToPoop()
		{
			float totalConsumedCalories = GetTotalConsumedCalories();
			return totalConsumedCalories > 0f && totalConsumedCalories >= minPoopSizeInCalories;
		}

		public void Consume(Tag tag, float calories)
		{
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry value = caloriesConsumed[i];
				if (value.tag == tag)
				{
					value.calories += calories;
					caloriesConsumed[i] = value;
					return;
				}
			}
			CaloriesConsumedEntry item = default(CaloriesConsumedEntry);
			item.tag = tag;
			item.calories = calories;
			caloriesConsumed.Add(item);
		}

		public Tag GetNextPoopEntry()
		{
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry caloriesConsumedEntry = caloriesConsumed[i];
				if (!(caloriesConsumedEntry.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(caloriesConsumedEntry.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						return dietInfo.producedElement;
					}
				}
			}
			return Tag.Invalid;
		}
	}

	public new class Instance : GameInstance
	{
		public const float HUNGRY_RATIO = 0.9f;

		public AmountInstance calories;

		[Serialize]
		public Stomach stomach;

		public float lastMealOrPoopTime;

		public AttributeInstance metabolism;

		public AttributeModifier deltaCalorieMetabolismModifier;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
			calories.value = calories.GetMax() * 0.9f;
			stomach = new Stomach(def.diet, master.gameObject, def.minPoopSizeInCalories);
			metabolism = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
			deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, true, false, false);
			calories.deltaAttribute.Add(deltaCalorieMetabolismModifier);
		}

		public void OnCaloriesConsumed(object data)
		{
			CaloriesConsumedEvent caloriesConsumedEvent = (CaloriesConsumedEvent)data;
			calories.value += caloriesConsumedEvent.calories;
			stomach.Consume(caloriesConsumedEvent.tag, caloriesConsumedEvent.calories);
			lastMealOrPoopTime = Time.time;
		}

		public float GetDeathTimeRemaining()
		{
			return base.smi.def.deathTimer - (GameClock.Instance.GetTime() - base.sm.starvationStartTime.Get(base.smi));
		}

		public void Poop()
		{
			lastMealOrPoopTime = Time.time;
			stomach.Poop();
		}

		private float GetCalories0to1()
		{
			return calories.value / calories.GetMax();
		}

		public bool IsHungry()
		{
			return GetCalories0to1() < 0.9f;
		}

		public bool IsOutOfCalories()
		{
			return GetCalories0to1() <= 0f;
		}
	}

	public State normal;

	private HungryStates hungry;

	private Effect outOfCaloriesTame;

	public FloatParameter starvationStartTime;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<CreatureCalorieMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = normal;
		base.serializable = true;
		root.EventHandler(GameHashes.CaloriesConsumed, delegate(Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.Poop, ReadyToPoop, delegate(Instance smi)
		{
			smi.Poop();
		}).Update(UpdateMetabolismCalorieModifier, UpdateRate.SIM_200ms, false);
		normal.Transition(hungry, (Instance smi) => smi.IsHungry(), UpdateRate.SIM_1000ms);
		hungry.DefaultState(hungry.hungry).ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, normal, (Instance smi) => !smi.IsHungry());
		hungry.hungry.Transition(normal, (Instance smi) => !smi.IsHungry(), UpdateRate.SIM_1000ms).Transition(hungry.outofcalories, (Instance smi) => smi.IsOutOfCalories(), UpdateRate.SIM_1000ms).ToggleStatusItem(CREATURES.STATUSITEMS.HUNGRY.NAME, CREATURES.STATUSITEMS.HUNGRY.TOOLTIP, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, null);
		hungry.outofcalories.DefaultState(hungry.outofcalories.wild).Transition(hungry.hungry, (Instance smi) => !smi.IsOutOfCalories(), UpdateRate.SIM_1000ms);
		hungry.outofcalories.wild.TagTransition(GameTags.Creatures.Wild, hungry.outofcalories.tame, true).ToggleStatusItem(CREATURES.STATUSITEMS.HUNGRY.NAME, CREATURES.STATUSITEMS.HUNGRY.TOOLTIP, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, null);
		hungry.outofcalories.tame.Enter("StarvationStartTime", StarvationStartTime).Exit("ClearStarvationTime", delegate(Instance smi)
		{
			starvationStartTime.Set(0f, smi);
		}).Transition(hungry.outofcalories.starvedtodeath, (Instance smi) => smi.GetDeathTimeRemaining() <= 0f, UpdateRate.SIM_1000ms)
			.TagTransition(GameTags.Creatures.Wild, hungry.outofcalories.wild, false)
			.ToggleStatusItem(CREATURES.STATUSITEMS.STARVING.NAME, CREATURES.STATUSITEMS.STARVING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, default(HashedString), 0, (string str, Instance smi) => str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(smi.GetDeathTimeRemaining(), "F1")), null, null)
			.ToggleNotification((Instance smi) => new Notification(CREATURES.STATUSITEMS.STARVING.NOTIFICATION_NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notifications, object data) => CREATURES.STATUSITEMS.STARVING.NOTIFICATION_TOOLTIP + notifications.ReduceMessages(false), null, true, 0f, null, null, null))
			.ToggleEffect((Instance smi) => outOfCaloriesTame);
		hungry.outofcalories.starvedtodeath.Enter(delegate(Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
		});
		outOfCaloriesTame = new Effect("OutOfCaloriesTame", CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, CREATURES.MODIFIERS.OUT_OF_CALORIES.TOOLTIP, 0f, false, false, false, null, 0f, null);
		outOfCaloriesTame.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, false, false, true));
	}

	private static bool ReadyToPoop(Instance smi)
	{
		if (!smi.stomach.IsReadyToPoop())
		{
			return false;
		}
		if (Time.time - smi.lastMealOrPoopTime < smi.def.minimumTimeBeforePooping)
		{
			return false;
		}
		return true;
	}

	private static void UpdateMetabolismCalorieModifier(Instance smi, float dt)
	{
		smi.deltaCalorieMetabolismModifier.SetValue(1f - Db.Get().CritterAttributes.Metabolism.Lookup(smi.gameObject).GetTotalValue() / 100f);
	}

	private static void StarvationStartTime(Instance smi)
	{
		float num = smi.sm.starvationStartTime.Get(smi);
		if (num == 0f)
		{
			smi.sm.starvationStartTime.Set(GameClock.Instance.GetTime(), smi);
		}
	}
}
