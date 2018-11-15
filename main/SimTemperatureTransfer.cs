using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SkipSaveFileSerialization]
public class SimTemperatureTransfer : KMonoBehaviour
{
	private const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;

	public float deltaKJ;

	public Action<SimTemperatureTransfer> onSimRegistered;

	protected int simHandle = -1;

	private float pendingEnergyModifications;

	[SerializeField]
	protected float surfaceArea = 10f;

	[SerializeField]
	protected float thickness = 0.01f;

	[SerializeField]
	protected float groundTransferScale = 0.0625f;

	private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();

	[CompilerGenerated]
	private static PrimaryElement.GetTemperatureCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static PrimaryElement.SetTemperatureCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<int, object> _003C_003Ef__mg_0024cache2;

	public float SurfaceArea
	{
		get
		{
			return surfaceArea;
		}
		set
		{
			surfaceArea = value;
		}
	}

	public float Thickness
	{
		get
		{
			return thickness;
		}
		set
		{
			thickness = value;
		}
	}

	public float GroundTransferScale
	{
		get
		{
			return GroundTransferScale;
		}
		set
		{
			groundTransferScale = value;
		}
	}

	public int SimHandle => simHandle;

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	public static void DoStateTransition(int sim_handle)
	{
		SimTemperatureTransfer value = null;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			SimTemperatureTransfer simTemperatureTransfer = handleInstanceMap[sim_handle];
			if ((UnityEngine.Object)simTemperatureTransfer != (UnityEngine.Object)null && !simTemperatureTransfer.HasTag(GameTags.Sealed))
			{
				PrimaryElement component = simTemperatureTransfer.GetComponent<PrimaryElement>();
				Element element = component.Element;
				if (element.highTempTransitionTarget != SimHashes.Unobtanium)
				{
					if (component.Mass > 0f)
					{
						int gameCell = Grid.PosToCell(simTemperatureTransfer.transform.GetPosition());
						SimMessages.AddRemoveSubstance(gameCell, element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
					}
					Util.KDestroyGameObject(simTemperatureTransfer.gameObject);
				}
			}
		}
	}

	protected override void OnPrefabInit()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.getTemperatureCallback = OnGetTemperature;
		component.setTemperatureCallback = OnSetTemperature;
		PrimaryElement primaryElement = component;
		primaryElement.onDataChanged = (Action<PrimaryElement>)Delegate.Combine(primaryElement.onDataChanged, new Action<PrimaryElement>(OnDataChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PrimaryElement component = GetComponent<PrimaryElement>();
		Element element = component.Element;
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "SimTemperatureTransfer.OnSpawn");
		if (component.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
		{
			base.enabled = false;
		}
		SimRegister();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (Sim.IsValidHandle(simHandle))
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			OnSetTemperature(component, component.Temperature);
		}
	}

	protected override void OnCmpDisable()
	{
		if (Sim.IsValidHandle(simHandle))
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			float temperature = component.Temperature;
			component.InternalTemperature = component.Temperature;
			SimMessages.SetElementChunkData(simHandle, temperature, 0f);
		}
		base.OnCmpDisable();
	}

	private void OnCellChanged()
	{
		int cell = Grid.PosToCell(this);
		if (!Grid.IsValidCell(cell))
		{
			base.enabled = false;
		}
		else if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.MoveElementChunk(simHandle, cell);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChanged);
		SimUnregister();
		base.OnForcedCleanUp();
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(simHandle, delta_kilojoules);
		}
		else
		{
			pendingEnergyModifications += delta_kilojoules;
		}
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		float result;
		if (Sim.IsValidHandle(component.simHandle))
		{
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			result = Game.Instance.simData.elementChunks[handleIndex].temperature;
			component.deltaKJ = Game.Instance.simData.elementChunks[handleIndex].deltaKJ;
		}
		else
		{
			result = primary_element.InternalTemperature;
		}
		return result;
	}

	private unsafe static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		if (temperature <= 0f)
		{
			KCrashReporter.Assert(false, "STT.OnSetTemperature - Tried to set <= 0 degree temperature");
			temperature = 293f;
		}
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		if (Sim.IsValidHandle(component.simHandle))
		{
			float mass = primary_element.Mass;
			float heat_capacity = (!(mass >= 0.01f)) ? 0f : (mass * primary_element.Element.specificHeatCapacity);
			SimMessages.SetElementChunkData(component.simHandle, temperature, heat_capacity);
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			Game.Instance.simData.elementChunks[handleIndex].temperature = temperature;
		}
		else
		{
			primary_element.InternalTemperature = temperature;
		}
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			float heat_capacity = (!(primary_element.Mass >= 0.01f)) ? 0f : (primary_element.Mass * primary_element.Element.specificHeatCapacity);
			SimMessages.SetElementChunkData(simHandle, primary_element.Temperature, heat_capacity);
		}
	}

	protected void SimRegister()
	{
		if (base.isSpawned && simHandle == -1 && base.enabled)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				Element element = component.Element;
				if (!element.IsTemperatureInsulated)
				{
					int gameCell = Grid.PosToCell(base.transform.GetPosition());
					simHandle = -2;
					HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add(OnSimRegisteredCallback, this, "SimTemperatureTransfer.SimRegister");
					float num = component.InternalTemperature;
					KCrashReporter.Assert(num > 0f, "Invalid temperature");
					KCrashReporter.Assert(component.Mass > 0f);
					if (num <= 0f)
					{
						component.InternalTemperature = 293f;
						num = 293f;
					}
					SimMessages.AddElementChunk(gameCell, component.ElementID, component.Mass, num, surfaceArea, thickness, groundTransferScale, handle.index);
				}
			}
		}
	}

	protected unsafe void SimUnregister()
	{
		if (simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (Sim.IsValidHandle(simHandle))
			{
				int handleIndex = Sim.GetHandleIndex(simHandle);
				component.InternalTemperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
				SimMessages.RemoveElementChunk(simHandle, -1);
				handleInstanceMap.Remove(simHandle);
			}
			simHandle = -1;
		}
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((SimTemperatureTransfer)data).OnSimRegistered(handle);
	}

	private unsafe void OnSimRegistered(int handle)
	{
		if ((UnityEngine.Object)this != (UnityEngine.Object)null && simHandle == -2)
		{
			simHandle = handle;
			int handleIndex = Sim.GetHandleIndex(handle);
			float temperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
			if (temperature <= 0f)
			{
				KCrashReporter.Assert(false, "Bad temperature");
			}
			handleInstanceMap[simHandle] = this;
			if (pendingEnergyModifications > 0f)
			{
				ModifyEnergy(pendingEnergyModifications);
				pendingEnergyModifications = 0f;
			}
			if (onSimRegistered != null)
			{
				onSimRegistered(this);
			}
		}
		else
		{
			SimMessages.RemoveElementChunk(handle, -1);
		}
	}
}
