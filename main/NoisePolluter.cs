using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class NoisePolluter : KMonoBehaviour, IPolluter
{
	public const string ID = "NoisePolluter";

	public int radius;

	public int noise;

	public AttributeInstance dB;

	public AttributeInstance dBRadius;

	private NoiseSplat splat;

	public System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectNoisePollutersCallback;

	public bool isMovable;

	[MyCmpReq]
	public OccupyArea occupyArea;

	private static readonly EventSystem.IntraObjectHandler<NoisePolluter> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<NoisePolluter>(delegate(NoisePolluter component, object data)
	{
		component.OnActiveChanged(data);
	});

	public string sourceName
	{
		get;
		private set;
	}

	public bool active
	{
		get;
		private set;
	}

	public static bool IsNoiseableCell(int cell)
	{
		return Grid.IsValidCell(cell) && (Grid.IsGas(cell) || !Grid.IsSubstantialLiquid(cell, 0.35f));
	}

	public void ResetCells()
	{
		if (radius == 0)
		{
			Debug.LogFormat("[{0}] has a 0 radius noise, this will disable it", GetName());
		}
	}

	public void SetAttributes(Vector2 pos, int dB, GameObject go, string name)
	{
		sourceName = name;
		noise = dB;
	}

	public int GetRadius()
	{
		return radius;
	}

	public int GetNoise()
	{
		return noise;
	}

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void SetSplat(NoiseSplat new_splat)
	{
		splat = new_splat;
	}

	public void Clear()
	{
		if (splat != null)
		{
			splat.Clear();
			splat = null;
		}
	}

	public Vector2 GetPosition()
	{
		return base.transform.GetPosition();
	}

	public void SetActive(bool active = true)
	{
		if (!active && splat != null)
		{
			AudioEventManager.Get().ClearNoiseSplat(splat);
			splat.Clear();
		}
		this.active = active;
	}

	public void Refresh()
	{
		if (active)
		{
			if (splat != null)
			{
				AudioEventManager.Get().ClearNoiseSplat(splat);
				splat.Clear();
			}
			KSelectable component = GetComponent<KSelectable>();
			string name = (!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? base.name : component.GetName();
			GameObject gameObject = GetComponent<KMonoBehaviour>().gameObject;
			splat = AudioEventManager.Get().CreateNoiseSplat(GetPosition(), noise, radius, name, gameObject);
		}
	}

	private void OnActiveChanged(object data)
	{
		bool isActive = ((Operational)data).IsActive;
		SetActive(isActive);
		Refresh();
	}

	public void SetValues(EffectorValues values)
	{
		noise = values.amount;
		radius = values.radius;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (radius == 0 || noise == 0)
		{
			Debug.LogWarning("Noisepollutor::OnSpawn [" + GetName() + "] noise: [" + noise + "] radius: [" + radius + "]", null);
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			ResetCells();
			Operational component = GetComponent<Operational>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				Subscribe(824508782, OnActiveChangedDelegate);
			}
			refreshCallback = Refresh;
			refreshPartionerCallback = delegate
			{
				Refresh();
			};
			onCollectNoisePollutersCallback = OnCollectNoisePolluters;
			Attributes attributes = this.GetAttributes();
			Db db = Db.Get();
			dB = attributes.Add(db.BuildingAttributes.NoisePollution);
			dBRadius = attributes.Add(db.BuildingAttributes.NoisePollutionRadius);
			if (noise != 0 && radius != 0)
			{
				AttributeModifier modifier = new AttributeModifier(db.BuildingAttributes.NoisePollution.Id, (float)noise, UI.TOOLTIPS.BASE_VALUE, false, false, true);
				AttributeModifier modifier2 = new AttributeModifier(db.BuildingAttributes.NoisePollutionRadius.Id, (float)radius, UI.TOOLTIPS.BASE_VALUE, false, false, true);
				attributes.Add(modifier);
				attributes.Add(modifier2);
			}
			else
			{
				Debug.LogWarning("Noisepollutor::OnSpawn [" + GetName() + "] radius: [" + radius + "] noise: [" + noise + "]", null);
			}
			KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
			isMovable = ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.isMovable);
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "NoisePolluter.OnSpawn");
			AttributeInstance attributeInstance = dB;
			attributeInstance.OnDirty = (System.Action)Delegate.Combine(attributeInstance.OnDirty, refreshCallback);
			AttributeInstance attributeInstance2 = dBRadius;
			attributeInstance2.OnDirty = (System.Action)Delegate.Combine(attributeInstance2.OnDirty, refreshCallback);
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				OnActiveChanged(component.IsActive);
			}
		}
	}

	private void OnCellChange()
	{
		Refresh();
	}

	private void OnCollectNoisePolluters(object data)
	{
		List<NoisePolluter> list = (List<NoisePolluter>)data;
		list.Add(this);
	}

	public string GetName()
	{
		if (string.IsNullOrEmpty(sourceName))
		{
			sourceName = GetComponent<KSelectable>().GetName();
		}
		return sourceName;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			if (dB != null)
			{
				AttributeInstance attributeInstance = dB;
				attributeInstance.OnDirty = (System.Action)Delegate.Remove(attributeInstance.OnDirty, refreshCallback);
				AttributeInstance attributeInstance2 = dBRadius;
				attributeInstance2.OnDirty = (System.Action)Delegate.Remove(attributeInstance2.OnDirty, refreshCallback);
			}
			if (isMovable)
			{
				Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
			}
		}
		if (splat != null)
		{
			AudioEventManager.Get().ClearNoiseSplat(splat);
			splat.Clear();
		}
	}

	public float GetNoiseForCell(int cell)
	{
		return splat.GetDBForCell(cell);
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (dB != null && dBRadius != null)
		{
			float totalValue = dB.GetTotalValue();
			float totalValue2 = dBRadius.GetTotalValue();
			string str = (noise <= 0) ? UI.BUILDINGEFFECTS.TOOLTIPS.NOISE_POLLUTION_DECREASE : UI.BUILDINGEFFECTS.TOOLTIPS.NOISE_POLLUTION_INCREASE;
			str = str + "\n\n" + dB.GetAttributeValueTooltip();
			string arg = GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f);
			Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.NOISE_CREATED, arg, totalValue2), string.Format(str, arg, totalValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(item);
		}
		else if (noise != 0)
		{
			string format = (noise < 0) ? UI.BUILDINGEFFECTS.TOOLTIPS.NOISE_POLLUTION_DECREASE : UI.BUILDINGEFFECTS.TOOLTIPS.NOISE_POLLUTION_INCREASE;
			string arg2 = GameUtil.AddPositiveSign(noise.ToString(), noise > 0);
			Descriptor item2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.NOISE_CREATED, arg2, radius), string.Format(format, arg2, radius), Descriptor.DescriptorType.Effect, false);
			list.Add(item2);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return GetEffectDescriptions();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}
}
