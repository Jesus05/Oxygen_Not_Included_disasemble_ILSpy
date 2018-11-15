using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Wire : KMonoBehaviour, IDisconnectable, IFirstFrameCallback, IWattageRating, IHaveUtilityNetworkMgr, IUtilityNetworkItem, IBridgedNetworkItem
{
	public enum WattageRating
	{
		Max500,
		Max1000,
		Max2000,
		Max20000,
		Max50000,
		NumRatings
	}

	[SerializeField]
	public WattageRating MaxWattageRating;

	[SerializeField]
	private bool disconnected = true;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	public float circuitOverloadTime;

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private static StatusItem WireCircuitStatus = null;

	private static StatusItem WireMaxWattageStatus = null;

	private System.Action firstFrameCallback;

	public bool IsConnected
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell) as ElectricalUtilityNetwork;
			return electricalUtilityNetwork != null;
		}
	}

	public ushort NetworkID
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell) as ElectricalUtilityNetwork;
			return (ushort)((electricalUtilityNetwork == null) ? 65535 : ((ushort)electricalUtilityNetwork.id));
		}
	}

	public static float GetMaxWattageAsFloat(WattageRating rating)
	{
		switch (rating)
		{
		case WattageRating.Max500:
			return 500f;
		case WattageRating.Max1000:
			return 1000f;
		case WattageRating.Max2000:
			return 2000f;
		case WattageRating.Max20000:
			return 20000f;
		case WattageRating.Max50000:
			return 50000f;
		default:
			return 0f;
		}
	}

	protected override void OnSpawn()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.electricalConduitSystem.AddToNetworks(cell, this, false);
		InitializeSwitchState();
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		GetComponent<KSelectable>().AddStatusItem(WireMaxWattageStatus, this);
		GetComponent<KSelectable>().AddStatusItem(WireCircuitStatus, this);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity(OutlineSymbol, false);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || (UnityEngine.Object)Grid.Objects[cell, (int)component.Def.ReplacementLayer] == (UnityEngine.Object)null)
		{
			Game.Instance.electricalConduitSystem.RemoveFromNetworks(cell, this, false);
		}
		Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
		base.OnCleanUp();
	}

	private void InitializeSwitchState()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		bool flag = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			CircuitSwitch component = gameObject.GetComponent<CircuitSwitch>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				flag = true;
				component.AttachWire(this);
			}
		}
		if (!flag)
		{
			Connect();
		}
	}

	public UtilityConnections GetWireConnections()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return Game.Instance.electricalConduitSystem.GetConnections(cell, true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = GetWireConnections();
		return Game.Instance.electricalConduitSystem.GetVisualizerString(wireConnections);
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		InitializeSwitchState();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (WireCircuitStatus == null)
		{
			WireCircuitStatus = new StatusItem("WireCircuitStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				Wire wire2 = (Wire)data;
				int cell = Grid.PosToCell(wire2.transform.GetPosition());
				CircuitManager circuitManager = Game.Instance.circuitManager;
				ushort circuitID = circuitManager.GetCircuitID(cell);
				float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
				float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
				GameUtil.WattageFormatterUnit unit2 = GameUtil.WattageFormatterUnit.Watts;
				if (wire2.MaxWattageRating == WattageRating.Max20000)
				{
					unit2 = GameUtil.WattageFormatterUnit.Kilowatts;
				}
				str = str.Replace("{CurrentLoad}", GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit2));
				str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(wattsNeededWhenActive, unit2));
				return str;
			});
		}
		if (WireMaxWattageStatus == null)
		{
			WireMaxWattageStatus = new StatusItem("WireMaxWattageStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				Wire wire = (Wire)data;
				GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
				if (wire.MaxWattageRating == WattageRating.Max20000)
				{
					unit = GameUtil.WattageFormatterUnit.Kilowatts;
				}
				str = str.Replace("{WireMaxWattage}", GameUtil.GetFormattedWattage(GetMaxWattageAsFloat(wire.MaxWattageRating), unit));
				return str;
			});
		}
	}

	public WattageRating GetMaxWattageRating()
	{
		return MaxWattageRating;
	}

	public bool IsDisconnected()
	{
		return disconnected;
	}

	public bool Connect()
	{
		BuildingHP component = GetComponent<BuildingHP>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null || component.HitPoints > 0)
		{
			disconnected = false;
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
			Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
		}
		return !disconnected;
	}

	public void Disconnect()
	{
		disconnected = true;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected, null);
		Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
	}

	public void SetFirstFrameCallback(System.Action ffCb)
	{
		firstFrameCallback = ffCb;
		StartCoroutine(RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell);
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}
}
