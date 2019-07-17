using UnityEngine;

public class BasicForagePlantPlanted : StateMachineComponent<BasicForagePlantPlanted.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BasicForagePlantPlanted, object>.GameInstance
	{
		public StatesInstance(BasicForagePlantPlanted smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BasicForagePlantPlanted>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State idle;

			public State harvest;
		}

		public State seed_grow;

		public AliveStates alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = seed_grow;
			base.serializable = true;
			seed_grow.PlayAnim("idle", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive.idle, null);
			alive.InitializeStates(masterTarget, dead);
			alive.idle.PlayAnim("idle").EventTransition(GameHashes.Harvest, alive.harvest, null).Enter(delegate(StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			});
			alive.harvest.Enter(delegate(StatesInstance smi)
			{
				smi.master.seedProducer.DropSeed(null);
			}).GoTo(dead);
			dead.Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.animController.StopAndClear();
				Object.Destroy(smi.master.animController);
				smi.master.DestroySelf(null);
			});
		}
	}

	[MyCmpReq]
	private Harvestable harvestable;

	[MyCmpReq]
	private SeedProducer seedProducer;

	[MyCmpReq]
	private KBatchedAnimController animController;

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
