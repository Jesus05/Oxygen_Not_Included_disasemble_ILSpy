using System;

public abstract class Usable : KMonoBehaviour, IStateMachineTarget
{
	private StateMachine.Instance smi;

	public abstract void StartUsing(User user);

	protected void StartUsing(StateMachine.Instance smi, User user)
	{
		DebugUtil.Assert(this.smi == null);
		DebugUtil.Assert(smi != null);
		this.smi = smi;
		smi.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(smi.OnStop, new Action<string, StateMachine.Status>(user.OnStateMachineStop));
		smi.StartSM();
	}

	public void StopUsing(User user)
	{
		if (smi != null)
		{
			StateMachine.Instance instance = smi;
			instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Remove(instance.OnStop, new Action<string, StateMachine.Status>(user.OnStateMachineStop));
			smi.StopSM("Usable.StopUsing");
			smi = null;
		}
	}
}
