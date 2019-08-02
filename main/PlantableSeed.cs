using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PlantableSeed : KMonoBehaviour, IReceptacleDirection, IGameObjectEffectDescriptor
{
	public Tag PlantID;

	public Tag PreviewID;

	[Serialize]
	public float timeUntilSelfPlant;

	public Tag replantGroundTag;

	public string domesticatedDescription;

	public SingleEntityReceptacle.ReceptacleDirection direction;

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnSplitDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnSplit(data);
	});

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

	public void TryPlant(bool allow_plant_from_storage = false)
	{
		timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
		if (!allow_plant_from_storage && base.gameObject.HasTag(GameTags.Stored))
		{
			return;
		}
		int cell = Grid.PosToCell(base.gameObject);
		if (!TestSuitableGround(cell))
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
			goto IL_00a5;
		}
		goto IL_00a5;
		IL_00a5:
		Util.KDestroyGameObject(pickupable.gameObject);
	}

	public bool TestSuitableGround(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = (Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(cell) : Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (Grid.Foundation[num])
		{
			return false;
		}
		if (Grid.Element[num].hardness >= 150)
		{
			return false;
		}
		if (replantGroundTag.IsValid && !Grid.Element[num].HasTag(replantGroundTag))
		{
			return false;
		}
		GameObject prefab = Assets.GetPrefab(PlantID);
		EntombVulnerable component = prefab.GetComponent<EntombVulnerable>();
		if ((Object)component != (Object)null && !component.IsCellSafe(cell))
		{
			return false;
		}
		DrowningMonitor component2 = prefab.GetComponent<DrowningMonitor>();
		if ((Object)component2 != (Object)null && !component2.IsCellSafe(cell))
		{
			return false;
		}
		TemperatureVulnerable component3 = prefab.GetComponent<TemperatureVulnerable>();
		if ((Object)component3 != (Object)null && !component3.IsCellSafe(cell))
		{
			return false;
		}
		UprootedMonitor component4 = prefab.GetComponent<UprootedMonitor>();
		if ((Object)component4 != (Object)null && !component4.IsCellSafe(cell))
		{
			return false;
		}
		OccupyArea component5 = prefab.GetComponent<OccupyArea>();
		if ((Object)component5 != (Object)null && !component5.CanOccupyArea(cell, ObjectLayer.Building))
		{
			return false;
		}
		return true;
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
