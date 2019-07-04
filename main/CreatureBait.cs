using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureBait : StateMachineComponent<CreatureBait.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CreatureBait, object>.GameInstance
	{
		public StatesInstance(CreatureBait master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CreatureBait>
	{
		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.DoNothing();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Tag[] constructionElements = GetComponent<Deconstructable>().constructionElements;
		Tag tag = constructionElements[1];
		Lure.Instance sMI = base.gameObject.GetSMI<Lure.Instance>();
		sMI.SetActiveLures(new Tag[1]
		{
			tag
		});
	}
}
