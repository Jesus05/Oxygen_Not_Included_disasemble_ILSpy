#define UNITY_ASSERTIONS
using Klei;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class ElementChunk : KMonoBehaviour, IHasSortOrder
{
	private static readonly EventSystem.IntraObjectHandler<ElementChunk> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<ElementChunk>(delegate(ElementChunk component, object data)
	{
		component.OnAbsorb(data);
	});

	public int sortOrder
	{
		get
		{
			return GetComponent<PrimaryElement>().Element.buildMenuSort;
		}
		set
		{
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.OreSizeVisualizers.Add(base.gameObject);
		GameComps.ElementSplitters.Add(base.gameObject);
		Subscribe(-2064133523, OnAbsorbDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		base.transform.SetPosition(position);
		PrimaryElement component = GetComponent<PrimaryElement>();
		Element element = component.Element;
		KSelectable component2 = GetComponent<KSelectable>();
		Func<Element> data = () => element;
		component2.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, data);
		component2.AddStatusItem(Db.Get().MiscStatusItems.OreMass, base.gameObject);
		component2.AddStatusItem(Db.Get().MiscStatusItems.OreTemp, base.gameObject);
	}

	protected override void OnCleanUp()
	{
		GameComps.ElementSplitters.Remove(base.gameObject);
		GameComps.OreSizeVisualizers.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
		{
			PrimaryElement primaryElement = pickupable.PrimaryElement;
			if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
			{
				float num = 0f;
				float mass = primaryElement.Mass;
				if (mass > 0f)
				{
					PrimaryElement component = GetComponent<PrimaryElement>();
					float mass2 = component.Mass;
					num = ((!(mass2 > 0f)) ? primaryElement.Temperature : SimUtil.CalculateFinalTemperature(mass2, component.Temperature, mass, primaryElement.Temperature));
					component.SetMassTemperature(mass2 + mass, num);
					UnityEngine.Debug.Assert(component.Temperature > 0f || component.Mass == 0f, "OnAbsorb resulted in a temperature of 0", base.gameObject);
				}
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
