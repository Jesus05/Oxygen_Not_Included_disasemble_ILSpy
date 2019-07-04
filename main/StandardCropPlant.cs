using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
	public class AnimSet
	{
		public string grow;

		public string grow_pst;

		public string idle_full;

		public string wilt_base;

		public string harvest;
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

		[CompilerGenerated]
		private static Func<StatesInstance, string> _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static Func<StatesInstance, string> _003C_003Ef__mg_0024cache4;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = alive;
			State state = dead;
			string name = CREATURES.STATUSITEMS.DEAD.NAME;
			string tooltip = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.rm.Replanted && !UprootedMonitor.IsObjectUprooted(masterTarget.Get(smi)))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification, "");
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.idle).ToggleComponent<Growing>();
			alive.idle.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, alive.pre_fruiting, (StatesInstance smi) => smi.master.growing.ReachedNextHarvest()).EventTransition(GameHashes.CropSleep, alive.sleeping, IsSleeping)
				.PlayAnim((StatesInstance smi) => smi.master.anims.grow, KAnim.PlayMode.Paused)
				.Enter(RefreshPositionPercent)
				.Update(RefreshPositionPercent, UpdateRate.SIM_4000ms, false)
				.EventHandler(GameHashes.ConsumePlant, RefreshPositionPercent);
			alive.pre_fruiting.PlayAnim((StatesInstance smi) => smi.master.anims.grow_pst, KAnim.PlayMode.Once).TriggerOnEnter(GameHashes.BurstEmitDisease, null).EventTransition(GameHashes.AnimQueueComplete, alive.fruiting, null);
			alive.fruiting_lost.Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)smi.master.harvestable != (UnityEngine.Object)null)
				{
					smi.master.harvestable.SetCanBeHarvested(false);
				}
			}).GoTo(alive.idle);
			alive.wilting.PlayAnim(GetWiltAnim, KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.idle, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Harvest, alive.harvest, null);
			alive.sleeping.PlayAnim((StatesInstance smi) => smi.master.anims.grow, KAnim.PlayMode.Once).EventTransition(GameHashes.CropWakeUp, alive.idle, GameStateMachine<States, StatesInstance, StandardCropPlant, object>.Not(IsSleeping)).EventTransition(GameHashes.Harvest, alive.harvest, null)
				.EventTransition(GameHashes.Wilt, alive.wilting, null);
			alive.fruiting.DefaultState(alive.fruiting.fruiting_idle).EventTransition(GameHashes.Wilt, alive.wilting, null).EventTransition(GameHashes.Harvest, alive.harvest, null)
				.EventTransition(GameHashes.Grow, alive.fruiting_lost, (StatesInstance smi) => !smi.master.growing.ReachedNextHarvest());
			alive.fruiting.fruiting_idle.PlayAnim((StatesInstance smi) => smi.master.anims.idle_full, KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)smi.master.harvestable != (UnityEngine.Object)null)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				}
			}).Transition(alive.fruiting.fruiting_old, IsOld, UpdateRate.SIM_4000ms);
			alive.fruiting.fruiting_old.PlayAnim(GetWiltAnim, KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)smi.master.harvestable != (UnityEngine.Object)null)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				}
			}).Transition(alive.fruiting.fruiting_idle, GameStateMachine<States, StatesInstance, StandardCropPlant, object>.Not(IsOld), UpdateRate.SIM_4000ms);
			alive.harvest.PlayAnim((StatesInstance smi) => smi.master.anims.harvest, KAnim.PlayMode.Once).Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)GameScheduler.Instance != (UnityEngine.Object)null && (UnityEngine.Object)smi.master != (UnityEngine.Object)null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit, null, null);
				}
				if ((UnityEngine.Object)smi.master.harvestable != (UnityEngine.Object)null)
				{
					smi.master.harvestable.SetCanBeHarvested(false);
				}
			}).OnAnimQueueComplete(alive.idle);
		}

		private static string GetWiltAnim(StatesInstance smi)
		{
			float num = smi.master.growing.PercentOfCurrentHarvest();
			string str = (num < 0.75f) ? "1" : ((!(num < 1f)) ? "3" : "2");
			return smi.master.anims.wilt_base + str;
		}

		private static void RefreshPositionPercent(StatesInstance smi, float dt)
		{
			smi.master.RefreshPositionPercent();
		}

		private static void RefreshPositionPercent(StatesInstance smi)
		{
			smi.master.RefreshPositionPercent();
		}

		public bool IsOld(StatesInstance smi)
		{
			return smi.master.growing.PercentOldAge() > 0.5f;
		}

		public bool IsSleeping(StatesInstance smi)
		{
			return smi.master.GetSMI<CropSleepingMonitor.Instance>()?.IsSleeping() ?? false;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, StandardCropPlant, object>.GameInstance
	{
		public StatesInstance(StandardCropPlant master)
			: base(master)
		{
		}
	}

	[MyCmpReq]
	private Crop crop;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ReceptacleMonitor rm;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpGet]
	private Harvestable harvestable;

	public static AnimSet defaultAnimSet = new AnimSet
	{
		grow = "grow",
		grow_pst = "grow_pst",
		idle_full = "idle_full",
		wilt_base = "wilt",
		harvest = "harvest"
	};

	public AnimSet anims = defaultAnimSet;

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

	public void RefreshPositionPercent()
	{
		animController.SetPositionPercent(growing.PercentOfCurrentHarvest());
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = "";
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
