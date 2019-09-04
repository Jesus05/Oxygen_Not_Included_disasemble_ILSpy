using System.Collections.Generic;

public class LogicUtilityNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr, IBridgedNetworkItem
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.logicCircuitSystem.AddLink(cell1, cell2);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.logicCircuitSystem.RemoveLink(cell1, cell2);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.logicCircuitSystem;
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
