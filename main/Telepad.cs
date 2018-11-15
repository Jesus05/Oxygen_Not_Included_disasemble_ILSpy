using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class Telepad : StateMachineComponent<Telepad.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Telepad, object>.GameInstance
	{
		public StatesInstance(Telepad master)
			: base(master)
		{
		}

		public bool IsColonyLost()
		{
			return (Object)GameFlowManager.Instance != (Object)null && GameFlowManager.Instance.IsGameOver();
		}

		public void UpdateMeter()
		{
			float timeRemaining = Immigration.Instance.GetTimeRemaining();
			float totalWaitTime = Immigration.Instance.GetTotalWaitTime();
			float positionPercent = Mathf.Clamp01(1f - timeRemaining / totalWaitTime);
			base.master.meter.SetPositionPercent(positionPercent);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Telepad>
	{
		public Signal openPortal;

		public Signal closePortal;

		public State idle;

		public State opening;

		public State open;

		public State close;

		public State unoperational;

		private static readonly HashedString[] workingAnims = new HashedString[2]
		{
			"working_loop",
			"working_pst"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			base.serializable = true;
			idle.Enter(delegate(StatesInstance smi)
			{
				smi.UpdateMeter();
			}).Update("TelepadMeter", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateMeter();
			}, UpdateRate.SIM_4000ms, false).EventTransition(GameHashes.OperationalChanged, unoperational, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.PlayAnim("idle")
				.OnSignal(openPortal, opening);
			unoperational.PlayAnim("idle").Enter("StopImmigration", delegate(StatesInstance smi)
			{
				Immigration.Instance.Stop();
				smi.master.meter.SetPositionPercent(0f);
			}).Exit("StartImmigration", delegate
			{
				Immigration.Instance.Restart();
			})
				.EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			opening.Enter(delegate(StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(1f);
			}).PlayAnim("working_pre").OnAnimQueueComplete(open);
			open.OnSignal(closePortal, close).Enter(delegate(StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(1f);
			}).PlayAnim("working_loop", KAnim.PlayMode.Loop)
				.Transition(close, (StatesInstance smi) => smi.IsColonyLost(), UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OperationalChanged, close, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			close.Enter(delegate(StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(0f);
			}).PlayAnims((StatesInstance smi) => workingAnims, KAnim.PlayMode.Once).OnAnimQueueComplete(idle);
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	private MeterController meter;

	private const float MAX_IMMIGRATION_TIME = 120f;

	private const int NUM_METER_NOTCHES = 8;

	private List<MinionStartingStats> minionStats;

	private static readonly HashedString[] PortalBirthAnim = new HashedString[1]
	{
		"portalbirth"
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<Deconstructable>().allowDeconstruction = false;
		int x = 0;
		int y = 0;
		Grid.CellToXY(Grid.PosToCell(this), out x, out y);
		if (x == 0)
		{
			Debug.LogError("Headquarters spawned at: (" + x.ToString() + "," + y.ToString() + ")", null);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Telepads.Add(this);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimController>().SetDirty();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		Components.Telepads.Remove(this);
		base.OnCleanUp();
	}

	public void Update()
	{
		if (!base.smi.IsColonyLost())
		{
			if (Immigration.Instance.ImmigrantsAvailable)
			{
				base.smi.sm.openPortal.Trigger(base.smi);
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.NewDuplicantsAvailable, this);
			}
			else
			{
				base.smi.sm.closePortal.Trigger(base.smi);
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Wattson, this);
			}
			if (GetTimeRemaining() < -120f)
			{
				Messenger.Instance.QueueMessage(new DuplicantsLeftMessage());
				Immigration.Instance.SpawnMinions();
			}
		}
	}

	public void RejectAll()
	{
		Immigration.Instance.SpawnMinions();
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public void OnClickImmigrant(MinionStartingStats starting_stats)
	{
		int cell = Grid.PosToCell(this);
		int num = Immigration.Instance.SpawnMinions();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			item.GetComponent<Effects>().Add("NewCrewArrival", true);
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
			gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
			gameObject.SetActive(true);
			starting_stats.Apply(gameObject);
			Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
			ChoreProvider component = gameObject.GetComponent<ChoreProvider>();
			new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", PortalBirthAnim, null);
		}
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public float GetTimeRemaining()
	{
		return Immigration.Instance.GetTimeRemaining();
	}
}
