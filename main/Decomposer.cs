using UnityEngine;

public class Decomposer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		StateMachineController component = GetComponent<StateMachineController>();
		if (!((Object)component == (Object)null))
		{
			DecompositionMonitor.Instance instance = new DecompositionMonitor.Instance(this, null, 1f, false);
			component.AddStateMachineInstance(instance);
			instance.StartSM();
			instance.dirtyWaterMaxRange = 3;
		}
	}
}
