using UnityEngine;

public class Reservoir : KMonoBehaviour
{
	private MeterController meter;

	[MyCmpGet]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<Reservoir> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Reservoir>(delegate(Reservoir component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_fill", "meter_OL");
		Subscribe(-1697596308, OnStorageChangeDelegate);
		OnStorageChange(null);
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(Mathf.Clamp01(storage.MassStored() / storage.capacityKg));
	}
}
