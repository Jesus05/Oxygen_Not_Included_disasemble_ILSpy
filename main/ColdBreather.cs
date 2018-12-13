using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SkipSaveFileSerialization]
public class ColdBreather : StateMachineComponent<ColdBreather.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ColdBreather, object>.GameInstance
	{
		public StatesInstance(ColdBreather master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ColdBreather>
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
			statusItemCooling = new StatusItem("cooling", CREATURES.STATUSITEMS.COOLING.NAME, CREATURES.STATUSITEMS.COOLING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf, null);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, (object)null).EventTransition(GameHashes.EntombedChanged, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, dead, (StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			grow.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive, null);
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.Exhale();
			}, UpdateRate.SIM_200ms, false);
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).ToggleMainStatusItem(statusItemCooling)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(true);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private const float EXHALE_PERIOD = 1f;

	public float deltaEmitTemperature = -5f;

	public Vector3 emitOffsetCell = new Vector3(0f, 0f);

	private List<GameObject> gases = new List<GameObject>();

	private Tag lastEmitTag;

	private int nextGasEmitIndex;

	private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;

	[CompilerGenerated]
	private static Action<Sim.MassEmittedCallback, object> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(OnSimEmittedCallback, this, "ColdBreather");
		elementConsumer.EnableConsumption(false);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.massEmitCallbackManager.Release(simEmitCBHandle, "coldbreather");
		simEmitCBHandle.Clear();
		if ((bool)storage)
		{
			storage.DropAll(true);
		}
		base.OnCleanUp();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.COLDBREATHER, UI.GAMEOBJECTEFFECTS.TOOLTIPS.COLDBREATHER, Descriptor.DescriptorType.Effect, false));
		return list;
	}

	private void Exhale()
	{
		if (!(lastEmitTag != Tag.Invalid))
		{
			gases.Clear();
			storage.Find(GameTags.Gas, gases);
			if (nextGasEmitIndex >= gases.Count)
			{
				nextGasEmitIndex = 0;
			}
			PrimaryElement component;
			do
			{
				if (nextGasEmitIndex >= gases.Count)
				{
					return;
				}
				int index = nextGasEmitIndex++;
				component = gases[index].GetComponent<PrimaryElement>();
			}
			while (!((UnityEngine.Object)component != (UnityEngine.Object)null) || !(component.Mass > 0f) || !simEmitCBHandle.IsValid());
			float temperature = Mathf.Max(component.Element.lowTemp + 5f, component.Temperature + deltaEmitTemperature);
			int gameCell = Grid.PosToCell(base.transform.GetPosition() + emitOffsetCell);
			byte idx = component.Element.idx;
			Game.Instance.massEmitCallbackManager.GetItem(simEmitCBHandle);
			SimMessages.EmitMass(gameCell, idx, component.Mass, temperature, component.DiseaseIdx, component.DiseaseCount, simEmitCBHandle.index);
			lastEmitTag = component.Element.tag;
		}
	}

	private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data)
	{
		((ColdBreather)data).OnSimEmitted(info);
	}

	private void OnSimEmitted(Sim.MassEmittedCallback info)
	{
		if (info.suceeded == 1 && (bool)storage && lastEmitTag.IsValid)
		{
			storage.ConsumeIgnoringDisease(lastEmitTag, info.mass);
		}
		lastEmitTag = Tag.Invalid;
	}
}
