using Klei.AI;
using TUNING;
using UnityEngine;

public class Tinkerable : Workable
{
	private Chore chore;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Effects effects;

	[MyCmpGet]
	private RoomTracker roomTracker;

	public Tag tinkerMaterialTag;

	public float tinkerMaterialAmount;

	public string addedEffect;

	public HashedString choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;

	public HashedString choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnEffectRemovedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnEffectRemoved(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnStorageChange(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnUpdateRoom(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private bool hasReservedMaterial = false;

	public static Tinkerable MakePowerTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = PowerControlStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.addedEffect = "PowerTinker";
		tinkerable.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
		tinkerable.SetWorkTime(180f);
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;
		tinkerable.choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;
		tinkerable.multitoolContext = "powertinker";
		tinkerable.multitoolHitEffectTag = "fx_powertinker_splash";
		tinkerable.shouldShowSkillPerkStatusItem = false;
		prefab.AddOrGet<Storage>();
		prefab.AddOrGet<Effects>();
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	public static Tinkerable MakeFarmTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = FarmStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.addedEffect = "FarmTinker";
		tinkerable.requiredSkillPerk = Db.Get().SkillPerks.CanFarmTinker.Id;
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.SetWorkTime(15f);
		tinkerable.attributeConverter = Db.Get().AttributeConverters.PlantTendSpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.CropTend.IdHash;
		tinkerable.choreTypeFetch = Db.Get().ChoreTypes.FarmFetch.IdHash;
		tinkerable.multitoolContext = "tend";
		tinkerable.multitoolHitEffectTag = "fx_tend_splash";
		tinkerable.shouldShowSkillPerkStatusItem = false;
		prefab.AddOrGet<Storage>();
		prefab.AddOrGet<Effects>();
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
		workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		Subscribe(-1157678353, OnEffectRemovedDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		Subscribe(144050788, OnUpdateRoomDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		UpdateMaterialReservation(false);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		UpdateChore();
	}

	private void OnEffectRemoved(object data)
	{
		UpdateChore();
	}

	private void OnUpdateRoom(object data)
	{
		UpdateChore();
	}

	private void OnStorageChange(object data)
	{
		UpdateChore();
	}

	private void UpdateChore()
	{
		Operational component = GetComponent<Operational>();
		bool flag = (Object)component == (Object)null || component.IsFunctional;
		bool flag2 = !HasEffect() && RoomHasActiveTinkerstation() && flag;
		if (chore == null && flag2)
		{
			UpdateMaterialReservation(true);
			SetWorkTime(workTime);
			if (HasMaterial())
			{
				chore = new WorkChore<Tinkerable>(Db.Get().ChoreTypes.GetByHash(choreTypeTinker), this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				if ((Object)component != (Object)null)
				{
					chore.AddPrecondition(ChorePreconditions.instance.IsFunctional, component);
				}
			}
			else
			{
				chore = new FetchChore(Db.Get().ChoreTypes.GetByHash(choreTypeFetch), storage, tinkerMaterialAmount, new Tag[1]
				{
					tinkerMaterialTag
				}, null, null, null, true, OnFetchComplete, null, null, FetchOrder2.OperationalRequirement.Functional, 0);
			}
			chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, requiredSkillPerk);
			RoomTracker component2 = GetComponent<RoomTracker>();
			if (!string.IsNullOrEmpty(component2.requiredRoomType))
			{
				chore.AddPrecondition(ChorePreconditions.instance.IsInMyRoom, Grid.PosToCell(base.transform.GetPosition()));
			}
		}
		else if (chore != null && !flag2)
		{
			UpdateMaterialReservation(false);
			chore.Cancel("No longer needed");
			chore = null;
		}
	}

	private bool RoomHasActiveTinkerstation()
	{
		if (roomTracker.IsInCorrectRoom())
		{
			if (roomTracker.room != null)
			{
				foreach (KPrefabID building in roomTracker.room.buildings)
				{
					if (!((Object)building == (Object)null))
					{
						TinkerStation component = building.GetComponent<TinkerStation>();
						if ((Object)component != (Object)null && component.outputPrefab == tinkerMaterialTag)
						{
							Operational component2 = building.GetComponent<Operational>();
							if (component2.IsOperational)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}
		return false;
	}

	private void UpdateMaterialReservation(bool shouldReserve)
	{
		if (shouldReserve && !hasReservedMaterial)
		{
			MaterialNeeds.Instance.UpdateNeed(tinkerMaterialTag, tinkerMaterialAmount);
			hasReservedMaterial = shouldReserve;
		}
		else if (!shouldReserve && hasReservedMaterial)
		{
			MaterialNeeds.Instance.UpdateNeed(tinkerMaterialTag, 0f - tinkerMaterialAmount);
			hasReservedMaterial = shouldReserve;
		}
	}

	private void OnFetchComplete(Chore data)
	{
		UpdateMaterialReservation(false);
		chore = null;
		UpdateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		storage.ConsumeIgnoringDisease(tinkerMaterialTag, tinkerMaterialAmount);
		effects.Add(addedEffect, true);
		UpdateMaterialReservation(false);
		chore = null;
		UpdateChore();
	}

	private bool HasMaterial()
	{
		return storage.GetAmountAvailable(tinkerMaterialTag) >= tinkerMaterialAmount;
	}

	private bool HasEffect()
	{
		return effects.HasEffect(addedEffect);
	}
}
