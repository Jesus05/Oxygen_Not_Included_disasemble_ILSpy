using FMOD.Studio;
using KSerialization;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name} {WattsUsed}W")]
public class EnergyConsumer : KMonoBehaviour, ISaveLoadable, IEnergyConsumer, IEffectDescriptor
{
	[MyCmpReq]
	private Building building = null;

	[MyCmpGet]
	protected Operational operational = null;

	[MyCmpGet]
	private KSelectable selectable;

	[SerializeField]
	public int powerSortOrder = 0;

	[Serialize]
	protected float circuitOverloadTime = 0f;

	public static readonly Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);

	private Dictionary<string, float> lastTimeSoundPlayed = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private float _BaseWattageRating;

	public int PowerSortOrder => powerSortOrder;

	public int PowerCell
	{
		get;
		private set;
	}

	public bool HasWire => (Object)Grid.Objects[PowerCell, 24] != (Object)null;

	public virtual bool IsPowered
	{
		get
		{
			return operational.GetFlag(PoweredFlag);
		}
		private set
		{
			operational.SetFlag(PoweredFlag, value);
		}
	}

	public bool IsConnected => CircuitID != 65535;

	public string Name => selectable.GetName();

	public ushort CircuitID
	{
		get;
		private set;
	}

	public float BaseWattageRating
	{
		get
		{
			return _BaseWattageRating;
		}
		set
		{
			_BaseWattageRating = value;
		}
	}

	public float WattsUsed
	{
		get
		{
			if (!operational.IsActive)
			{
				return 0f;
			}
			return BaseWattageRating;
		}
	}

	public float WattsNeededWhenActive => building.Def.EnergyConsumptionWhenActive;

	public float BaseWattsNeededWhenActive => building.Def.EnergyConsumptionWhenActive;

	protected override void OnPrefabInit()
	{
		CircuitID = ushort.MaxValue;
		IsPowered = false;
		BaseWattageRating = building.Def.EnergyConsumptionWhenActive;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.EnergyConsumers.Add(this);
		Building component = GetComponent<Building>();
		PowerCell = component.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
		Game.Instance.energySim.AddEnergyConsumer(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveEnergyConsumer(this);
		Game.Instance.circuitManager.Disconnect(this);
		Components.EnergyConsumers.Remove(this);
		base.OnCleanUp();
	}

	public virtual void EnergySim200ms(float dt)
	{
		CircuitID = Game.Instance.circuitManager.GetCircuitID(PowerCell);
		if (!IsConnected)
		{
			IsPowered = false;
		}
		circuitOverloadTime = Mathf.Max(0f, circuitOverloadTime - dt);
	}

	public virtual void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
	{
		switch (connection_status)
		{
		case CircuitManager.ConnectionStatus.NotConnected:
			IsPowered = false;
			break;
		case CircuitManager.ConnectionStatus.Unpowered:
			if (IsPowered && (Object)GetComponent<Battery>() == (Object)null)
			{
				IsPowered = false;
				circuitOverloadTime = 6f;
				PlayCircuitSound("overdraw");
			}
			break;
		case CircuitManager.ConnectionStatus.Powered:
			if (!IsPowered && circuitOverloadTime <= 0f)
			{
				IsPowered = true;
				PlayCircuitSound("powered");
			}
			break;
		}
	}

	protected void PlayCircuitSound(string state)
	{
		string sound = null;
		if (state == "powered")
		{
			sound = Sounds.Instance.BuildingPowerOnMigrated;
		}
		else if (state == "overdraw")
		{
			sound = Sounds.Instance.ElectricGridOverloadMigrated;
		}
		else
		{
			Debug.Log("Invalid state for sound in EnergyConsumer.", null);
		}
		if (CameraController.Instance.IsAudibleSound(base.transform.GetPosition()))
		{
			if (!lastTimeSoundPlayed.TryGetValue(state, out float value))
			{
				value = 0f;
			}
			float value2 = (Time.time - value) / soundDecayTime;
			FMOD.Studio.EventInstance instance = KFMOD.BeginOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(base.transform.GetPosition()));
			instance.setParameterValue("timeSinceLast", value2);
			KFMOD.EndOneShot(instance);
			lastTimeSoundPlayed[state] = Time.time;
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
