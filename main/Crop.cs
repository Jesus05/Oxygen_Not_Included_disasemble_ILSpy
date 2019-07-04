using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Crop : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serializable]
	public struct CropVal
	{
		public string cropId;

		public float cropDuration;

		public int numProduced;

		public bool renewable;

		public CropVal(string crop_id, float crop_duration, int num_produced = 1, bool renewable = true)
		{
			cropId = crop_id;
			cropDuration = crop_duration;
			numProduced = num_produced;
			this.renewable = renewable;
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public CropVal cropVal;

	public string domesticatedDesc = "";

	private Storage planterStorage;

	private static readonly EventSystem.IntraObjectHandler<Crop> OnHarvestDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Crop> OnSeedDroppedDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnSeedDropped(data);
	});

	public string cropId => cropVal.cropId;

	public Storage PlanterStorage
	{
		get
		{
			return planterStorage;
		}
		set
		{
			planterStorage = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Crops.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1272413801, OnHarvestDelegate);
		Subscribe(-1736624145, OnSeedDroppedDelegate);
	}

	public void Configure(CropVal cropval)
	{
		cropVal = cropval;
	}

	public bool CanGrow()
	{
		return cropVal.renewable;
	}

	public void SpawnFruit(object callbackParam)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null))
		{
			CropVal cropVal = this.cropVal;
			if (!string.IsNullOrEmpty(cropVal.cropId))
			{
				GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, cropVal.cropId, Grid.SceneLayer.Ore);
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					float y = 0.75f;
					gameObject.transform.SetPosition(gameObject.transform.GetPosition() + new Vector3(0f, y, 0f));
					gameObject.SetActive(true);
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					component.Units = (float)cropVal.numProduced;
					Edible component2 = gameObject.GetComponent<Edible>();
					if ((bool)component2)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
					}
				}
				else
				{
					DebugUtil.LogErrorArgs(base.gameObject, "tried to spawn an invalid crop prefab:", cropVal.cropId);
				}
				Trigger(-1072826864, null);
			}
		}
	}

	protected override void OnCleanUp()
	{
		Components.Crops.Remove(this);
		base.OnCleanUp();
	}

	private void OnHarvest(object obj)
	{
	}

	public void OnSeedDropped(object data)
	{
	}

	public List<Descriptor> RequirementDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}

	public List<Descriptor> InformationDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Tag tag = new Tag(cropVal.cropId);
		GameObject prefab = Assets.GetPrefab(tag);
		Edible component = prefab.GetComponent<Edible>();
		float num = 0f;
		string arg = "";
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			num = component.FoodInfo.CaloriesPerUnit;
		}
		float calories = num * (float)cropVal.numProduced;
		InfoDescription component2 = prefab.GetComponent<InfoDescription>();
		if ((bool)component2)
		{
			arg = component2.description;
		}
		string arg2 = GameTags.DisplayAsCalories.Contains(tag) ? GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true) : ((!GameTags.DisplayAsUnits.Contains(tag)) ? GameUtil.GetFormattedMass((float)cropVal.numProduced, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") : GameUtil.GetFormattedUnits((float)cropVal.numProduced, GameUtil.TimeSlice.None, false));
		LocString yIELD = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor item = new Descriptor(string.Format(yIELD, prefab.GetProperName(), arg2), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, arg, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false);
		list.Add(item);
		Descriptor item2 = new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f, GameUtil.TimeSlice.None)), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false);
		list.Add(item2);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors(go))
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in InformationDescriptors(go))
		{
			list.Add(item2);
		}
		return list;
	}
}
