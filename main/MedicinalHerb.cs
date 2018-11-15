using STRINGS;
using UnityEngine;

public class MedicinalHerb : StateMachineComponent<MedicinalHerb.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MedicinalHerb, object>.GameInstance
	{
		public StatesInstance(MedicinalHerb smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MedicinalHerb>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State seed_grow;

			public State idle;

			public FruitingState fruiting;

			public WiltingState wilting;

			public State destroy;
		}

		public class FruitingState : State
		{
			public State fruiting_pre;

			public State fruiting_idle;

			public State fruiting_harvest;
		}

		public class WiltingState : State
		{
			public State wilting_pre;

			public State wilting;

			public State wilting_pst;
		}

		public State blocked_from_growing;

		public AliveStates alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = alive;
			base.serializable = true;
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, (object)null).EventTransition(GameHashes.EntombedChanged, alive.seed_grow, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, alive.seed_grow, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, alive.seed_grow, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, dead, (StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.seed_grow).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
			});
			alive.seed_grow.QueueAnim("seed_grow", false, null).EventTransition(GameHashes.AnimQueueComplete, alive.idle, null).EventTransition(GameHashes.Wilt, alive.wilting.wilting_pre, (StatesInstance smi) => smi.master.wiltCondition.IsWilting())
				.EventTransition(GameHashes.CropReady, alive.fruiting.fruiting_pre, null);
			alive.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.CropDepleted, dead, (StatesInstance smi) => !smi.master.crop.CanGrow()).EventTransition(GameHashes.Wilt, alive.wilting.wilting_pre, (StatesInstance smi) => smi.master.wiltCondition.IsWilting())
				.EventTransition(GameHashes.CropReady, alive.fruiting.fruiting_pre, null);
			alive.wilting.wilting_pre.PlayAnim("wilt_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.wilting.wilting).EventTransition(GameHashes.WiltRecover, alive.wilting.wilting_pst, null);
			alive.wilting.wilting.PlayAnim("idle_wilt_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.wilting.wilting_pst, null);
			alive.wilting.wilting_pst.PlayAnim("wilt_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.idle);
			alive.fruiting.EventTransition(GameHashes.Harvest, alive.fruiting.fruiting_harvest, null).EventHandler(GameHashes.Wilt, delegate(StatesInstance smi)
			{
				smi.master.crop.SpawnFruit(null);
				smi.master.harvestable.SetCanBeHarvested(false);
				smi.GoTo(alive.wilting.wilting_pre);
			});
			alive.fruiting.fruiting_pre.PlayAnim("grow").OnAnimQueueComplete(alive.fruiting.fruiting_idle);
			alive.fruiting.fruiting_idle.PlayAnim("idle_bloom_loop", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			});
			alive.fruiting.fruiting_harvest.PlayAnim("harvest").Enter(delegate(StatesInstance smi)
			{
				if ((Object)GameScheduler.Instance != (Object)null && (Object)smi.master != (Object)null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.4f, smi.master.crop.SpawnFruit, null, null);
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		crop = GetComponent<Crop>();
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}
}
