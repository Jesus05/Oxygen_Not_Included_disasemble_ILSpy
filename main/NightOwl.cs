using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class NightOwl : StateMachineComponent<NightOwl.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, NightOwl, object>.GameInstance
	{
		public StatesInstance(NightOwl master)
			: base(master)
		{
		}

		public bool IsNight()
		{
			if (!((Object)GameClock.Instance == (Object)null) && !(base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview))
			{
				return GameClock.Instance.IsNighttime();
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, NightOwl>
	{
		public State idle;

		public State early;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.Transition(early, (StatesInstance smi) => smi.IsNight(), UpdateRate.SIM_200ms);
			early.Enter("Night", delegate(StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotNight", delegate(StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.NightTime, (object)null)
				.ToggleExpression(Db.Get().Expressions.Happy, null)
				.Transition(idle, (StatesInstance smi) => !smi.IsNight(), UpdateRate.SIM_200ms);
		}
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	private static readonly EventSystem.IntraObjectHandler<NightOwl> OnDeathDelegate = new EventSystem.IntraObjectHandler<NightOwl>(delegate(NightOwl component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<NightOwl> OnRevivedDelegate = new EventSystem.IntraObjectHandler<NightOwl>(delegate(NightOwl component, object data)
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
			new AttributeModifier("Construction", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Digging", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Machinery", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Athletics", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Learning", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Cooking", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Art", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Strength", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Caring", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Botanist", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Ranching", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true)
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

	public void ModifyTrait(Trait t)
	{
	}
}
