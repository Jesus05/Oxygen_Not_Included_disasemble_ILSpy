public class SaltPlant : StateMachineComponent<SaltPlant.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SaltPlant, object>.GameInstance
	{
		public StatesInstance(SaltPlant master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SaltPlant>
	{
		public State alive;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = alive;
			alive.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-724860998, OnWilt);
		Subscribe(712767498, OnWiltRecover);
	}

	private void OnWilt(object data = null)
	{
		base.gameObject.GetComponent<ElementConsumer>().EnableConsumption(false);
	}

	private void OnWiltRecover(object data = null)
	{
		base.gameObject.GetComponent<ElementConsumer>().EnableConsumption(true);
	}
}
