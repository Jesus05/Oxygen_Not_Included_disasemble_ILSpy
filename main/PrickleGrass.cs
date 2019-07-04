using STRINGS;
using UnityEngine;

public class PrickleGrass : StateMachineComponent<PrickleGrass.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PrickleGrass, object>.GameInstance
	{
		public StatesInstance(PrickleGrass smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PrickleGrass>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State idle;

			public WiltingState wilting;
		}

		public class WiltingState : State
		{
			public State wilting_pre;

			public State wilting;

			public State wilting_pst;
		}

		public State grow;

		public State blocked_from_growing;

		public AliveStates alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grow;
			base.serializable = true;
			State state = dead;
			string name = CREATURES.STATUSITEMS.DEAD.NAME;
			string tooltip = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main).ToggleTag(GameTags.PreventEmittingDisease).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, (object)null).EventTransition(GameHashes.EntombedChanged, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, dead, (StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			grow.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.replanted && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive, null);
			State state2 = alive.InitializeStates(masterTarget, dead).DefaultState(alive.idle);
			tooltip = CREATURES.STATUSITEMS.IDLE.NAME;
			name = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state2.ToggleStatusItem(tooltip, name, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
			alive.idle.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<DecorProvider>().SetValues(smi.master.positive_decor_effect);
				smi.master.GetComponent<DecorProvider>().Refresh();
				smi.master.AddTag(GameTags.Decoration);
			});
			alive.wilting.PlayAnim("wilt1", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.idle, null).ToggleTag(GameTags.PreventEmittingDisease)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.GetComponent<DecorProvider>().SetValues(smi.master.negative_decor_effect);
					smi.master.GetComponent<DecorProvider>().Refresh();
					smi.master.RemoveTag(GameTags.Decoration);
				});
		}
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private EntombVulnerable entombVulnerable;

	public bool replanted = false;

	public EffectorValues positive_decor_effect = new EffectorValues
	{
		amount = 1,
		radius = 5
	};

	public EffectorValues negative_decor_effect = new EffectorValues
	{
		amount = -1,
		radius = 5
	};

	private static readonly EventSystem.IntraObjectHandler<PrickleGrass> SetReplantedTrueDelegate = new EventSystem.IntraObjectHandler<PrickleGrass>(delegate(PrickleGrass component, object data)
	{
		component.replanted = true;
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1309017699, SetReplantedTrueDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}
}
