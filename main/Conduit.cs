using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Conduit : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr, IBridgedNetworkItem, IDisconnectable, FlowUtilityNetwork.IItem
{
	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	[SerializeField]
	private bool disconnected = true;

	public ConduitType type;

	private System.Action firstFrameCallback;

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnHighlightedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnHighlighted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitFrozenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitFrozen(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitBoilingDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitBoiling(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnStructureTemperatureRegistered(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	public FlowUtilityNetwork Network
	{
		set
		{
		}
	}

	public int Cell => Grid.PosToCell(this);

	public Endpoint EndpointType => Endpoint.Conduit;

	public ConduitType ConduitType => ConduitType;

	public GameObject GameObject => base.gameObject;

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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1201923725, OnHighlightedDelegate);
		Subscribe(-700727624, OnConduitFrozenDelegate);
		Subscribe(-1152799878, OnConduitBoilingDelegate);
		Subscribe(-1555603773, OnStructureTemperatureRegisteredDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
	}

	private void OnStructureTemperatureRegistered(object data)
	{
		int cell = Grid.PosToCell(this);
		GetNetworkManager().AddToNetworks(cell, this, false);
		Connect();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		BuildingDef def = GetComponent<Building>().Def;
		if ((UnityEngine.Object)def != (UnityEngine.Object)null && def.ThermalConductivity != 1f)
		{
			ConduitFlowVisualizer flowVisualizer = GetFlowVisualizer();
			flowVisualizer.AddThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
		BuildingDef def = GetComponent<Building>().Def;
		if ((UnityEngine.Object)def != (UnityEngine.Object)null && def.ThermalConductivity != 1f)
		{
			ConduitFlowVisualizer flowVisualizer = GetFlowVisualizer();
			flowVisualizer.RemoveThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GetNetworkManager().RemoveFromNetworks(cell, this, false);
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || (UnityEngine.Object)Grid.Objects[cell, (int)component.Def.ReplacementLayer] == (UnityEngine.Object)null)
		{
			GetNetworkManager().RemoveFromNetworks(cell, this, false);
			GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
		}
		base.OnCleanUp();
	}

	private ConduitFlowVisualizer GetFlowVisualizer()
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidFlowVisualizer : Game.Instance.gasFlowVisualizer;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetFlowManager()
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static ConduitFlow GetFlowManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static IUtilityNetworkMgr GetNetworkManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}

	private void OnHighlighted(object data)
	{
		int highlightedCell = (!(bool)data) ? (-1) : Grid.PosToCell(base.transform.GetPosition());
		ConduitFlowVisualizer flowVisualizer = GetFlowVisualizer();
		flowVisualizer.SetHighlightedCell(highlightedCell);
	}

	private void OnConduitFrozen(object data)
	{
		Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = (string)BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
			popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE
		});
		GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnConduitBoiling(object data)
	{
		Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = (string)BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
			popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED
		});
		GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
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
			GetNetworkManager().ForceRebuildNetworks();
		}
		return !disconnected;
	}

	public void Disconnect()
	{
		disconnected = true;
		GetNetworkManager().ForceRebuildNetworks();
	}
}
