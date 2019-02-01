using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SocialGatheringPoint : StateMachineComponent<SocialGatheringPoint.StatesInstance>
{
	public class States : GameStateMachine<States, StatesInstance, SocialGatheringPoint>
	{
		public State off;

		public State on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.TagTransition(GameTags.Operational, on, false);
			on.TagTransition(GameTags.Operational, off, true).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.master.tracker.Update(true);
			}).Exit("CancelChore", delegate(StatesInstance smi)
			{
				smi.master.tracker.Update(false);
			});
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SocialGatheringPoint, object>.GameInstance
	{
		public StatesInstance(SocialGatheringPoint smi)
			: base(smi)
		{
		}
	}

	public CellOffset[] choreOffsets = new CellOffset[2]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public int basePriority;

	public string socialEffect;

	public float workTime = 15f;

	public System.Action OnSocializeBeginCB;

	public System.Action OnSocializeEndCB;

	private SocialChoreTracker tracker;

	private SocialGatheringPointWorkable[] workables;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		workables = new SocialGatheringPointWorkable[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("SocialGatheringPointWorkable", pos);
			SocialGatheringPointWorkable socialGatheringPointWorkable = go.AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.basePriority = basePriority;
			socialGatheringPointWorkable.specificEffect = socialEffect;
			socialGatheringPointWorkable.OnWorkableEventCB = OnWorkableEvent;
			socialGatheringPointWorkable.SetWorkTime(workTime);
			workables[i] = socialGatheringPointWorkable;
		}
		tracker = new SocialChoreTracker(base.gameObject, choreOffsets);
		tracker.choreCount = choreCount;
		tracker.CreateChoreCB = CreateChore;
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (tracker != null)
		{
			tracker.Clear();
			tracker = null;
		}
		for (int i = 0; i < workables.Length; i++)
		{
			if ((bool)workables[i])
			{
				Util.KDestroyGameObject(workables[i]);
				workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = workables[i];
		ChoreType relax = Db.Get().ChoreTypes.Relax;
		Workable target = workable;
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		Chore chore = new WorkChore<SocialGatheringPointWorkable>(relax, target, null, null, true, null, null, OnSocialChoreEnd, false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, false);
		chore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.smi.IsInsideState(base.smi.sm.on))
		{
			tracker.Update(true);
		}
	}

	private void OnWorkableEvent(Workable.WorkableEvent workable_event)
	{
		switch (workable_event)
		{
		case Workable.WorkableEvent.WorkStarted:
			if (OnSocializeBeginCB != null)
			{
				OnSocializeBeginCB();
			}
			break;
		case Workable.WorkableEvent.WorkStopped:
			if (OnSocializeEndCB != null)
			{
				OnSocializeEndCB();
			}
			break;
		}
	}
}
