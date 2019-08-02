using KSerialization;
using STRINGS;
using UnityEngine;

public class OilEater : StateMachineComponent<OilEater.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OilEater, object>.GameInstance
	{
		public StatesInstance(OilEater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilEater>
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

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grow;
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, delegate(object data)
				{
					GameObject gameObject = (GameObject)data;
					CreatureHelpers.DeselectCreature(gameObject);
					Util.KDestroyGameObject(gameObject);
				}, smi.master.gameObject);
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
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive, null);
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature).Update("Alive", delegate(StatesInstance smi, float dt)
			{
				smi.master.Exhaust(dt);
			}, UpdateRate.SIM_200ms, false);
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop);
			alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	private const SimHashes srcElement = SimHashes.CrudeOil;

	private const SimHashes emitElement = SimHashes.CarbonDioxide;

	public float emitRate = 1f;

	public float minEmitMass;

	public Vector3 emitOffset = Vector3.zero;

	[Serialize]
	private float emittedMass;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void Exhaust(float dt)
	{
		if (!base.smi.master.wiltCondition.IsWilting())
		{
			emittedMass += dt * emitRate;
			if (emittedMass >= minEmitMass)
			{
				int gameCell = Grid.PosToCell(base.transform.GetPosition() + emitOffset);
				PrimaryElement component = GetComponent<PrimaryElement>();
				SimMessages.AddRemoveSubstance(gameCell, SimHashes.CarbonDioxide, CellEventLogger.Instance.ElementEmitted, emittedMass, component.Temperature, byte.MaxValue, 0, true, -1);
				emittedMass = 0f;
			}
		}
	}
}
