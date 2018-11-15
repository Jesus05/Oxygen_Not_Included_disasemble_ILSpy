using Klei.AI;
using UnityEngine;

public class CreatureFeeder : KMonoBehaviour
{
	public string effectId;

	private static readonly EventSystem.IntraObjectHandler<CreatureFeeder> OnAteFromStorageDelegate = new EventSystem.IntraObjectHandler<CreatureFeeder>(delegate(CreatureFeeder component, object data)
	{
		component.OnAteFromStorage(data);
	});

	protected override void OnSpawn()
	{
		Components.CreatureFeeders.Add(this);
		Subscribe(-1452790913, OnAteFromStorageDelegate);
	}

	protected override void OnCleanUp()
	{
		Components.CreatureFeeders.Remove(this);
	}

	private void OnAteFromStorage(object data)
	{
		if (!string.IsNullOrEmpty(effectId))
		{
			GameObject gameObject = data as GameObject;
			gameObject.GetComponent<Effects>().Add(effectId, true);
		}
	}
}
