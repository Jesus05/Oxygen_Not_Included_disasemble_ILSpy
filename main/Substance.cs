using FMODUnity;
using Klei;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Substance
{
	public string name;

	public SimHashes elementID;

	internal Tag nameTag;

	public Color32 colour;

	[FormerlySerializedAs("debugColour")]
	public Color32 uiColour;

	[FormerlySerializedAs("overlayColour")]
	public Color32 conduitColour = Color.white;

	[NonSerialized]
	internal bool renderedByWorld;

	[NonSerialized]
	internal int idx;

	public Material material;

	public KAnimFile anim;

	[SerializeField]
	internal bool showInEditor = true;

	[NonSerialized]
	internal KAnimFile[] anims;

	[NonSerialized]
	internal ElementsAudio.ElementAudioConfig audioConfig;

	[NonSerialized]
	internal MaterialPropertyBlock propertyBlock;

	[EventRef]
	public string fallingStartSound;

	[EventRef]
	public string fallingStopSound;

	public GameObject SpawnResource(Vector3 position, float mass, float temperature, byte disease_idx, int disease_count, bool prevent_merge = false, bool forceTemperature = false, bool manual_activation = false)
	{
		GameObject gameObject = null;
		PrimaryElement primaryElement = null;
		if (!prevent_merge)
		{
			int cell = Grid.PosToCell(position);
			GameObject gameObject2 = Grid.Objects[cell, 3];
			if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
			{
				Pickupable component = gameObject2.GetComponent<Pickupable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					Tag b = GameTagExtensions.Create(elementID);
					for (ObjectLayerListItem objectLayerListItem = component.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
					{
						KPrefabID component2 = objectLayerListItem.gameObject.GetComponent<KPrefabID>();
						if (component2.PrefabTag == b)
						{
							gameObject = component2.gameObject;
							primaryElement = component2.GetComponent<PrimaryElement>();
							temperature = SimUtil.CalculateFinalTemperature(primaryElement.Mass, primaryElement.Temperature, mass, temperature);
							position = gameObject.transform.GetPosition();
							break;
						}
					}
				}
			}
		}
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			GameObject prefab = Assets.GetPrefab(nameTag);
			gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
			primaryElement = gameObject.GetComponent<PrimaryElement>();
			primaryElement.Mass = mass;
		}
		else
		{
			primaryElement.Mass += mass;
		}
		primaryElement.InternalTemperature = temperature;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		gameObject.transform.SetPosition(position);
		if (!manual_activation)
		{
			ActivateSubstanceGameObject(gameObject, disease_idx, disease_count);
		}
		return gameObject;
	}

	public void ActivateSubstanceGameObject(GameObject obj, byte disease_idx, int disease_count)
	{
		obj.SetActive(true);
		obj.GetComponent<PrimaryElement>().AddDisease(disease_idx, disease_count, "Substances.SpawnResource");
	}

	private void SetTexture(MaterialPropertyBlock block, string texture_name)
	{
		Texture texture = material.GetTexture(texture_name);
		if ((UnityEngine.Object)texture != (UnityEngine.Object)null)
		{
			propertyBlock.SetTexture(texture_name, texture);
		}
	}

	public void RefreshPropertyBlock()
	{
		if (propertyBlock == null)
		{
			propertyBlock = new MaterialPropertyBlock();
		}
		if ((UnityEngine.Object)material != (UnityEngine.Object)null)
		{
			SetTexture(propertyBlock, "_MainTex");
			float @float = material.GetFloat("_WorldUVScale");
			propertyBlock.SetFloat("_WorldUVScale", @float);
			Element element = ElementLoader.FindElementByHash(elementID);
			if (element.IsSolid)
			{
				SetTexture(propertyBlock, "_MainTex2");
				SetTexture(propertyBlock, "_HeightTex2");
				propertyBlock.SetFloat("_Frequency", material.GetFloat("_Frequency"));
				propertyBlock.SetColor("_ShineColour", material.GetColor("_ShineColour"));
				propertyBlock.SetColor("_ColourTint", material.GetColor("_ColourTint"));
			}
		}
	}

	internal AmbienceType GetAmbience()
	{
		return (audioConfig == null) ? AmbienceType.None : audioConfig.ambienceType;
	}

	internal SolidAmbienceType GetSolidAmbience()
	{
		return (audioConfig == null) ? SolidAmbienceType.None : audioConfig.solidAmbienceType;
	}

	internal string GetMiningSound()
	{
		return (audioConfig == null) ? "" : audioConfig.miningSound;
	}

	internal string GetMiningBreakSound()
	{
		return (audioConfig == null) ? "" : audioConfig.miningBreakSound;
	}

	internal string GetOreBumpSound()
	{
		return (audioConfig == null) ? "" : audioConfig.oreBumpSound;
	}

	internal string GetFloorEventAudioCategory()
	{
		return (audioConfig == null) ? "" : audioConfig.floorEventAudioCategory;
	}

	internal string GetCreatureChewSound()
	{
		return (audioConfig == null) ? "" : audioConfig.creatureChewSound;
	}
}
