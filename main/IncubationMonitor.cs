using Klei;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IncubationMonitor : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>
{
	public class Def : BaseDef
	{
		public float baseIncubationRate;

		public Tag spawnedCreature;

		public override void Configure(GameObject prefab)
		{
			List<string> initialAmounts = prefab.GetComponent<Modifiers>().initialAmounts;
			initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
			initialAmounts.Add(Db.Get().Amounts.Incubation.Id);
			initialAmounts.Add(Db.Get().Amounts.Viability.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance incubation;

		public AmountInstance wildness;

		public AmountInstance viability;

		public EggIncubator incubator;

		public Effect incubatingEffect;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			incubation = Db.Get().Amounts.Incubation.Lookup(base.gameObject);
			Action<object> handler = OnStore;
			master.Subscribe(856640610, handler);
			master.Subscribe(1309017699, handler);
			Action<object> handler2 = OnOperationalChanged;
			master.Subscribe(1628751838, handler2);
			master.Subscribe(960378201, handler2);
			wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			wildness.value = wildness.GetMax();
			viability = Db.Get().Amounts.Viability.Lookup(base.gameObject);
			viability.value = viability.GetMax();
			float value = def.baseIncubationRate;
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				value = 33.3333321f;
			}
			AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, value, CREATURES.MODIFIERS.BASE_INCUBATION_RATE.NAME, false, false, true);
			incubatingEffect = new Effect("Incubating", CREATURES.MODIFIERS.INCUBATING.NAME, CREATURES.MODIFIERS.INCUBATING.TOOLTIP, 0f, true, false, false, null, 0f, null);
			incubatingEffect.Add(modifier);
		}

		public Storage GetStorage()
		{
			return (!((UnityEngine.Object)base.transform.parent != (UnityEngine.Object)null)) ? null : base.transform.parent.GetComponent<Storage>();
		}

		public void OnStore(object data)
		{
			Storage storage = data as Storage;
			bool stored = (bool)storage || (data != null && (bool)data);
			EggIncubator eggIncubator = (!(bool)storage) ? null : storage.GetComponent<EggIncubator>();
			UpdateIncubationState(stored, eggIncubator);
		}

		public void OnOperationalChanged(object data = null)
		{
			bool stored = base.gameObject.HasTag(GameTags.Stored);
			Storage storage = GetStorage();
			EggIncubator eggIncubator = (!(bool)storage) ? null : storage.GetComponent<EggIncubator>();
			UpdateIncubationState(stored, eggIncubator);
		}

		private void UpdateIncubationState(bool stored, EggIncubator incubator)
		{
			this.incubator = incubator;
			base.smi.sm.inIncubator.Set((UnityEngine.Object)incubator != (UnityEngine.Object)null, base.smi);
			bool value = stored && !(bool)incubator;
			base.smi.sm.isSuppressed.Set(value, base.smi);
			Operational operational = (!(bool)incubator) ? null : incubator.GetComponent<Operational>();
			bool value2 = (bool)incubator && ((UnityEngine.Object)operational == (UnityEngine.Object)null || operational.IsOperational);
			base.smi.sm.incubatorIsActive.Set(value2, base.smi);
		}

		public void ApplySongBuff()
		{
			GetComponent<Effects>().Add("EggSong", true);
		}

		public bool HasSongBuff()
		{
			return GetComponent<Effects>().HasEffect("EggSong");
		}
	}

	public BoolParameter incubatorIsActive;

	public BoolParameter inIncubator;

	public BoolParameter isSuppressed;

	public State incubating;

	public State entombed;

	public State suppressed;

	public State hatching_pre;

	public State hatching_pst;

	public State not_viable;

	private Effect suppressedEffect;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static StateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static StateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static StateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache6;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = incubating;
		root.Enter(delegate(Instance smi)
		{
			smi.OnOperationalChanged(null);
		});
		incubating.PlayAnim("idle", KAnim.PlayMode.Loop).Transition(hatching_pre, IsReadyToHatch, UpdateRate.SIM_1000ms).TagTransition(GameTags.Entombed, entombed, false)
			.ParamTransition(isSuppressed, suppressed, GameStateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.IsTrue)
			.ToggleEffect((Instance smi) => smi.incubatingEffect);
		entombed.TagTransition(GameTags.Entombed, incubating, true);
		suppressed.ToggleEffect((Instance smi) => suppressedEffect).ParamTransition(isSuppressed, incubating, GameStateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.IsFalse).TagTransition(GameTags.Entombed, entombed, false)
			.Transition(not_viable, NoLongerViable, UpdateRate.SIM_1000ms);
		hatching_pre.Enter(DropSelfFromStorage).PlayAnim("hatching_pre").OnAnimQueueComplete(hatching_pst);
		hatching_pst.Enter(SpawnBaby).PlayAnim("hatching_pst").OnAnimQueueComplete(null)
			.Exit(DeleteSelf);
		not_viable.Enter(SpawnGenericEgg).GoTo(null).Exit(DeleteSelf);
		suppressedEffect = new Effect("IncubationSuppressed", CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		suppressedEffect.Add(new AttributeModifier(Db.Get().Amounts.Viability.deltaAttribute.Id, -0.0166666675f, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME, false, false, true));
	}

	private static bool IsReadyToHatch(Instance smi)
	{
		if (!smi.gameObject.HasTag(GameTags.Entombed))
		{
			return smi.incubation.value >= smi.incubation.GetMax();
		}
		return false;
	}

	private static void SpawnBaby(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.def.spawnedCreature), position);
		gameObject.SetActive(true);
		gameObject.GetSMI<AnimInterruptMonitor.Instance>().Play("hatching_pst", KAnim.PlayMode.Once);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if ((UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
		Db.Get().Amounts.Wildness.Copy(gameObject, smi.gameObject);
		if ((UnityEngine.Object)smi.incubator != (UnityEngine.Object)null)
		{
			smi.incubator.StoreBaby(gameObject);
		}
		SpawnShell(smi);
		SaveLoader.Instance.saveManager.Unregister(smi.GetComponent<SaveLoadRoot>());
	}

	private static bool NoLongerViable(Instance smi)
	{
		if (!smi.gameObject.HasTag(GameTags.Entombed))
		{
			return smi.viability.value <= smi.viability.GetMin();
		}
		return false;
	}

	private static GameObject SpawnShell(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EggShell"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(true);
		return gameObject;
	}

	private static GameObject SpawnEggInnards(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RawEgg"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(true);
		return gameObject;
	}

	private static void SpawnGenericEgg(Instance smi)
	{
		SpawnShell(smi);
		GameObject gameObject = SpawnEggInnards(smi);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if ((UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
	}

	private static void DeleteSelf(Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	private static void DropSelfFromStorage(Instance smi)
	{
		if (!smi.sm.inIncubator.Get(smi))
		{
			Storage storage = smi.GetStorage();
			if ((bool)storage)
			{
				storage.Drop(smi.gameObject, true);
			}
			smi.gameObject.AddTag(GameTags.StoredPrivate);
		}
	}
}
