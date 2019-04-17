using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, StandardCropPlant, object>.GameInstance
	{
		public StatesInstance(StandardCropPlant master)
			: base(master)
		{
		}

		public bool IsOld()
		{
			return base.master.growing.PercentOldAge() > 0.5f;
		}

		public int WiltStage()
		{
			float num = base.master.growing.PercentOfCurrentHarvest();
			if (num < 0.75f)
			{
				return 1;
			}
			if (num < 1f)
			{
				return 2;
			}
			return 3;
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<CropSleepingMonitor.Instance>()?.IsSleeping() ?? false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, StandardCropPlant>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State idle;

			public State pre_fruiting;

			public State fruiting_lost;

			public State barren;

			public FruitingState fruiting;

			public State wilting;

			public State destroy;

			public State harvest;

			public State sleeping;
		}

		public class FruitingState : State
		{
			public State fruiting_idle;

			public State fruiting_old;
		}

		public AliveStates alive;

		public State dead;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, StandardCropPlant, object>.State.Callback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<StatesInstance, float> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, StandardCropPlant, object>.State.Callback _003C_003Ef__mg_0024cache2;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = alive;
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !UprootedMonitor.IsObjectUprooted(masterTarget.Get(smi)))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification, string.Empty);
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.idle).ToggleComponent<Growing>();
			alive.idle.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, alive.pre_fruiting, (StatesInstance smi) => smi.master.growing.ReachedNextHarvest()).EventTransition(GameHashes.CropSleep, alive.sleeping, (StatesInstance smi) => smi.IsSleeping())
				.PlayAnim("grow", KAnim.PlayMode.Paused)
				.Enter(RefreshPositionPercent)
				.Update(RefreshPositionPercent, UpdateRate.SIM_4000ms, false)
				.EventHandler(GameHashes.ConsumePlant, RefreshPositionPercent);
			alive.pre_fruiting.PlayAnim("grow_pst", KAnim.PlayMode.Once).TriggerOnEnter(GameHashes.BurstEmitDisease, null).EventTransition(GameHashes.AnimQueueComplete, alive.fruiting, null);
			alive.fruiting_lost.Enter(delegate(StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(false);
			}).GoTo(alive.idle);
			alive.wilting.PlayAnim("wilt", KAnim.PlayMode.Loop, (StatesInstance smi) => smi.WiltStage().ToString()).EventTransition(GameHashes.WiltRecover, alive.idle, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Harvest, alive.harvest, null);
			alive.sleeping.PlayAnim("grow").EventTransition(GameHashes.CropWakeUp, alive.idle, (StatesInstance smi) => !smi.IsSleeping()).EventTransition(GameHashes.Harvest, alive.harvest, null)
				.EventTransition(GameHashes.Wilt, alive.wilting, null);
			alive.fruiting.DefaultState(alive.fruiting.fruiting_idle).EventTransition(GameHashes.Wilt, alive.wilting, null).EventTransition(GameHashes.Harvest, alive.harvest, null)
				.EventTransition(GameHashes.Grow, alive.fruiting_lost, (StatesInstance smi) => !smi.master.growing.ReachedNextHarvest());
			alive.fruiting.fruiting_idle.PlayAnim("idle_full", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Update("fruiting_idle", delegate(StatesInstance smi, float dt)
			{
				if (smi.IsOld())
				{
					smi.GoTo(alive.fruiting.fruiting_old);
				}
			}, UpdateRate.SIM_4000ms, false);
			alive.fruiting.fruiting_old.PlayAnim("wilt", KAnim.PlayMode.Loop, (StatesInstance smi) => smi.WiltStage().ToString()).Enter(delegate(StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Update("fruiting_old", delegate(StatesInstance smi, float dt)
			{
				if (!smi.IsOld())
				{
					smi.GoTo(alive.fruiting.fruiting_idle);
				}
			}, UpdateRate.SIM_4000ms, false);
			alive.harvest.PlayAnim("harvest", KAnim.PlayMode.Once).Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)GameScheduler.Instance != (UnityEngine.Object)null && (UnityEngine.Object)smi.master != (UnityEngine.Object)null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit, null, null);
				}
				smi.master.harvestable.SetCanBeHarvested(false);
			}).OnAnimQueueComplete(alive.idle);
		}
	}

	[MyCmpReq]
	private Crop crop;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private Harvestable harvestable;

	[MyCmpReq]
	private KAnimControllerBase animController;

	private static void RefreshPositionPercent(StatesInstance smi, float dt)
	{
		RefreshPositionPercent(smi);
	}

	private static void RefreshPositionPercent(StatesInstance smi)
	{
		smi.master.animController.SetPositionPercent(smi.master.growing.PercentOfCurrentHarvest());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.Get<KBatchedAnimController>().randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public Notification CreateDeathNotification()
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.gameObject.GetProperName(), true, 0f, null, null, null);
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = string.Empty;
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP, text);
	}
}
