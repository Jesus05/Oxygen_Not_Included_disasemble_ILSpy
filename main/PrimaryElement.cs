using Klei;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PrimaryElement : KMonoBehaviour, ISaveLoadable
{
	public delegate float GetTemperatureCallback(PrimaryElement primary_element);

	public delegate void SetTemperatureCallback(PrimaryElement primary_element, float temperature);

	public GetTemperatureCallback getTemperatureCallback = OnGetTemperature;

	public SetTemperatureCallback setTemperatureCallback = OnSetTemperature;

	public Action<int, string> ModifyDiseaseCountHandler;

	public Action<byte, int, string> AddDiseaseHandler;

	private bool useSimDiseaseInfo = false;

	public const float DefaultChunkMass = 400f;

	private static readonly Tag[] metalTags = new Tag[2]
	{
		GameTags.Metal,
		GameTags.RefinedMetal
	};

	[Serialize]
	[HashedEnum]
	public SimHashes ElementID = (SimHashes)0;

	private float _units = 1f;

	[Serialize]
	[SerializeField]
	private float _Temperature;

	[NonSerialized]
	[Serialize]
	public bool KeepZeroMassObject = false;

	[Serialize]
	private HashedString diseaseID;

	[Serialize]
	private int diseaseCount;

	private HandleVector<int>.Handle diseaseHandle = HandleVector<int>.InvalidHandle;

	public float MassPerUnit = 1f;

	[NonSerialized]
	private Element _Element = null;

	[NonSerialized]
	public Action<PrimaryElement> onDataChanged;

	[NonSerialized]
	private bool forcePermanentDiseaseContainer = false;

	private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>(delegate(PrimaryElement component, object data)
	{
		component.OnSplitFromChunk(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>(delegate(PrimaryElement component, object data)
	{
		component.OnAbsorb(data);
	});

	[CompilerGenerated]
	private static GetTemperatureCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static SetTemperatureCallback _003C_003Ef__mg_0024cache1;

	[Serialize]
	public float Units
	{
		get
		{
			return _units;
		}
		set
		{
			_units = value;
		}
	}

	public float Temperature
	{
		get
		{
			return getTemperatureCallback(this);
		}
		set
		{
			SetTemperature(value);
		}
	}

	public float InternalTemperature
	{
		get
		{
			return _Temperature;
		}
		set
		{
			_Temperature = value;
		}
	}

	public float Mass
	{
		get
		{
			return Units * MassPerUnit;
		}
		set
		{
			SetMass(value);
			if (onDataChanged != null)
			{
				onDataChanged(this);
			}
		}
	}

	public Element Element
	{
		get
		{
			if (_Element == null)
			{
				_Element = ElementLoader.FindElementByHash(ElementID);
			}
			return _Element;
		}
	}

	public byte DiseaseIdx
	{
		get
		{
			byte result = byte.MaxValue;
			if (useSimDiseaseInfo)
			{
				int i = Grid.PosToCell(base.transform.GetPosition());
				result = Grid.DiseaseIdx[i];
			}
			else if (diseaseHandle.IsValid())
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(diseaseHandle);
				result = header.diseaseIdx;
			}
			return result;
		}
	}

	public int DiseaseCount
	{
		get
		{
			int result = 0;
			if (useSimDiseaseInfo)
			{
				int i = Grid.PosToCell(base.transform.GetPosition());
				result = Grid.DiseaseCount[i];
			}
			else if (diseaseHandle.IsValid())
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(diseaseHandle);
				result = header.diseaseCount;
			}
			return result;
		}
	}

	public void SetUseSimDiseaseInfo(bool use)
	{
		useSimDiseaseInfo = use;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		_Temperature = Temperature;
		SanitizeMassAndTemperature();
		diseaseID.HashValue = 0;
		diseaseCount = 0;
		if (useSimDiseaseInfo)
		{
			int i = Grid.PosToCell(base.transform.GetPosition());
			if (Grid.DiseaseIdx[i] != 255)
			{
				diseaseID = Db.Get().Diseases[Grid.DiseaseIdx[i]].id;
				diseaseCount = Grid.DiseaseCount[i];
			}
		}
		else if (diseaseHandle.IsValid())
		{
			DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(diseaseHandle);
			if (header.diseaseIdx != 255)
			{
				diseaseID = Db.Get().Diseases[header.diseaseIdx].id;
				diseaseCount = header.diseaseCount;
			}
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (ElementID == (SimHashes)351109216)
		{
			ElementID = SimHashes.Creature;
		}
		SanitizeMassAndTemperature();
		float temperature = _Temperature;
		if (float.IsNaN(temperature) || float.IsInfinity(temperature) || temperature < 0f || 10000f < temperature)
		{
			DeserializeWarnings.Instance.PrimaryElementTemperatureIsNan.Warn($"{base.name} has invalid temperature of {Temperature}. Resetting temperature.", null);
			temperature = Element.defaultValues.temperature;
		}
		_Temperature = temperature;
		Temperature = temperature;
		if (Element == null)
		{
			DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + "Primary element has no element.", null);
		}
		if (Mass > 100000f)
		{
			Output.LogWarningWithObj(base.gameObject, "deserialized very large ore mass... error?");
		}
		if (Mass < 0f)
		{
			Output.LogErrorWithObj(base.gameObject, "deserialized ore with less than 0 mass. Error! Destroying");
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			if (onDataChanged != null)
			{
				onDataChanged(this);
			}
			byte index = Db.Get().Diseases.GetIndex(diseaseID);
			if (index == 255 || diseaseCount <= 0)
			{
				if (diseaseHandle.IsValid())
				{
					GameComps.DiseaseContainers.Remove(base.gameObject);
					diseaseHandle.Clear();
				}
			}
			else if (diseaseHandle.IsValid())
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(diseaseHandle);
				header.diseaseIdx = index;
				header.diseaseCount = diseaseCount;
				GameComps.DiseaseContainers.SetHeader(diseaseHandle, header);
			}
			else
			{
				diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, index, diseaseCount);
			}
		}
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
	}

	private void SanitizeMassAndTemperature()
	{
		if (_Temperature <= 0f)
		{
			KCrashReporter.Assert(false, base.gameObject.name + " is attempting to serialize a temperature of <= 0K. Resetting to default.");
			_Temperature = Element.defaultValues.temperature;
		}
		if (Mass > 100000f)
		{
			KCrashReporter.Assert(false, base.gameObject.name + $" is attempting to serialize a mass of {Mass}. Resetting to default.");
			Mass = Element.defaultValues.mass;
		}
	}

	private void SetMass(float mass)
	{
		Units = mass / MassPerUnit;
		if (Units <= 0f && !KeepZeroMassObject)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		else if (!KeepZeroMassObject && Units <= 0f)
		{
			throw new ArgumentException("Invalid mass");
		}
		if (Units > 100000f && ElementID != SimHashes.Regolith)
		{
			KCrashReporter.Assert(false, base.gameObject.name + $" is getting an abnormal mass set: {Units}.");
		}
	}

	private void SetTemperature(float temperature)
	{
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Output.LogErrorWithObj(base.gameObject, "Invalid temperature [" + temperature + "]");
		}
		else
		{
			if (temperature <= 0f)
			{
				KCrashReporter.Assert(false, "Tried to set PrimaryElement.Temperature to a value <= 0");
			}
			setTemperatureCallback(this, temperature);
		}
	}

	public void SetMassTemperature(float mass, float temperature)
	{
		SetMass(mass);
		SetTemperature(temperature);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.InfraredVisualizers.Add(base.gameObject);
		Subscribe(1335436905, OnSplitFromChunkDelegate);
		Subscribe(-2064133523, OnAbsorbDelegate);
	}

	protected override void OnSpawn()
	{
		Attributes attributes = this.GetAttributes();
		if (attributes != null)
		{
			Element element = Element;
			try
			{
				foreach (AttributeModifier attributeModifier in element.attributeModifiers)
				{
					attributes.Add(attributeModifier);
				}
			}
			catch
			{
				Debug.Log("!", null);
			}
		}
	}

	public void ForcePermanentDiseaseContainer(bool force_on)
	{
		if (force_on)
		{
			if (!diseaseHandle.IsValid())
			{
				diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, byte.MaxValue, 0);
			}
		}
		else if (diseaseHandle.IsValid() && DiseaseIdx == 255)
		{
			GameComps.DiseaseContainers.Remove(base.gameObject);
			diseaseHandle.Clear();
		}
		forcePermanentDiseaseContainer = force_on;
	}

	protected override void OnCleanUp()
	{
		GameComps.InfraredVisualizers.Remove(base.gameObject);
		if (diseaseHandle.IsValid())
		{
			GameComps.DiseaseContainers.Remove(base.gameObject);
			diseaseHandle.Clear();
		}
		base.OnCleanUp();
	}

	public void SetElement(SimHashes element_id)
	{
		ElementID = element_id;
		UpdateTags();
	}

	public void UpdateTags()
	{
		if (ElementID == (SimHashes)0)
		{
			Output.LogWithObj(base.gameObject, "UpdateTags() Primary element 0");
		}
		else
		{
			KPrefabID component = GetComponent<KPrefabID>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				List<Tag> list = new List<Tag>();
				Element element = Element;
				list.Add(GameTagExtensions.Create(element.id));
				Tag[] oreTags = element.oreTags;
				foreach (Tag item in oreTags)
				{
					list.Add(item);
				}
				if (component.HasAnyTags(metalTags))
				{
					list.Add(GameTags.StoredMetal);
				}
				foreach (Tag item2 in list)
				{
					component.AddTag(item2);
				}
			}
		}
	}

	public void ModifyDiseaseCount(int delta, string reason)
	{
		if (ModifyDiseaseCountHandler != null)
		{
			ModifyDiseaseCountHandler(delta, reason);
		}
		else if (useSimDiseaseInfo)
		{
			int gameCell = Grid.PosToCell(this);
			SimMessages.ModifyDiseaseOnCell(gameCell, byte.MaxValue, delta);
		}
		else if (delta != 0 && diseaseHandle.IsValid())
		{
			int num = GameComps.DiseaseContainers.ModifyDiseaseCount(diseaseHandle, delta);
			if (num <= 0 && !forcePermanentDiseaseContainer)
			{
				Trigger(-1689370368, false);
				GameComps.DiseaseContainers.Remove(base.gameObject);
				diseaseHandle.Clear();
			}
		}
	}

	public void AddDisease(byte disease_idx, int delta, string reason)
	{
		if (delta != 0)
		{
			if (AddDiseaseHandler != null)
			{
				AddDiseaseHandler(disease_idx, delta, reason);
			}
			else if (useSimDiseaseInfo)
			{
				int gameCell = Grid.PosToCell(this);
				SimMessages.ModifyDiseaseOnCell(gameCell, disease_idx, delta);
			}
			else if (diseaseHandle.IsValid())
			{
				int num = GameComps.DiseaseContainers.AddDisease(diseaseHandle, disease_idx, delta);
				if (num <= 0)
				{
					GameComps.DiseaseContainers.Remove(base.gameObject);
					diseaseHandle.Clear();
				}
			}
			else if (delta > 0)
			{
				diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, disease_idx, delta);
				Trigger(-1689370368, true);
				Trigger(-283306403, null);
			}
		}
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		return primary_element._Temperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		if (temperature <= 0f)
		{
			Output.LogErrorWithObj(primary_element.gameObject, primary_element.gameObject.name + " has a temperature of zero which has always been an error in my experience.");
		}
		primary_element._Temperature = temperature;
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (!((UnityEngine.Object)pickupable == (UnityEngine.Object)null))
		{
			float percent = Units / (Units + pickupable.PrimaryElement.Units);
			SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(pickupable.PrimaryElement, percent);
			AddDisease(percentOfDisease.idx, percentOfDisease.count, "PrimaryElement.SplitFromChunk");
			pickupable.PrimaryElement.ModifyDiseaseCount(-percentOfDisease.count, "PrimaryElement.SplitFromChunk");
		}
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (!((UnityEngine.Object)pickupable == (UnityEngine.Object)null))
		{
			AddDisease(pickupable.PrimaryElement.DiseaseIdx, pickupable.PrimaryElement.DiseaseCount, "PrimaryElement.OnAbsorb");
		}
	}

	public void SetDiseaseVisualProvider(GameObject visualizer)
	{
		HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(base.gameObject);
		if (handle != HandleVector<int>.InvalidHandle)
		{
			DiseaseContainer new_data = GameComps.DiseaseContainers.GetPayload(handle);
			new_data.visualDiseaseProvider = visualizer;
			GameComps.DiseaseContainers.SetPayload(handle, ref new_data);
		}
	}
}
