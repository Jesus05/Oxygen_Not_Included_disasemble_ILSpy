using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalControlledSwitch : CircuitSwitch
{
	private static readonly EventSystem.IntraObjectHandler<OperationalControlledSwitch> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalControlledSwitch>(delegate(OperationalControlledSwitch component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		manuallyControlled = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	private void OnOperationalChanged(object data)
	{
		bool state = (bool)data;
		SetState(state);
	}
}
