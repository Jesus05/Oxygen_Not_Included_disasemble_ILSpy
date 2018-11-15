using Klei;
using Klei.AI;
using STRINGS;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BabyMonitor : GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>
{
	public class Def : BaseDef
	{
		public Tag adultPrefab;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void SpawnAdult()
		{
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.smi.def.adultPrefab), position);
			gameObject.SetActive(true);
			gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("growup_pst");
			foreach (AmountInstance amount in base.gameObject.GetAmounts())
			{
				AmountInstance amountInstance = amount.amount.Lookup(gameObject);
				if (amountInstance != null)
				{
					float num = amount.value / amount.GetMax();
					amountInstance.value = num * amountInstance.GetMax();
				}
			}
			gameObject.Trigger(-2027483228, base.gameObject);
			KSelectable component = base.gameObject.GetComponent<KSelectable>();
			if ((Object)SelectTool.Instance != (Object)null && (Object)SelectTool.Instance.selected != (Object)null && (Object)SelectTool.Instance.selected == (Object)component)
			{
				SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
			}
			base.smi.gameObject.DeleteObject();
		}
	}

	public State baby;

	public State spawnadult;

	public Effect babyEffect;

	[CompilerGenerated]
	private static StateMachine<BabyMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = baby;
		root.Enter(AddBabyEffect);
		baby.Transition(spawnadult, IsReadyToSpawnAdult, UpdateRate.SIM_4000ms);
		spawnadult.ToggleBehaviour(GameTags.Creatures.Behaviours.GrowUpBehaviour, (Instance smi) => true, null);
		babyEffect = new Effect("IsABaby", CREATURES.MODIFIERS.BABY.NAME, CREATURES.MODIFIERS.BABY.TOOLTIP, 0f, true, false, false, null, 0f, null);
		babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -0.9f, CREATURES.MODIFIERS.BABY.NAME, true, false, true));
		babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, CREATURES.MODIFIERS.BABY.NAME, false, false, true));
	}

	private static void AddBabyEffect(Instance smi)
	{
		smi.Get<Effects>().Add(smi.sm.babyEffect, false);
	}

	private static bool IsReadyToSpawnAdult(Instance smi)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(smi.gameObject);
		float num = 5f;
		if (GenericGameSettings.instance.acceleratedLifecycle)
		{
			num = 0.005f;
		}
		return amountInstance.value > num;
	}
}
