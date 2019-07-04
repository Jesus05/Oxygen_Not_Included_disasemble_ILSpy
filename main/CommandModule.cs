using Klei.AI;
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

		public bool CheckStoredMinionIsAssignee()
		{
			foreach (MinionStorage.Info item in GetComponent<MinionStorage>().GetStoredMinionInfo())
			{
				MinionStorage.Info current = item;
				if (current.serializedMinion != null)
				{
					KPrefabID kPrefabID = current.serializedMinion.Get();
					if (!((Object)kPrefabID == (Object)null))
					{
						StoredMinionIdentity component = kPrefabID.GetComponent<StoredMinionIdentity>();
						Assignable component2 = GetComponent<Assignable>();
						if (component2.assignee == component.assignableProxy.Get())
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CommandModule>
	{
		public class GroundedStates : State
		{
			public State awaitingAstronaut;

			public State hasAstronaut;

			public State waitingToRelease;
		}

		public class SpaceborneStates : State
		{
			public State launch;

			public State idle;

			public State land;
		}

		public Signal gantryChanged;

		public BoolParameter accumulatedPee;

		public GroundedStates grounded;

		public SpaceborneStates spaceborne;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			grounded.PlayAnim("grounded", KAnim.PlayMode.Loop).DefaultState(grounded.awaitingAstronaut).TagTransition(GameTags.RocketNotOnGround, spaceborne, false);
			grounded.awaitingAstronaut.Enter(delegate(StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.hasAstronaut);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).EventHandler(GameHashes.AssigneeChanged, delegate(StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.hasAstronaut);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).ToggleChore((StatesInstance smi) => smi.master.CreateWorkChore(), grounded.hasAstronaut);
			grounded.hasAstronaut.EventHandler(GameHashes.AssigneeChanged, delegate(StatesInstance smi)
			{
				if (!smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.waitingToRelease);
				}
			});
			grounded.waitingToRelease.ToggleStatusItem(Db.Get().BuildingStatusItems.DisembarkingDuplicant, (object)null).OnSignal(gantryChanged, grounded.awaitingAstronaut, delegate(StatesInstance smi)
			{
				if (!HasValidGantry(smi.gameObject))
				{
					return false;
				}
				smi.master.ReleaseAstronaut(accumulatedPee.Get(smi));
				accumulatedPee.Set(false, smi);
				Game.Instance.userMenu.Refresh(smi.gameObject);
				return true;
			});
			spaceborne.DefaultState(spaceborne.launch);
			spaceborne.launch.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(true);
			}).GoTo(spaceborne.idle);
			spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, spaceborne.land, true);
			spaceborne.land.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(false);
				Game.Instance.userMenu.Refresh(smi.gameObject);
				accumulatedPee.Set(true, smi);
			}).GoTo(grounded.waitingToRelease);
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

	public ConditionHasMinimumMass destHasResources;

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

	public void ReleaseAstronaut(bool fill_bladder)
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
				if (!((Object)gameObject == (Object)null))
				{
					if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
					{
						gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					}
					if (fill_bladder)
					{
						AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(gameObject);
						if (amountInstance != null)
						{
							amountInstance.value = amountInstance.GetMax();
						}
					}
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
		assignable.AddAssignPrecondition(CanAssignTo);
		base.smi.StartSM();
		int cell = Grid.PosToCell(base.gameObject);
		partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", base.gameObject, cell, GameScenePartitioner.Instance.validNavCellChangedLayer, OnGantryChanged);
		OnGantryChanged(null);
		RocketModule component = GetComponent<RocketModule>();
		reachable = (ConditionDestinationReachable)component.AddLaunchCondition(new ConditionDestinationReachable(this));
		hasAstronaut = (ConditionHasAstronaut)component.AddLaunchCondition(new ConditionHasAstronaut(this));
		hasSuit = (ConditionHasAtmoSuit)component.AddLaunchCondition(new ConditionHasAtmoSuit(this));
		cargoEmpty = (CargoBayIsEmpty)component.AddLaunchCondition(new CargoBayIsEmpty(this));
		destHasResources = (ConditionHasMinimumMass)component.AddLaunchCondition(new ConditionHasMinimumMass(this));
		flightPathIsClear = (ConditionFlightPathIsClear)component.AddFlightCondition(new ConditionFlightPathIsClear(base.gameObject, 1));
	}

	private bool CanAssignTo(MinionAssignablesProxy worker)
	{
		if (!(worker.target is MinionIdentity))
		{
			if (!(worker.target is StoredMinionIdentity))
			{
				return false;
			}
			return (worker.target as StoredMinionIdentity).HasPerk(Db.Get().SkillPerks.CanUseRockets);
		}
		return (worker.target as KMonoBehaviour).GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanUseRockets);
	}

	private static bool HasValidGantry(GameObject go)
	{
		int i = Grid.OffsetCell(Grid.PosToCell(go), 0, -1);
		return Grid.FakeFloor[i];
	}

	private void OnGantryChanged(object data)
	{
		if ((Object)base.gameObject != (Object)null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry, false);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry, false);
			if (HasValidGantry(base.smi.master.gameObject))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry, null);
			}
			base.smi.sm.gantryChanged.Trigger(base.smi);
		}
	}

	private Chore CreateWorkChore()
	{
		ChoreType astronaut = Db.Get().ChoreTypes.Astronaut;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(astronaut, this, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, true);
		workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRockets);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, assignable);
		return workChore;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		partitionerEntry.Clear();
		ReleaseAstronaut(false);
		base.smi.StopSM("cleanup");
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
