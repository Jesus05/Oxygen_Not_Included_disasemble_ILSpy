using KSerialization;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureLure : StateMachineComponent<CreatureLure.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CreatureLure, object>.GameInstance
	{
		public StatesInstance(CreatureLure master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CreatureLure>
	{
		public State idle;

		public State working;

		public State empty;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, CreatureLure, object>.State.Callback _003C_003Ef__mg_0024cache0;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("off", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.activeBaitSetting != Tag.Invalid)
				{
					if (smi.master.baitStorage.IsEmpty())
					{
						smi.master.CreateFetchChore();
					}
					else if (smi.master.operational.IsOperational)
					{
						smi.GoTo(working);
					}
				}
			}).EventTransition(GameHashes.OnStorageChange, working, (StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, working, (StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid && smi.master.operational.IsOperational);
			working.Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, false);
				HashedString batchTag = ElementLoader.FindElementByName(smi.master.activeBaitSetting.ToString()).substance.anim.batchTag;
				KAnim.Build build = ElementLoader.FindElementByName(smi.master.activeBaitSetting.ToString()).substance.anim.GetData().build;
				KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
				HashedString target_symbol = "slime_mold";
				SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
				component.TryRemoveSymbolOverride(target_symbol, 0);
				component.AddSymbolOverride(target_symbol, symbol, 0);
				smi.GetSMI<Lure.Instance>().SetActiveLures(new Tag[1]
				{
					smi.master.activeBaitSetting
				});
			}).Exit(ClearBait).QueueAnim("working_pre", false, null)
				.QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid)
				.EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => !smi.master.operational.IsOperational && !smi.master.baitStorage.IsEmpty());
			empty.QueueAnim("working_pst", false, null).QueueAnim("off", false, null).Enter(delegate(StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			})
				.EventTransition(GameHashes.OnStorageChange, working, (StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, working, (StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational);
		}

		private static void ClearBait(Instance smi)
		{
			Lure.Instance sMI = smi.GetSMI<Lure.Instance>();
			if (sMI != null)
			{
				smi.GetSMI<Lure.Instance>().SetActiveLures(null);
			}
		}
	}

	public static float CONSUMPTION_RATE = 1f;

	[Serialize]
	public Tag activeBaitSetting;

	public List<Tag> baitTypes;

	public Storage baitStorage;

	protected FetchChore fetchChore;

	private Operational operational;

	private Operational.Flag baited = new Operational.Flag("Baited", Operational.Flag.Type.Requirement);

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<CreatureLure> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureLure>(delegate(CreatureLure component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CreatureLure> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CreatureLure>(delegate(CreatureLure component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		operational = GetComponent<Operational>();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		CreatureLure component = gameObject.GetComponent<CreatureLure>();
		if ((Object)component != (Object)null)
		{
			ChangeBaitSetting(component.activeBaitSetting);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (activeBaitSetting == Tag.Invalid)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, null);
		}
		else
		{
			ChangeBaitSetting(activeBaitSetting);
			OnStorageChange(null);
		}
		Subscribe(-1697596308, OnStorageChangeDelegate);
	}

	private void OnStorageChange(object data = null)
	{
		bool value = baitStorage.GetAmountAvailable(activeBaitSetting) > 0f;
		operational.SetFlag(baited, value);
	}

	public void ChangeBaitSetting(Tag baitSetting)
	{
		if (fetchChore != null)
		{
			fetchChore.Cancel("SwitchedResource");
		}
		if (baitSetting != activeBaitSetting)
		{
			activeBaitSetting = baitSetting;
			baitStorage.DropAll(false, false, default(Vector3), true);
		}
		base.smi.GoTo(base.smi.sm.idle);
		baitStorage.storageFilters = new List<Tag>
		{
			activeBaitSetting
		};
		if (baitSetting != Tag.Invalid)
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, false);
			if (base.smi.master.baitStorage.IsEmpty())
			{
				CreateFetchChore();
			}
		}
		else
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, null);
		}
	}

	protected void CreateFetchChore()
	{
		if (fetchChore != null)
		{
			fetchChore.Cancel("Overwrite");
		}
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, false);
		if (!(activeBaitSetting == Tag.Invalid))
		{
			fetchChore = new FetchChore(Db.Get().ChoreTypes.Transport, baitStorage, 100f, new Tag[1]
			{
				activeBaitSetting
			}, null, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0, null);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, null);
		}
	}
}
