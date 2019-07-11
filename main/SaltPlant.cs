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

	private static readonly EventSystem.IntraObjectHandler<SaltPlant> OnWiltDelegate = new EventSystem.IntraObjectHandler<SaltPlant>(delegate(SaltPlant component, object data)
	{
		component.OnWilt(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SaltPlant> OnWiltRecoverDelegate = new EventSystem.IntraObjectHandler<SaltPlant>(delegate(SaltPlant component, object data)
	{
		component.OnWiltRecover(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-724860998, OnWiltDelegate);
		Subscribe(712767498, OnWiltRecoverDelegate);
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
