using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class OxidizerTank : KMonoBehaviour, IUserControlledCapacity
{
	public Storage storage;

	private MeterController meter;

	private bool isSuspended;

	[Serialize]
	public float targetFillMass = 2700f;

	[SerializeField]
	private Tag[] oxidizerTypes = new Tag[2]
	{
		SimHashes.OxyRock.CreateTag(),
		SimHashes.LiquidOxygen.CreateTag()
	};

	private static readonly EventSystem.IntraObjectHandler<OxidizerTank> OnReturnRocketDelegate = new EventSystem.IntraObjectHandler<OxidizerTank>(delegate(OxidizerTank component, object data)
	{
		component.OnReturn(data);
	});

	private static readonly EventSystem.IntraObjectHandler<OxidizerTank> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<OxidizerTank>(delegate(OxidizerTank component, object data)
	{
		component.OnStorageChange(data);
	});

	public bool IsSuspended => isSuspended;

	public float UserMaxCapacity
	{
		get
		{
			return targetFillMass;
		}
		set
		{
			targetFillMass = value;
			storage.capacityKg = targetFillMass;
			ConduitConsumer component = GetComponent<ConduitConsumer>();
			if ((Object)component != (Object)null)
			{
				component.capacityKG = targetFillMass;
			}
			ManualDeliveryKG component2 = GetComponent<ManualDeliveryKG>();
			if ((Object)component2 != (Object)null)
			{
				component2.capacity = (component2.refillMass = targetFillMass);
			}
			Trigger(-945020481, this);
		}
	}

	public float MinCapacity => 0f;

	public float MaxCapacity => 2700f;

	public float AmountStored => storage.MassStored();

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit(false);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		UserMaxCapacity = UserMaxCapacity;
		Subscribe(1366341636, OnReturnRocketDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
	}

	public float MassStored()
	{
		return storage.MassStored();
	}

	public float GetTotalOxidizerAvailable()
	{
		float num = 0f;
		Tag[] array = oxidizerTypes;
		foreach (Tag tag in array)
		{
			num += storage.GetAmountAvailable(tag);
		}
		return num;
	}

	public Dictionary<Tag, float> GetOxidizersAvailable()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		Tag[] array = oxidizerTypes;
		foreach (Tag tag in array)
		{
			dictionary[tag] = storage.GetAmountAvailable(tag);
		}
		return dictionary;
	}

	[ContextMenu("Fill Tank")]
	public void FillTank(SimHashes element)
	{
		if (ElementLoader.FindElementByHash(element).IsLiquid)
		{
			storage.AddLiquid(element, targetFillMass, ElementLoader.FindElementByHash(element).defaultValues.temperature, 0, 0, false, true);
		}
		else if (ElementLoader.FindElementByHash(element).IsSolid)
		{
			GameObject go = ElementLoader.FindElementByHash(element).substance.SpawnResource(base.gameObject.transform.GetPosition(), targetFillMass, 300f, byte.MaxValue, 0, false, false);
			storage.Store(go, false, false, true, false);
		}
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
	}

	private void OnReturn(object data)
	{
		storage.ConsumeAllIgnoringDisease();
	}
}
