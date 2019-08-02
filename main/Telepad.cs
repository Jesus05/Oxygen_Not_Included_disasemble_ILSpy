using Klei.AI;
using STRINGS;
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

		public Signal idlePortal;

		public State idle;

		public State resetToIdle;

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
			root.OnSignal(idlePortal, resetToIdle);
			resetToIdle.GoTo(idle);
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

	public float startingSkillPoints;

	public static readonly HashedString[] PortalBirthAnim = new HashedString[1]
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
			Debug.LogError("Headquarters spawned at: (" + x.ToString() + "," + y.ToString() + ")");
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
				Immigration.Instance.EndImmigration();
			}
		}
	}

	public void RejectAll()
	{
		Immigration.Instance.EndImmigration();
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public void OnAcceptDelivery(ITelepadDeliverable delivery)
	{
		int cell = Grid.PosToCell(this);
		Immigration.Instance.EndImmigration();
		GameObject gameObject = delivery.Deliver(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
		if ((Object)component != (Object)null)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.PersonalTime, GameClock.Instance.GetTimeSinceStartOfReport(), string.Format(UI.ENDOFDAYREPORT.NOTES.PERSONAL_TIME, DUPLICANTS.CHORES.NOT_EXISTING_TASK), gameObject.GetProperName());
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				item.GetComponent<Effects>().Add("NewCrewArrival", true);
			}
			MinionResume component2 = component.GetComponent<MinionResume>();
			for (int i = 0; (float)i < startingSkillPoints; i++)
			{
				component2.ForceAddSkillPoint();
			}
		}
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public float GetTimeRemaining()
	{
		return Immigration.Instance.GetTimeRemaining();
	}
}
