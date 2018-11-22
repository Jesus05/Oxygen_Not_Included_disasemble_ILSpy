using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CommandModule, object>.GameInstance
	{
		public StatesInstance(CommandModule master)
			: base(master)
		{
		}

		public void SetSuspended(bool suspended)
		{
			Storage component = GetComponent<Storage>();
			if ((Object)component != (Object)null)
			{
				component.allowItemRemoval = !suspended;
			}
			ManualDeliveryKG component2 = GetComponent<ManualDeliveryKG>();
			if ((Object)component2 != (Object)null)
			{
				component2.Pause(suspended, "Rocket is suspended");
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CommandModule>
	{
		public class GroundedStates : State
		{
			public State idle;

			public State awaitingAstronaut;

			public State hasAstronaut;
		}

		public class SpaceborneStates : State
		{
			public State launch;

			public State idle;

			public State land;
		}

		public GroundedStates grounded;

		public SpaceborneStates spaceborne;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded.idle;
			grounded.DefaultState(grounded.idle).TagTransition(GameTags.RocketNotOnGround, spaceborne, false);
			grounded.idle.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(grounded.awaitingAstronaut);
			grounded.awaitingAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).EnterTransition(grounded.hasAstronaut, (StatesInstance smi) => smi.GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0).ToggleChore((StatesInstance smi) => smi.master.CreateWorkChore(), grounded.hasAstronaut);
			grounded.hasAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).EventHandler(GameHashes.AssigneeChanged, delegate(StatesInstance smi)
			{
				smi.master.ReleaseAstronaut();
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).EventTransition(GameHashes.AssigneeChanged, grounded.idle, null);
			spaceborne.DefaultState(spaceborne.launch);
			spaceborne.launch.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(true);
			}).GoTo(spaceborne.idle);
			spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, spaceborne.land, true);
			spaceborne.land.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(false);
				smi.master.ReleaseAstronaut();
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).GoTo(grounded);
		}
	}

	public Storage storage;

	public RocketStats rocketStats;

	private bool releasingAstronaut = false;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public ConditionDestinationReachable reachable;

	public ConditionHasAstronaut hasAstronaut;

	public ConditionHasAtmoSuit hasSuit;

	public CargoBayIsEmpty cargoEmpty;

	public ConditionFlightPathIsClear flightPathIsClear;

	public Assignable assignable;

	private CharacterOverlay characterOverlay;

	private HandleVector<int>.Handle partitionerEntry;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		characterOverlay = base.gameObject.AddComponent<CharacterOverlay>();
		characterOverlay.Register();
		rocketStats = new RocketStats(this);
	}

	public void ReleaseAstronaut()
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		storage = GetComponent<Storage>();
		assignable = GetComponent<Assignable>();
		assignable.eligibleFilter = delegate(MinionAssignablesProxy identity)
		{
			if (!(identity.target is MinionIdentity))
			{
				if (!(identity.target is StoredMinionIdentity))
				{
					return false;
				}
				return (identity.target as StoredMinionIdentity).HasPerk(RoleManager.rolePerks.CanUseRockets);
			}
			return (identity.target as KMonoBehaviour).GetComponent<MinionResume>().HasPerk(RoleManager.rolePerks.CanUseRockets);
		};
		base.smi.StartSM();
		int cell = Grid.OffsetCell(Grid.PosToCell(base.gameObject), 0, -1);
		partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnGantryChanged);
		OnGantryChanged(null);
		RocketModule component = GetComponent<RocketModule>();
		reachable = (ConditionDestinationReachable)component.AddLaunchCondition(new ConditionDestinationReachable(this));
		hasAstronaut = (ConditionHasAstronaut)component.AddLaunchCondition(new ConditionHasAstronaut(this));
		hasSuit = (ConditionHasAtmoSuit)component.AddLaunchCondition(new ConditionHasAtmoSuit(this));
		cargoEmpty = (CargoBayIsEmpty)component.AddLaunchCondition(new CargoBayIsEmpty(this));
		flightPathIsClear = (ConditionFlightPathIsClear)component.AddFlightCondition(new ConditionFlightPathIsClear(base.gameObject, 1));
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
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(astronaut, this, null, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.emergency, 5, false);
		workChore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanUseRockets);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, assignable);
		return workChore;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		partitionerEntry.Clear();
		ReleaseAstronaut();
		base.smi.StopSM("cleanup");
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
