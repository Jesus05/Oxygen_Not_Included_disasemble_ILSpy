using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FetchChore, object>.GameInstance
	{
		public StatesInstance(FetchChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FetchChore>
	{
		public TargetParameter fetcher;

		public TargetParameter source;

		public TargetParameter chunk;

		public TargetParameter destination;

		public FloatParameter requestedamount;

		public FloatParameter actualamount;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
		}
	}

	public Tag[] tags;

	public int tagBitsHash;

	public TagBits tagBits;

	public TagBits requiredTagBits;

	public TagBits forbiddenTagBits;

	public Automatable automatable;

	public bool allowMultifetch = true;

	private HandleVector<int>.Handle partitionerEntry;

	public static readonly Precondition IsFetchTargetAvailable = new Precondition
	{
		id = "IsFetchTargetAvailable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_FETCH_TARGET_AVAILABLE,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			FetchChore fetchChore = (FetchChore)context.chore;
			Pickupable pickupable = (Pickupable)context.data;
			bool flag = false;
			if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null)
			{
				pickupable = fetchChore.FindFetchTarget(context.consumerState);
				flag = ((UnityEngine.Object)pickupable != (UnityEngine.Object)null);
			}
			else
			{
				flag = FetchManager.IsFetchablePickup(pickupable.KPrefabID, pickupable.storage, pickupable.UnreservedAmount, ref fetchChore.tagBits, ref fetchChore.requiredTagBits, ref fetchChore.forbiddenTagBits, context.consumerState.storage);
			}
			if (flag)
			{
				if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null)
				{
					Debug.Log($"Failed to find fetch target for {fetchChore.destination}");
					return false;
				}
				context.data = pickupable;
				if (context.consumerState.consumer.GetNavigationCost(pickupable, out int cost))
				{
					context.cost += cost;
					return true;
				}
			}
			return false;
		}
	};

	public float originalAmount => base.smi.sm.requestedamount.Get(base.smi);

	public float amount
	{
		get
		{
			return base.smi.sm.actualamount.Get(base.smi);
		}
		set
		{
			base.smi.sm.actualamount.Set(value, base.smi);
		}
	}

	public Pickupable fetchTarget
	{
		get
		{
			return base.smi.sm.chunk.Get<Pickupable>(base.smi);
		}
		set
		{
			base.smi.sm.chunk.Set(value, base.smi);
		}
	}

	public GameObject fetcher
	{
		get
		{
			return base.smi.sm.fetcher.Get(base.smi);
		}
		set
		{
			base.smi.sm.fetcher.Set(value, base.smi);
		}
	}

	public Storage destination => base.smi.sm.destination.Get<Storage>(base.smi);

	public FetchChore(ChoreType choreType, Storage destination, float amount, Tag[] tags, Tag[] required_tags = null, Tag[] forbidden_tags = null, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, FetchOrder2.OperationalRequirement operational_requirement = FetchOrder2.OperationalRequirement.Operational, int priority_mod = 0, Tag[] chore_tags = null)
		: base(choreType, (IStateMachineTarget)destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, 5, false, true, priority_mod, chore_tags, false, ReportManager.ReportType.WorkTime)
	{
		if (choreType == null)
		{
			Debug.LogError("You must specify a chore type for fetching!");
		}
		if (amount <= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
		{
			DebugUtil.LogWarningArgs(string.Format("Chore {0} is requesting {1} {2} to {3}", choreType.Id, tags[0], amount, (!((UnityEngine.Object)destination != (UnityEngine.Object)null)) ? "to nowhere" : destination.name));
		}
		SetPrioritizable((!((UnityEngine.Object)destination.prioritizable != (UnityEngine.Object)null)) ? destination.GetComponent<Prioritizable>() : destination.prioritizable);
		base.smi = new StatesInstance(this);
		base.smi.sm.requestedamount.Set(amount, base.smi);
		base.smi.sm.destination.Set(destination, base.smi);
		this.tags = tags;
		tagBits = new TagBits(tags);
		requiredTagBits = new TagBits(required_tags);
		forbiddenTagBits = new TagBits(forbidden_tags);
		tagBitsHash = tagBits.GetHashCode();
		DebugUtil.DevAssert(!tagBits.HasAny(ref FetchManager.disallowedTagBits), "Fetch chore fetching invalid tags.");
		if (destination.GetOnlyFetchMarkedItems())
		{
			requiredTagBits.SetTag(GameTags.Garbage);
		}
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, destination);
		AddPrecondition(IsFetchTargetAvailable, null);
		Deconstructable component = base.target.GetComponent<Deconstructable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component);
		}
		BuildingEnabledButton component2 = base.target.GetComponent<BuildingEnabledButton>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component2);
		}
		if (operational_requirement != FetchOrder2.OperationalRequirement.None && (bool)destination.gameObject.GetComponent<Operational>())
		{
			if (operational_requirement == FetchOrder2.OperationalRequirement.Operational)
			{
				Operational component3 = destination.GetComponent<Operational>();
				if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				{
					AddPrecondition(ChorePreconditions.instance.IsOperational, component3);
				}
			}
			if (operational_requirement == FetchOrder2.OperationalRequirement.Functional)
			{
				Operational component4 = destination.GetComponent<Operational>();
				if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
				{
					AddPrecondition(ChorePreconditions.instance.IsFunctional, component4);
				}
			}
		}
		partitionerEntry = GameScenePartitioner.Instance.Add(destination.name, this, Grid.PosToCell(destination), GameScenePartitioner.Instance.fetchChoreLayer, null);
		destination.Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		automatable = destination.GetComponent<Automatable>();
		if ((bool)automatable)
		{
			AddPrecondition(ChorePreconditions.instance.IsAllowedByAutomation, automatable);
		}
	}

	public void FetchAreaBegin(Precondition.Context context, float amount_to_be_fetched)
	{
		amount = amount_to_be_fetched;
		base.smi.sm.fetcher.Set(context.consumerState.gameObject, base.smi);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, context.chore.choreType.Name, GameUtil.GetChoreName(this, context.data));
		base.Begin(context);
	}

	public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
	{
		if (is_success)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, base.choreType.Name, GameUtil.GetChoreName(this, pickupable));
			fetchTarget = pickupable;
			base.driver = driver;
			fetcher = driver.gameObject;
			Succeed("FetchAreaEnd");
		}
		else
		{
			SetOverrideTarget(null);
			Fail("FetchAreaFail");
		}
	}

	public Pickupable FindFetchTarget(ChoreConsumerState consumer_state)
	{
		Pickupable target = null;
		if ((UnityEngine.Object)destination != (UnityEngine.Object)null)
		{
			if (consumer_state.hasSolidTransferArm)
			{
				SolidTransferArm solidTransferArm = consumer_state.solidTransferArm;
				solidTransferArm.FindFetchTarget(destination, tagBits, requiredTagBits, forbiddenTagBits, originalAmount, ref target);
			}
			else
			{
				target = Game.Instance.fetchManager.FindFetchTarget(destination, ref tagBits, ref requiredTagBits, ref forbiddenTagBits, originalAmount);
			}
		}
		return target;
	}

	public override void Begin(Precondition.Context context)
	{
		Pickupable pickupable = (Pickupable)context.data;
		if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null)
		{
			pickupable = FindFetchTarget(context.consumerState);
		}
		base.smi.sm.source.Set(pickupable.gameObject, base.smi);
		pickupable.Subscribe(-1582839653, OnTagsChanged);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		Pickupable pickupable = base.smi.sm.source.Get<Pickupable>(base.smi);
		if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
		{
			pickupable.Unsubscribe(-1582839653, OnTagsChanged);
		}
		base.End(reason);
	}

	private void OnTagsChanged(object data)
	{
		if ((UnityEngine.Object)base.smi.sm.chunk.Get(base.smi) != (UnityEngine.Object)null)
		{
			Fail("Tags changed");
		}
	}

	public override void PrepareChore(ref Precondition.Context context)
	{
		context.chore = new FetchAreaChore(context);
	}

	public float AmountWaitingToFetch()
	{
		if ((UnityEngine.Object)fetcher == (UnityEngine.Object)null)
		{
			return originalAmount;
		}
		return amount;
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		if (base.smi.sm.destination.Get<Storage>(base.smi).GetOnlyFetchMarkedItems())
		{
			requiredTagBits.SetTag(GameTags.Garbage);
		}
		else
		{
			requiredTagBits.Clear(GameTags.Garbage);
		}
	}

	private void OnMasterPriorityChanged(PriorityScreen.PriorityClass priorityClass, int priority_value)
	{
		masterPriority.priority_class = priorityClass;
		masterPriority.priority_value = priority_value;
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Precondition.Context> succeeded_contexts, List<Precondition.Context> failed_contexts, bool is_attempting_override)
	{
	}

	public void CollectChoresFromGlobalChoreProvider(ChoreConsumerState consumer_state, List<Precondition.Context> succeeded_contexts, List<Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		base.CollectChores(consumer_state, succeeded_contexts, failed_contexts, is_attempting_override);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		Storage storage = base.smi.sm.destination.Get<Storage>(base.smi);
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			storage.Unsubscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		}
	}
}
