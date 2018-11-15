using KSerialization;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OxidizerTank : Storage, ISingleSliderControl, ISliderControl
{
	private MeterController meter;

	private bool isSuspended;

	[Serialize]
	public float targetFillMass = 2700f;

	[SerializeField]
	private Tag[] oxidizerTypes = new Tag[2]
	{
		"Oxylite".ToTag(),
		"LiquidOxygen".ToTag()
	};

	public float minimumLaunchMass = TUNING.BUILDINGS.ROCKETRY_MASS_KG.OXIDIZER_TANK_OXIDIZER_MASS[0];

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

	public string SliderTitleKey => "STRINGS.BUILDINGS.PREFABS.OXIDIZERTANK.NAME";

	public string SliderUnits => UI.UNITSUFFIXES.MASS.KILOGRAM;

	public Tag[] OxidizerTypes
	{
		get
		{
			return oxidizerTypes;
		}
		set
		{
			oxidizerTypes = value;
			if (storageFilters == null)
			{
				storageFilters = new List<Tag>();
			}
			Tag[] array = oxidizerTypes;
			foreach (Tag item in array)
			{
				storageFilters.Add(item);
			}
		}
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 2700f;
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
		return "STRINGS.UI.UISIDESCREENS.OXIDIZERTANK.FUELAMOUNT";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, OnReturn);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		Subscribe(-1697596308, delegate
		{
			meter.SetPositionPercent(MassStored() / capacityKg);
		});
	}

	public void FillTank(SimHashes element)
	{
		if (ElementLoader.FindElementByHash(element).IsLiquid)
		{
			AddLiquid(element, targetFillMass, ElementLoader.FindElementByHash(element).defaultValues.temperature, 0, 0, false, true);
		}
		else if (ElementLoader.FindElementByHash(element).IsSolid)
		{
			GameObject go = ElementLoader.FindElementByHash(element).substance.SpawnResource(base.gameObject.transform.GetPosition(), TargetFillMass, 300f, byte.MaxValue, 0, false, false);
			Store(go, false, false, true, false);
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
}
