using System.Collections.Generic;
using UnityEngine;

public class WireUtilityNetworkLink : UtilityNetworkLink, IWattageRating, IHaveUtilityNetworkMgr, IUtilityNetworkItem, IBridgedNetworkItem
{
	[SerializeField]
	public Wire.WattageRating maxWattageRating;

	public ushort NetworkID
	{
		get
		{
			GetCells(out int linked_cell, out int _);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(linked_cell) as ElectricalUtilityNetwork;
			return (ushort)((electricalUtilityNetwork == null) ? 65535 : ((ushort)electricalUtilityNetwork.id));
		}
	}

	public Wire.WattageRating GetMaxWattageRating()
	{
		return maxWattageRating;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.RemoveLink(cell1, cell2);
		Game.Instance.circuitManager.Disconnect(this);
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.AddLink(cell1, cell2);
		Game.Instance.circuitManager.Connect(this);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		GetCells(out int linked_cell, out int _);
		IUtilityNetworkMgr networkManager = GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(linked_cell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		GetCells(out int linked_cell, out int _);
		IUtilityNetworkMgr networkManager = GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(linked_cell);
		return networks.Contains(networkForCell);
	}
}
