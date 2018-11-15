using System.Collections.Generic;

internal class StateMachineManagerAsyncLoader : GlobalAsyncLoader<StateMachineManagerAsyncLoader>
{
	public List<StateMachine> stateMachines = new List<StateMachine>();

	public override void Run()
	{
	}
}
