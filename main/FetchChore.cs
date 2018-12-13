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
					Debug.Log($"Failed to find fetch target for {fetchChore.destination}", null);
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

	public float originalAmount => smi.sm.requestedamount.Get(smi);

	public float amount
	{
		get
		{
			return smi.sm.actualamount.Get(smi);
		}
		set
		{
			smi.sm.actualamount.Set(value, smi);
		}
	}

	public Pickupable fetchTarget
	{
		get
		{
			return smi.sm.chunk.Get<Pickupable>(smi);
		}
		set
		{
			smi.sm.chunk.Set(value, smi);
		}
	}

	public GameObject fetcher
	{
		get
		{
			return smi.sm.fetcher.Get(smi);
		}
		set
		{
			smi.sm.fetcher.Set(value, smi);
		}
	}

	public Storage destination => smi.sm.destination.Get<Storage>(smi);

	public FetchChore(ChoreType choreType, Storage destination, float amount, Tag[] tags, Tag[] required_tags = null, Tag[] forbidden_tags = null, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, FetchOrder2.OperationalRequirement operational_requirement = FetchOrder2.OperationalRequirement.Operational, int priority_mod = 0, Tag[] chore_tags = null)
		: base(choreType, (IStateMachineTarget)destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, 5, false, true, priority_mod, chore_tags)
	{
		if (choreType == null)
		{
			Output.LogError("You must specify a chore type for fetching!");
		}
		if (amount <= 0f)
		{
			Output.LogError("Requesting an invalid FetchChore amount");
		}
		SetPrioritizable((!((UnityEngine.Object)destination.prioritizable != (UnityEngine.Object)null)) ? destination.GetComponent<Prioritizable>() : destination.prioritizable);
		smi = new StatesInstance(this);
		smi.sm.requestedamount.Set(amount, smi);
		smi.sm.destination.Set(destination, smi);
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
		smi.sm.fetcher.Set(context.consumerState.gameObject, smi);
		base.Begin(context);
	}

	public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
	{
		if (is_success)
		{
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
		smi.sm.source.Set(pickupable.gameObject, smi);
		pickupable.Subscribe(-1582839653, OnTagsChanged);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		Pickupable pickupable = smi.sm.source.Get<Pickupable>(smi);
		if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
		{
			pickupable.Unsubscribe(-1582839653, OnTagsChanged);
		}
		base.End(reason);
	}

	private void OnTagsChanged(object data)
	{
		if ((UnityEngine.Object)smi.sm.chunk.Get(smi) != (UnityEngine.Object)null)
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
		if (smi.sm.destination.Get<Storage>(smi).GetOnlyFetchMarkedItems())
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
		Storage storage = smi.sm.destination.Get<Storage>(smi);
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			storage.Unsubscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		}
	}
}
