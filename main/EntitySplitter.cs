using Klei;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class EntitySplitter : KMonoBehaviour
{
	public float maxStackSize = 3.40282347E+38f;

	private static readonly EventSystem.IntraObjectHandler<EntitySplitter> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<EntitySplitter>(delegate(EntitySplitter component, object data)
	{
		component.OnAbsorb(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = GetComponent<Pickupable>();
		if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null)
		{
			Debug.LogError(base.name + " does not have a pickupable component!");
		}
		Pickupable pickupable2 = pickupable;
		pickupable2.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable2.OnTake, (Func<float, Pickupable>)((float amount) => Split(pickupable, amount, null)));
		Rottable.Instance rottable = base.gameObject.GetSMI<Rottable.Instance>();
		pickupable.absorbable = true;
		pickupable.CanAbsorb = ((Pickupable other) => CanFirstAbsorbSecond(pickupable, rottable, other, maxStackSize));
		Subscribe(-2064133523, OnAbsorbDelegate);
	}

	private static bool CanFirstAbsorbSecond(Pickupable pickupable, Rottable.Instance rottable, Pickupable other, float maxStackSize)
	{
		if ((UnityEngine.Object)other == (UnityEngine.Object)null)
		{
			return false;
		}
		KPrefabID component = pickupable.GetComponent<KPrefabID>();
		KPrefabID component2 = other.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return false;
		}
		if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
		{
			return false;
		}
		if (component.PrefabTag != component2.PrefabTag)
		{
			return false;
		}
		if (pickupable.TotalAmount + other.TotalAmount > maxStackSize)
		{
			return false;
		}
		if (rottable != null)
		{
			Rottable.Instance sMI = other.GetSMI<Rottable.Instance>();
			if (sMI == null)
			{
				return false;
			}
			if (!rottable.IsRotLevelStackable(sMI))
			{
				return false;
			}
		}
		return true;
	}

	public static Pickupable Split(Pickupable pickupable, float amount, GameObject prefab = null)
	{
		if (amount >= pickupable.TotalAmount && (UnityEngine.Object)prefab == (UnityEngine.Object)null)
		{
			return pickupable;
		}
		Storage storage = pickupable.storage;
		if ((UnityEngine.Object)prefab == (UnityEngine.Object)null)
		{
			prefab = Assets.GetPrefab(pickupable.GetComponent<KPrefabID>().PrefabTag);
		}
		GameObject parent = null;
		if ((UnityEngine.Object)pickupable.transform.parent != (UnityEngine.Object)null)
		{
			parent = pickupable.transform.parent.gameObject;
		}
		GameObject gameObject = GameUtil.KInstantiate(prefab, pickupable.transform.GetPosition(), Grid.SceneLayer.Ore, parent, null, 0);
		Debug.Assert((UnityEngine.Object)gameObject != (UnityEngine.Object)null, "WTH, the GO is null, shouldn't happen on instantiate");
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject.name, gameObject);
		}
		gameObject.SetActive(true);
		component.TotalAmount = Mathf.Min(amount, pickupable.TotalAmount);
		bool keepZeroMassObject = pickupable.PrimaryElement.KeepZeroMassObject;
		pickupable.PrimaryElement.KeepZeroMassObject = true;
		pickupable.TotalAmount -= amount;
		component.Trigger(1335436905, pickupable);
		pickupable.PrimaryElement.KeepZeroMassObject = keepZeroMassObject;
		pickupable.TotalAmount = pickupable.TotalAmount;
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			storage.Trigger(-1697596308, pickupable.gameObject);
			storage.Trigger(-778359855, null);
		}
		return component;
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			PrimaryElement primaryElement = pickupable.PrimaryElement;
			if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
			{
				float temperature = 0f;
				float mass = component.Mass;
				float mass2 = primaryElement.Mass;
				if (mass > 0f && mass2 > 0f)
				{
					temperature = SimUtil.CalculateFinalTemperature(mass, component.Temperature, mass2, primaryElement.Temperature);
				}
				else if (primaryElement.Mass > 0f)
				{
					temperature = primaryElement.Temperature;
				}
				component.SetMassTemperature(mass + mass2, temperature);
				if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
				{
					string sound = GlobalAssets.GetSound("Ore_absorb", false);
					if (sound != null && CameraController.Instance.IsAudibleSound(pickupable.transform.GetPosition(), sound))
					{
						PlaySound3D(sound);
					}
				}
			}
		}
	}
}
