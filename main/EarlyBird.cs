using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class EarlyBird : StateMachineComponent<EarlyBird.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EarlyBird, object>.GameInstance
	{
		public StatesInstance(EarlyBird master)
			: base(master)
		{
		}

		public bool IsMorning()
		{
			if (!((Object)ScheduleManager.Instance == (Object)null) && !(base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview))
			{
				int blockIdx = global::Schedule.GetBlockIdx();
				return blockIdx < TRAITS.EARLYBIRD_SCHEDULEBLOCK;
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EarlyBird>
	{
		public State idle;

		public State early;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.Transition(early, (StatesInstance smi) => smi.IsMorning(), UpdateRate.SIM_200ms);
			early.Enter("Morning", delegate(StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotMorning", delegate(StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.EarlyMorning, (object)null)
				.ToggleExpression(Db.Get().Expressions.Happy, null)
				.Transition(idle, (StatesInstance smi) => !smi.IsMorning(), UpdateRate.SIM_200ms);
		}
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	private static readonly EventSystem.IntraObjectHandler<EarlyBird> OnDeathDelegate = new EventSystem.IntraObjectHandler<EarlyBird>(delegate(EarlyBird component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EarlyBird> OnRevivedDelegate = new EventSystem.IntraObjectHandler<EarlyBird>(delegate(EarlyBird component, object data)
	{
		component.OnRevived(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(-1117766961, OnRevivedDelegate);
	}

	protected override void OnSpawn()
	{
		attributeModifiers = new AttributeModifier[11]
		{
			new AttributeModifier("Construction", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Digging", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Machinery", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Athletics", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Learning", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Cooking", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Art", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Strength", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Caring", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Botanist", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true),
			new AttributeModifier("Ranching", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME, false, false, true)
		};
		base.smi.StartSM();
	}

	public void ApplyModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < attributeModifiers.Length; i++)
		{
			AttributeModifier modifier = attributeModifiers[i];
			attributes.Add(modifier);
		}
	}

	public void RemoveModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < attributeModifiers.Length; i++)
		{
			AttributeModifier modifier = attributeModifiers[i];
			attributes.Remove(modifier);
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}
}
