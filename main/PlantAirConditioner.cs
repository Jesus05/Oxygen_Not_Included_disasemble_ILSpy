using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantAirConditioner : AirConditioner
{
	private Operational.Flag fertilizedFlag = new Operational.Flag("fertilized", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<PlantAirConditioner> OnFertilizedDelegate = new EventSystem.IntraObjectHandler<PlantAirConditioner>(delegate(PlantAirConditioner component, object data)
	{
		component.OnFertilized(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantAirConditioner> OnUnfertilizedDelegate = new EventSystem.IntraObjectHandler<PlantAirConditioner>(delegate(PlantAirConditioner component, object data)
	{
		component.OnUnfertilized(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1396791468, OnFertilizedDelegate);
		Subscribe(-1073674739, OnUnfertilizedDelegate);
	}

	private void OnFertilized(object data)
	{
		operational.SetFlag(fertilizedFlag, true);
	}

	private void OnUnfertilized(object data)
	{
		operational.SetFlag(fertilizedFlag, false);
	}
}
