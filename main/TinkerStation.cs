using Klei;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TinkerStation : Workable, IEffectDescriptor, ISim1000ms
{
	public HashedString choreType;

	public HashedString fetchChoreType;

	private Chore chore;

	[MyCmpAdd]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	public bool useFilteredStorage = false;

	protected FilteredStorage filteredStorage;

	public float massPerTinker;

	public Tag inputMaterial;

	public Tag outputPrefab;

	private static readonly EventSystem.IntraObjectHandler<TinkerStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TinkerStation>(delegate(TinkerStation component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public AttributeConverter AttributeConverter
	{
		set
		{
			attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		set
		{
			attributeExperienceMultiplier = value;
		}
	}

	public string SkillExperienceSkillGroup
	{
		set
		{
			skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			skillExperienceMultiplier = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		if (useFilteredStorage)
		{
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreType);
			filteredStorage = new FilteredStorage(this, null, null, null, false, byHash);
		}
		SetWorkTime(15f);
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (useFilteredStorage && filteredStorage != null)
		{
			filteredStorage.FilterChanged();
		}
	}

	protected override void OnCleanUp()
	{
		if (filteredStorage != null)
		{
			filteredStorage.CleanUp();
		}
		base.OnCleanUp();
	}

	private bool CorrectRolePrecondition(MinionIdentity worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		return (Object)component != (Object)null && component.HasPerk(requiredSkillPerk);
	}

	private void OnOperationalChanged(object data)
	{
		RoomTracker component = GetComponent<RoomTracker>();
		if ((Object)component != (Object)null && component.room != null)
		{
			component.room.RetriggerBuildings();
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			operational.SetActive(true, false);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		ShowProgressBar(false);
		operational.SetActive(false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		storage.ConsumeAndGetDisease(inputMaterial, massPerTinker, out SimUtil.DiseaseInfo _, out float _);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(outputPrefab), base.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
		gameObject.SetActive(true);
		chore = null;
	}

	public void Sim1000ms(float dt)
	{
		UpdateChore();
	}

	private void UpdateChore()
	{
		if (operational.IsOperational && ToolsRequested() && HasMaterial())
		{
			if (chore == null)
			{
				chore = new WorkChore<TinkerStation>(Db.Get().ChoreTypes.GetByHash(choreType), this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, requiredSkillPerk);
				SetWorkTime(workTime);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("Can't tinker");
			chore = null;
		}
	}

	private bool HasMaterial()
	{
		return storage.MassStored() > 0f;
	}

	private bool ToolsRequested()
	{
		return MaterialNeeds.Instance.GetAmount(outputPrefab) > 0f && WorldInventory.Instance.GetAmount(outputPrefab) <= 0f;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		string arg = inputMaterial.ProperName();
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		list.AddRange(GameUtil.GetAllDescriptors(Assets.GetPrefab(outputPrefab), false));
		List<Tinkerable> list2 = new List<Tinkerable>();
		foreach (GameObject item2 in Assets.GetPrefabsWithComponent<Tinkerable>())
		{
			Tinkerable component = item2.GetComponent<Tinkerable>();
			if (component.tinkerMaterialTag == outputPrefab)
			{
				list2.Add(component);
			}
		}
		if (list2.Count > 0)
		{
			Effect effect = Db.Get().effects.Get(list2[0].addedEffect);
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ADDED_EFFECT, effect.Name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ADDED_EFFECT, effect.Name, Effect.CreateTooltip(effect, true, "\n")), Descriptor.DescriptorType.Effect, false));
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS, UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS, Descriptor.DescriptorType.Effect, false));
			foreach (Tinkerable item3 in list2)
			{
				Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS_ITEM, item3.GetProperName()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS_ITEM, item3.GetProperName()), Descriptor.DescriptorType.Effect, false);
				item.IncreaseIndent();
				list.Add(item);
			}
		}
		return list;
	}

	public static TinkerStation AddTinkerStation(GameObject go, string required_room_type)
	{
		TinkerStation result = go.AddOrGet<TinkerStation>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = required_room_type;
		return result;
	}
}
