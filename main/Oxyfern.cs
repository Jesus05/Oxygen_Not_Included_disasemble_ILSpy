using STRINGS;
using UnityEngine;

public class Oxyfern : StateMachineComponent<Oxyfern.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Oxyfern, object>.GameInstance
	{
		public StatesInstance(Oxyfern master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Oxyfern>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State mature;

			public State wilting;
		}

		public State grow;

		public State blocked_from_growing;

		public AliveStates alive;

		public State dead;

		private StatusItem statusItemCooling;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = grow;
			State state = dead;
			string name = CREATURES.STATUSITEMS.DEAD.NAME;
			string tooltip = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, (object)null).EventTransition(GameHashes.EntombedChanged, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.TagTransition(GameTags.Uprooted, dead, false);
			grow.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
			}).PlayAnim("grow_pst", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive, null);
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature);
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle_full", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			alive.wilting.PlayAnim("wilt3").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private ElementConverter elementConverter;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private static readonly EventSystem.IntraObjectHandler<Oxyfern> OnUprootedDelegate = new EventSystem.IntraObjectHandler<Oxyfern>(delegate(Oxyfern component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Oxyfern> OnReplantedDelegate = new EventSystem.IntraObjectHandler<Oxyfern>(delegate(Oxyfern component, object data)
	{
		component.OnReplanted(data);
	});

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
		base.smi.StartSM();
	}

	protected override void OnPrefabInit()
	{
		Subscribe(1309017699, OnReplantedDelegate);
		base.OnPrefabInit();
	}

	private void OnUprooted(object data = null)
	{
		GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), base.gameObject.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
		base.gameObject.Trigger(1623392196, null);
		base.gameObject.GetComponent<KBatchedAnimController>().StopAndClear();
		Object.Destroy(base.gameObject.GetComponent<KBatchedAnimController>());
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnReplanted(object data = null)
	{
		SetConsumptionRate();
		if (receptacleMonitor.Replanted)
		{
			Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
		}
	}

	public void SetConsumptionRate()
	{
		if (receptacleMonitor.Replanted)
		{
			elementConsumer.consumptionRate = 0.000625000044f;
		}
		else
		{
			elementConsumer.consumptionRate = 0.000156250011f;
		}
	}
}
