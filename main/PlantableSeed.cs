using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PlantableSeed : KMonoBehaviour, IHasSortOrder, IReceptacleDirection, IGameObjectEffectDescriptor, ISim200ms
{
	public Tag PlantID;

	public Tag PreviewID;

	[Serialize]
	public float timeUntilSelfPlant = 0f;

	public Tag replantGroundTag;

	public string domesticatedDescription;

	public SingleEntityReceptacle.ReceptacleDirection direction = SingleEntityReceptacle.ReceptacleDirection.Top;

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnSplitDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnSplit(data);
	});

	public int sortOrder
	{
		get;
		set;
	}

	public SingleEntityReceptacle.ReceptacleDirection Direction => direction;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-2064133523, OnAbsorbDelegate);
		Subscribe(1335436905, OnSplitDelegate);
		timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
	}

	private void OnAbsorb(object data)
	{
	}

	private void OnSplit(object data)
	{
	}

	public void Sim200ms(float dt)
	{
		timeUntilSelfPlant -= dt;
		if (timeUntilSelfPlant <= 0f)
		{
			TryPlant();
		}
	}

	public void TryPlant()
	{
		timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
		int cell = Grid.PosToCell(base.gameObject);
		if (!TestSuitableGround(cell, false))
		{
			return;
		}
		Vector3 position = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(PlantID), position, Grid.SceneLayer.BuildingFront, null, 0);
		gameObject.SetActive(true);
		Pickupable component = GetComponent<Pickupable>();
		Pickupable pickupable = component.Take(1f);
		if (!((Object)pickupable != (Object)null))
		{
			KCrashReporter.Assert(false, "Seed has fractional total amount < 1f");
			return;
		}
		Crop component2 = gameObject.GetComponent<Crop>();
		if (!((Object)component2 != (Object)null))
		{
			goto IL_008f;
		}
		goto IL_008f;
		IL_008f:
		Util.KDestroyGameObject(pickupable.gameObject);
	}

	private bool TestSuitableGround(int cell, bool ignoreGround = false)
	{
		if (Grid.IsValidCell(cell))
		{
			GameObject prefab = Assets.GetPrefab(PlantID);
			EntombVulnerable component = prefab.GetComponent<EntombVulnerable>();
			if ((Object)component != (Object)null && !component.IsCellSafe(cell))
			{
				return false;
			}
			PressureVulnerable component2 = prefab.GetComponent<PressureVulnerable>();
			if ((Object)component2 != (Object)null && !component2.IsCellSafe(cell))
			{
				return false;
			}
			DrowningMonitor component3 = prefab.GetComponent<DrowningMonitor>();
			if ((Object)component3 != (Object)null && !component3.IsCellSafe(cell))
			{
				return false;
			}
			TemperatureVulnerable component4 = prefab.GetComponent<TemperatureVulnerable>();
			if ((Object)component4 != (Object)null && !component4.IsCellSafe(cell))
			{
				return false;
			}
			UprootedMonitor component5 = prefab.GetComponent<UprootedMonitor>();
			if ((Object)component5 != (Object)null && !component5.IsCellSafe(cell))
			{
				return false;
			}
			OccupyArea component6 = prefab.GetComponent<OccupyArea>();
			if ((Object)component6 != (Object)null && !component6.CanOccupyArea(cell, ObjectLayer.Building))
			{
				return false;
			}
			if (!ignoreGround)
			{
				int num = Grid.CellBelow(cell);
				if (Grid.Foundation[num] || (replantGroundTag.IsValid && !Grid.Element[num].HasTag(replantGroundTag)))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
		{
			Descriptor item = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_CEILING, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_CEILING, Descriptor.DescriptorType.Requirement, false);
			list.Add(item);
		}
		else if (direction == SingleEntityReceptacle.ReceptacleDirection.Side)
		{
			Descriptor item2 = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_WALL, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_WALL, Descriptor.DescriptorType.Requirement, false);
			list.Add(item2);
		}
		return list;
	}
}
