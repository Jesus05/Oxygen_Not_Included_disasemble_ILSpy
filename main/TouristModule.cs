using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TouristModule : StateMachineComponent<TouristModule.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TouristModule, object>.GameInstance
	{
		public StatesInstance(TouristModule smi)
			: base(smi)
		{
			smi.gameObject.Subscribe(238242047, delegate
			{
				smi.SetSuspended(false);
				smi.ReleaseAstronaut(null);
			});
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TouristModule>
	{
		public State idle;

		public State awaitingTourist;

		public State hasTourist;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(awaitingTourist);
			awaitingTourist.PlayAnim("grounded", KAnim.PlayMode.Loop).ToggleChore((StatesInstance smi) => smi.master.CreateWorkChore(), hasTourist);
			hasTourist.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.LandRocket, idle, null).EventTransition(GameHashes.AssigneeChanged, idle, null);
		}
	}

	public Storage storage;

	[Serialize]
	private bool isSuspended;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public Assignable assignable;

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<TouristModule> OnSuspendDelegate = new EventSystem.IntraObjectHandler<TouristModule>(delegate(TouristModule component, object data)
	{
		component.OnSuspend(data);
	});

	private static readonly EventSystem.IntraObjectHandler<TouristModule> OnAssigneeChangedDelegate = new EventSystem.IntraObjectHandler<TouristModule>(delegate(TouristModule component, object data)
	{
		component.OnAssigneeChanged(data);
	});

	public bool IsSuspended => isSuspended;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void SetSuspended(bool state)
	{
		isSuspended = state;
	}

	public void ReleaseAstronaut(object data)
	{
		if (!releasingAstronaut)
		{
			releasingAstronaut = true;
			MinionStorage component = GetComponent<MinionStorage>();
			List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
			for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
			{
				MinionStorage.Info info = storedMinionInfo[num];
				GameObject gameObject = component.DeserializeMinion(info.id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
				if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
				{
					gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
				}
			}
			releasingAstronaut = false;
		}
	}

	public void OnSuspend(object data)
	{
		Storage component = GetComponent<Storage>();
		if ((Object)component != (Object)null)
		{
			component.capacityKg = component.MassStored();
			component.allowItemRemoval = false;
		}
		if ((Object)GetComponent<ManualDeliveryKG>() != (Object)null)
		{
			Object.Destroy(GetComponent<ManualDeliveryKG>());
		}
		SetSuspended(true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		storage = GetComponent<Storage>();
		assignable = GetComponent<Assignable>();
		assignable.eligibleFilter = ((MinionIdentity identity) => true);
		base.smi.StartSM();
		int cell = Grid.OffsetCell(Grid.PosToCell(base.gameObject), 0, -1);
		partitionerEntry = GameScenePartitioner.Instance.Add("TouristModule.gantryChanged", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnGantryChanged);
		OnGantryChanged(null);
		Subscribe(-1056989049, OnSuspendDelegate);
		Subscribe(684616645, OnAssigneeChangedDelegate);
	}

	private void OnGantryChanged(object data)
	{
		if ((Object)base.gameObject != (Object)null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry, false);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry, false);
			if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry, null);
			}
		}
	}

	private Chore CreateWorkChore()
	{
		ChoreType astronaut = Db.Get().ChoreTypes.Astronaut;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(astronaut, this, null, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.emergency, 0, false);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, assignable);
		return workChore;
	}

	private void OnAssigneeChanged(object data)
	{
		if (GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0)
		{
			ReleaseAstronaut(null);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		partitionerEntry.Clear();
		ReleaseAstronaut(null);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.smi.StopSM("cleanup");
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
