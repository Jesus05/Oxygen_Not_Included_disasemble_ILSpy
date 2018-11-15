using UnityEngine;

public class BubbleSpawner : KMonoBehaviour
{
	public SimHashes element;

	public float emitMass;

	public float emitVariance;

	public Vector3 emitOffset = Vector3.zero;

	public Vector2 initialVelocity;

	[MyCmpGet]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<BubbleSpawner> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<BubbleSpawner>(delegate(BubbleSpawner component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnSpawn()
	{
		emitMass += (Random.value - 0.5f) * emitVariance * emitMass;
		base.OnSpawn();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = storage.FindFirst(ElementLoader.FindElementByHash(element).tag);
		if (!((Object)gameObject == (Object)null))
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.Mass >= emitMass)
			{
				gameObject.GetComponent<PrimaryElement>().Mass -= emitMass;
				BubbleManager.instance.SpawnBubble(base.transform.GetPosition(), initialVelocity, component.ElementID, emitMass, component.Temperature);
			}
		}
	}
}
