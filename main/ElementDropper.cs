using UnityEngine;

public class ElementDropper : KMonoBehaviour
{
	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public float emitMass;

	[SerializeField]
	public Vector3 emitOffset = Vector3.zero;

	[MyCmpGet]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<ElementDropper> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ElementDropper>(delegate(ElementDropper component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = storage.FindFirst(emitTag);
		if (!((Object)gameObject == (Object)null))
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.Mass >= emitMass)
			{
				Pickupable pickupable = gameObject.GetComponent<Pickupable>();
				if ((Object)pickupable != (Object)null)
				{
					pickupable = pickupable.Take(emitMass);
					pickupable.transform.SetPosition(pickupable.transform.GetPosition() + emitOffset);
				}
				else
				{
					storage.Drop(gameObject, true);
					gameObject.transform.SetPosition(gameObject.transform.GetPosition() + emitOffset);
				}
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), pickupable.transform, 1.5f, false);
			}
		}
	}
}
