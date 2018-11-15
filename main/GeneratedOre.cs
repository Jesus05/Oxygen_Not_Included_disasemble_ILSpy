using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedOre
{
	public static void LoadGeneratedOre(List<Type> types)
	{
		Type typeFromHandle = typeof(IOreConfig);
		HashSet<SimHashes> hashSet = new HashSet<SimHashes>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				object obj = Activator.CreateInstance(type);
				IOreConfig oreConfig = obj as IOreConfig;
				SimHashes elementID = oreConfig.ElementID;
				if (elementID != SimHashes.Void)
				{
					hashSet.Add(elementID);
				}
				GameObject gameObject = oreConfig.CreatePrefab();
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				Assets.AddPrefab(component);
			}
		}
		List<Element> elements = ElementLoader.elements;
		foreach (Element item in elements)
		{
			if (item != null && !hashSet.Contains(item.id))
			{
				if (item.substance != null && (UnityEngine.Object)item.substance.anim != (UnityEngine.Object)null)
				{
					GameObject gameObject2 = null;
					if (item.IsSolid)
					{
						gameObject2 = EntityTemplates.CreateSolidOreEntity(item.id, null);
					}
					else if (item.IsLiquid)
					{
						gameObject2 = EntityTemplates.CreateLiquidOreEntity(item.id, null);
					}
					else if (item.IsGas)
					{
						gameObject2 = EntityTemplates.CreateGasOreEntity(item.id, null);
					}
					if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
					{
						KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
						Assets.AddPrefab(component2);
					}
				}
				else
				{
					Output.LogError("Missing substance or anim for element [" + item.name + "]");
				}
			}
		}
	}

	public static SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		if (temperature <= 0f)
		{
			Output.LogWarning("GeneratedOre.CreateChunk tried to create a chunk with a temperature <= 0");
		}
		GameObject prefab = Assets.GetPrefab(element.tag);
		if ((UnityEngine.Object)prefab == (UnityEngine.Object)null)
		{
			Debug.LogError("Could not find prefab for element " + element.id.ToString(), null);
		}
		SubstanceChunk component = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0).GetComponent<SubstanceChunk>();
		component.transform.SetPosition(position);
		component.gameObject.SetActive(true);
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		component2.Mass = mass;
		component2.Temperature = temperature;
		component2.AddDisease(diseaseIdx, diseaseCount, "GeneratedOre.CreateChunk");
		KPrefabID component3 = component.GetComponent<KPrefabID>();
		component3.InitializeTags();
		return component;
	}
}
