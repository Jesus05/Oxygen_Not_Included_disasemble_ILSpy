using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneShuffler : Workable
{
	public class GeneShufflerSM : GameStateMachine<GeneShufflerSM, GeneShufflerSM.Instance, GeneShuffler>
	{
		public class WorkingStates : State
		{
			public State pre;

			public State loop;

			public State complete;

			public State pst;
		}

		public new class Instance : GameInstance
		{
			public Instance(GeneShuffler master)
				: base(master)
			{
			}
		}

		public State idle;

		public WorkingStates working;

		public State consumed;

		public State recharging;

		public BoolParameter isCharged;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("on").Enter(delegate(Instance smi)
			{
				smi.master.SetAssignable(true);
			}).Exit(delegate(Instance smi)
			{
				smi.master.SetAssignable(false);
			})
				.WorkableStartTransition((Instance smi) => smi.master, working.pre)
				.ParamTransition(isCharged, consumed, GameStateMachine<GeneShufflerSM, Instance, GeneShuffler, object>.IsFalse);
			working.pre.PlayAnim("working_pre").OnAnimQueueComplete(working.loop);
			working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ScheduleGoTo(5f, working.complete);
			working.complete.ToggleStatusItem(Db.Get().BuildingStatusItems.GeneShuffleCompleted, null).Enter(delegate(Instance smi)
			{
				smi.master.RefreshSideScreen();
			}).WorkableStopTransition((Instance smi) => smi.master, working.pst);
			working.pst.EventTransition(GameHashes.AnimQueueComplete, consumed, null);
			consumed.PlayAnim("off", KAnim.PlayMode.Once).ParamTransition(isCharged, recharging, GameStateMachine<GeneShufflerSM, Instance, GeneShuffler, object>.IsTrue);
			recharging.PlayAnim("recharging", KAnim.PlayMode.Once).OnAnimQueueComplete(idle);
		}
	}

	[MyCmpReq]
	public Assignable assignable;

	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public ManualDeliveryKG delivery;

	[MyCmpReq]
	public Storage storage;

	[Serialize]
	public bool IsConsumed;

	[Serialize]
	public bool RechargeRequested;

	private Chore chore;

	private GeneShufflerSM.Instance geneShufflerSMI;

	private Notification notification;

	private static Tag RechargeTag = new Tag("GeneShufflerRecharge");

	private static readonly EventSystem.IntraObjectHandler<GeneShuffler> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<GeneShuffler>(delegate(GeneShuffler component, object data)
	{
		component.OnStorageChange(data);
	});

	private bool storage_recursion_guard;

	public bool WorkComplete => geneShufflerSMI.IsInsideState(geneShufflerSMI.sm.working.complete);

	public bool IsWorking => geneShufflerSMI.IsInsideState(geneShufflerSMI.sm.working);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		assignable.OnAssign += Assign;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		showProgressBar = false;
		geneShufflerSMI = new GeneShufflerSM.Instance(this);
		RefreshRechargeChore();
		RefreshConsumedState();
		Subscribe(-1697596308, OnStorageChangeDelegate);
		geneShufflerSMI.StartSM();
	}

	private void Assign(IAssignableIdentity new_assignee)
	{
		CancelChore();
		if (new_assignee != null)
		{
			ActivateChore();
		}
	}

	private void Recharge()
	{
		SetConsumed(false);
		RequestRecharge(false);
		RefreshRechargeChore();
		RefreshSideScreen();
	}

	private void SetConsumed(bool consumed)
	{
		IsConsumed = consumed;
		RefreshConsumedState();
	}

	private void RefreshConsumedState()
	{
		geneShufflerSMI.sm.isCharged.Set(!IsConsumed, geneShufflerSMI);
	}

	private void OnStorageChange(object data)
	{
		if (!storage_recursion_guard)
		{
			storage_recursion_guard = true;
			if (IsConsumed)
			{
				for (int num = storage.items.Count - 1; num >= 0; num--)
				{
					GameObject gameObject = storage.items[num];
					if (!((Object)gameObject == (Object)null) && gameObject.HasTag(RechargeTag))
					{
						storage.ConsumeIgnoringDisease(gameObject);
						Recharge();
						break;
					}
				}
			}
			storage_recursion_guard = false;
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		notification = new Notification(MISC.NOTIFICATIONS.GENESHUFFLER.NAME, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.GENESHUFFLER.TOOLTIP + notificationList.ReduceMessages(false), null, false, 0f, null, null, null);
		notifier.Add(notification, string.Empty);
		DeSelectBuilding();
	}

	private void DeSelectBuilding()
	{
		if (GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, true);
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		if (chore != null)
		{
			chore.Cancel("aborted");
		}
		notifier.Remove(notification);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (chore != null)
		{
			chore.Cancel("stopped");
		}
		notifier.Remove(notification);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		CameraController.Instance.CameraGoTo(base.transform.GetPosition(), 1f, false);
		ApplyRandomTrait(worker);
		assignable.Unassign();
		DeSelectBuilding();
		notifier.Remove(notification);
	}

	private void ApplyRandomTrait(Worker worker)
	{
		Traits component = worker.GetComponent<Traits>();
		List<string> list = new List<string>();
		foreach (DUPLICANTSTATS.TraitVal gENESHUFFLERTRAIT in DUPLICANTSTATS.GENESHUFFLERTRAITS)
		{
			DUPLICANTSTATS.TraitVal current = gENESHUFFLERTRAIT;
			if (!component.HasTrait(current.id))
			{
				list.Add(current.id);
			}
		}
		Trait trait = null;
		if (list.Count > 0)
		{
			string id = list[Random.Range(0, list.Count)];
			trait = Db.Get().traits.TryGet(id);
			worker.GetComponent<Traits>().Add(trait);
			InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text = string.Format(UI.GENESHUFFLERMESSAGE.BODY_SUCCESS, worker.GetProperName(), trait.Name, trait.GetTooltip());
			infoDialogScreen.SetHeader(UI.GENESHUFFLERMESSAGE.HEADER).AddPlainText(text);
			SetConsumed(true);
		}
		else
		{
			InfoDialogScreen infoDialogScreen2 = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text2 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_FAILURE, worker.GetProperName());
			infoDialogScreen2.SetHeader(UI.GENESHUFFLERMESSAGE.HEADER).AddPlainText(text2);
		}
	}

	private void ActivateChore()
	{
		GetComponent<Workable>().SetWorkTime(float.PositiveInfinity);
		chore = new WorkChore<Workable>(Db.Get().ChoreTypes.GeneShuffle, override_anims: Assets.GetAnim("anim_interacts_neuralvacillator_kanim"), target: this, chore_provider: null, chore_tags: null, run_until_complete: true, on_complete: delegate
		{
			CompleteChore();
		}, on_begin: null, on_end: null, allow_in_red_alert: true, schedule_block: null, ignore_schedule_block: false, only_when_operational: true, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, priority_class_value: 5, ignore_building_assignment: false, add_to_daily_report: true);
	}

	private void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void CompleteChore()
	{
		chore.Cleanup();
		chore = null;
	}

	public void RequestRecharge(bool request)
	{
		RechargeRequested = request;
		RefreshRechargeChore();
	}

	private void RefreshRechargeChore()
	{
		delivery.Pause(!RechargeRequested, "No recharge requested");
	}

	public void RefreshSideScreen()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (component.IsSelected)
		{
			DetailsScreen.Instance.Refresh(base.gameObject);
		}
	}

	public void SetAssignable(bool set_it)
	{
		assignable.SetCanBeAssigned(set_it);
		RefreshSideScreen();
	}
}
