using KSerialization;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FuelTank : Storage, ISingleSliderControl, ISliderControl
{
	private bool isSuspended;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = TUNING.BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	[SerializeField]
	private Tag fuelType;

	public float minimumLaunchMass = TUNING.BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	public bool IsSuspended => isSuspended;

	public float TargetFillMass
	{
		get
		{
			return targetFillMass;
		}
		set
		{
			targetFillMass = value;
			capacityKg = targetFillMass;
			float num = MassStored();
			if (capacityKg < num)
			{
				DropAll(false);
			}
		}
	}

	public Tag FuelType
	{
		get
		{
			return fuelType;
		}
		set
		{
			fuelType = value;
			if (storageFilters == null)
			{
				storageFilters = new List<Tag>();
			}
			storageFilters.Add(fuelType);
		}
	}

	public string SliderTitleKey => "STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANK.NAME";

	public string SliderUnits => UI.UNITSUFFIXES.MASS.KILOGRAM;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, OnReturn);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.TransferArm, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		Subscribe(-1697596308, delegate
		{
			meter.SetPositionPercent(MassStored() / capacityKg);
		});
	}

	public void FillTank()
	{
		CommandModule commandModule = null;
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			commandModule = item.GetComponent<CommandModule>();
			if ((bool)commandModule)
			{
				break;
			}
		}
		if ((Object)commandModule != (Object)null)
		{
			RocketEngine mainEngine = commandModule.rocketStats.GetMainEngine();
			if ((Object)mainEngine != (Object)null)
			{
				AddLiquid(ElementLoader.GetElementID(mainEngine.fuelTag), minimumLaunchMass - MassStored(), ElementLoader.GetElement(mainEngine.fuelTag).defaultValues.temperature, 0, 0, false, true);
			}
		}
		else
		{
			Debug.LogWarning("Fuel tank couldn't find command module", null);
		}
	}

	private void OnReturn(object data)
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(items[num]);
		}
		items.Clear();
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 900f;
	}

	public float GetSliderValue(int index)
	{
		return TargetFillMass;
	}

	public void SetSliderValue(float mass, int index)
	{
		TargetFillMass = mass;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.LIQUIDFUELTANK.FUELAMOUNT";
	}
}
