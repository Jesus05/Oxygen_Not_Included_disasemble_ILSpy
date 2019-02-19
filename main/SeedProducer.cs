using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SeedProducer : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serializable]
	public struct SeedInfo
	{
		public string seedId;

		public ProductionType productionType;

		public int newSeedsProduced;
	}

	public enum ProductionType
	{
		Hidden,
		DigOnly,
		Harvest,
		Fruit
	}

	public SeedInfo seedInfo;

	private bool droppedSeedAlready;

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> DropSeedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		component.DropSeed(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> CropPickedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		component.CropPicked(data);
	});

	public void Configure(string SeedID, ProductionType productionType, int newSeedsProduced = 1)
	{
		seedInfo.seedId = SeedID;
		seedInfo.productionType = productionType;
		seedInfo.newSeedsProduced = newSeedsProduced;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, DropSeedDelegate);
		Subscribe(1623392196, DropSeedDelegate);
		Subscribe(-1072826864, CropPickedDelegate);
	}

	public GameObject ProduceSeed(string seedId, int units = 1)
	{
		if (seedId != null && units > 0)
		{
			Vector3 position = base.gameObject.transform.GetPosition() + new Vector3(0f, 0.5f, 0f);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(seedId)), position, Grid.SceneLayer.Ore, null, 0);
			PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			component2.Units = (float)units;
			Trigger(472291861, gameObject.GetComponent<PlantableSeed>());
			gameObject.SetActive(true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform, 1.5f, false);
			return gameObject;
		}
		return null;
	}

	public void DropSeed(object data = null)
	{
		if (!droppedSeedAlready)
		{
			GameObject gameObject = ProduceSeed(seedInfo.seedId, 1);
			Trigger(-1736624145, gameObject.GetComponent<PlantableSeed>());
			droppedSeedAlready = true;
		}
	}

	public void CropDepleted(object data)
	{
		DropSeed(null);
	}

	public void CropPicked(object data)
	{
		if (seedInfo.productionType == ProductionType.Harvest)
		{
			Worker completed_by = GetComponent<Harvestable>().completed_by;
			float num = 10f;
			if ((UnityEngine.Object)completed_by != (UnityEngine.Object)null)
			{
				num += completed_by.GetAttributes().Get(Db.Get().Attributes.Botanist).GetTotalValue() * Db.Get().AttributeConverters.SeedHarvestChance.multiplier;
			}
			int units = ((float)UnityEngine.Random.Range(0, 100) <= num) ? 1 : 0;
			ProduceSeed(seedInfo.seedId, units);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		GameObject prefab = Assets.GetPrefab(new Tag(seedInfo.seedId));
		if (!((UnityEngine.Object)prefab != (UnityEngine.Object)null))
		{
			goto IL_0028;
		}
		goto IL_0028;
		IL_0028:
		switch (seedInfo.productionType)
		{
		default:
			return null;
		case ProductionType.DigOnly:
			return null;
		case ProductionType.Harvest:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, Descriptor.DescriptorType.Lifecycle, true));
			break;
		case ProductionType.Fruit:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, Descriptor.DescriptorType.Lifecycle, true));
			break;
		}
		return list;
	}
}
