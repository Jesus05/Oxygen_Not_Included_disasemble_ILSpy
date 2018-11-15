using System.Collections.Generic;

public interface IBridgedNetworkItem
{
	void AddNetworks(ICollection<UtilityNetwork> networks);

	bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks);

	int GetNetworkCell();
}
