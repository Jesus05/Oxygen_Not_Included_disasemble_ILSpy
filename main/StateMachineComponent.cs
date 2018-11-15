using KSerialization;
using System;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class StateMachineComponent : KMonoBehaviour, ISaveLoadable, IStateMachineTarget
{
	[MyCmpAdd]
	protected StateMachineController stateMachineController;

	public abstract StateMachine.Instance GetSMI();
}
[SerializationConfig(MemberSerialization.OptIn)]
public class StateMachineComponent<StateMachineInstanceType> : StateMachineComponent, ISaveLoadable where StateMachineInstanceType : StateMachine.Instance
{
	private StateMachineInstanceType _smi;

	public StateMachineInstanceType smi
	{
		get
		{
			if (_smi == null)
			{
				_smi = (StateMachineInstanceType)Activator.CreateInstance(typeof(StateMachineInstanceType), new object[1]
				{
					this
				});
			}
			return _smi;
		}
	}

	public override StateMachine.Instance GetSMI()
	{
		return _smi;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (_smi != null)
		{
			_smi.StopSM("StateMachineComponent.OnCleanUp");
			_smi = (StateMachineInstanceType)null;
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (base.isSpawned)
		{
			StateMachineInstanceType smi = this.smi;
			smi.StartSM();
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (_smi != null)
		{
			_smi.StopSM("StateMachineComponent.OnDisable");
		}
	}
}
