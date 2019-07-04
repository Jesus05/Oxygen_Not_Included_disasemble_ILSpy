using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoBay : KMonoBehaviour
{
	public enum CargoType
	{
		solids,
		liquids,
		gasses,
		entities
	}

	public Storage storage;

	private MeterController meter;

	public CargoType storageType = CargoType.solids;

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnLand(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		Subscribe(-1697596308, delegate
		{
			meter.SetPositionPercent(storage.MassStored() / storage.Capacity());
		});
	}

	private void OnRefreshUserMenu(object data)
	{
		string iconName = "action_empty_contents";
		string text = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME;
		System.Action on_click = delegate
		{
			storage.DropAll(false, false, default(Vector3), true);
		};
		string tooltipText = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
		KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void SpawnResources(object data)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>()));
		int rootCell = Grid.PosToCell(base.gameObject);
		foreach (KeyValuePair<SimHashes, float> item in spacecraftDestination.GetMissionResourceResult(storage.RemainingCapacity(), storageType == CargoType.solids, storageType == CargoType.liquids, storageType == CargoType.gasses))
		{
			Element element = ElementLoader.FindElementByHash(item.Key);
			if (storageType == CargoType.solids && element.IsSolid)
			{
				GameObject gameObject = Scenario.SpawnPrefab(rootCell, 0, 0, element.tag.Name, Grid.SceneLayer.Ore);
				gameObject.GetComponent<PrimaryElement>().Mass = item.Value;
				gameObject.GetComponent<PrimaryElement>().Temperature = ElementLoader.FindElementByHash(item.Key).defaultValues.temperature;
				gameObject.SetActive(true);
				storage.Store(gameObject, false, false, true, false);
			}
			else if (storageType == CargoType.liquids && element.IsLiquid)
			{
				storage.AddLiquid(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0, false, true);
			}
			else if (storageType == CargoType.gasses && element.IsGas)
			{
				storage.AddGasChunk(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0, false, true);
			}
		}
		if (storageType == CargoType.entities)
		{
			foreach (KeyValuePair<Tag, int> item2 in spacecraftDestination.GetMissionEntityResult())
			{
				GameObject prefab = Assets.GetPrefab(item2.Key);
				if ((UnityEngine.Object)prefab == (UnityEngine.Object)null)
				{
					KCrashReporter.Assert(false, "Missing prefab: " + item2.Key.Name);
				}
				else
				{
					for (int i = 0; i < item2.Value; i++)
					{
						GameObject gameObject2 = Util.KInstantiate(prefab, base.transform.position);
						gameObject2.SetActive(true);
						storage.Store(gameObject2, false, false, true, false);
						Baggable component = gameObject2.GetComponent<Baggable>();
						if ((UnityEngine.Object)component != (UnityEngine.Object)null)
						{
							component.SetWrangled();
						}
					}
				}
			}
		}
	}

	public void OnLaunch(object data)
	{
		ReserveResources();
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.conduitType = ConduitType.None;
		}
	}

	private void ReserveResources()
	{
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>());
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftID);
		spacecraftDestination.UpdateRemainingResources(this);
	}

	public void OnLand(object data)
	{
		SpawnResources(data);
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			switch (storageType)
			{
			case CargoType.gasses:
				component.conduitType = ConduitType.Gas;
				break;
			case CargoType.liquids:
				component.conduitType = ConduitType.Liquid;
				break;
			default:
				component.conduitType = ConduitType.None;
				break;
			}
		}
	}
}
