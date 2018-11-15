using FMODUnity;
using Klei;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Substance
{
	[Serializable]
	public struct Loot
	{
		public GameObject item;

		public bool spawnOnFloor;

		public bool isEntombedItem;

		public void FreeResources()
		{
			item = null;
			spawnOnFloor = false;
		}
	}

	public string name;

	public SimHashes elementID;

	public Color32 colour;

	public Color32 debugColour;

	public Color32 overlayColour = Color.white;

	public Texture2D colourMap;

	public Texture2D shineMask;

	public Texture2D normalMap;

	public GameObject hitEffect;

	[EventRef]
	[FormerlySerializedAs("fallingStartSoundMigrated")]
	public string fallingStartSound;

	[EventRef]
	[FormerlySerializedAs("fallingStopSoundMigrated")]
	public string fallingStopSound;

	[NonSerialized]
	public bool renderedByWorld;

	[NonSerialized]
	public int idx;

	public Material material;

	public KAnimFile anim;

	[NonSerialized]
	public KAnimFile[] anims;

	public float hue;

	public float saturation = 1f;

	public MaterialPropertyBlock propertyBlock;

	public ElementsAudio.ElementAudioConfig audioConfig;

	public bool showInEditor = true;

	public GameObject SpawnResource(Vector3 position, float mass, float temperature, byte disease_idx, int disease_count, bool prevent_merge = false, bool forceTemperature = false)
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
			gameObject = GameUtil.KInstantiate(Assets.GetPrefab(GameTagExtensions.Create(elementID)), Grid.SceneLayer.Ore, null, 0);
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
		gameObject.SetActive(true);
		primaryElement.AddDisease(disease_idx, disease_count, "Substances.SpawnResource");
		return gameObject;
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
		propertyBlock.SetVector("_HueSaturation", new Vector4(hue, saturation, 0f, 0f));
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

	public AmbienceType GetAmbience()
	{
		if (audioConfig == null)
		{
			return AmbienceType.None;
		}
		return audioConfig.ambienceType;
	}

	public SolidAmbienceType GetSolidAmbience()
	{
		if (audioConfig == null)
		{
			return SolidAmbienceType.None;
		}
		return audioConfig.solidAmbienceType;
	}

	public string GetMiningSound()
	{
		if (audioConfig == null)
		{
			return string.Empty;
		}
		return audioConfig.miningSound;
	}

	public string GetMiningBreakSound()
	{
		if (audioConfig == null)
		{
			return string.Empty;
		}
		return audioConfig.miningBreakSound;
	}

	public string GetOreBumpSound()
	{
		if (audioConfig == null)
		{
			return string.Empty;
		}
		return audioConfig.oreBumpSound;
	}

	public string GetFloorEventAudioCategory()
	{
		if (audioConfig == null)
		{
			return string.Empty;
		}
		return audioConfig.floorEventAudioCategory;
	}

	public string GetCreatureChewSound()
	{
		if (audioConfig == null)
		{
			return string.Empty;
		}
		return audioConfig.creatureChewSound;
	}
}
