public interface IUtilityItem
{
	UtilityConnections Connections
	{
		get;
		set;
	}

	void UpdateConnections(UtilityConnections Connections);

	int GetNetworkID();

	UtilityNetwork GetNetworkForDirection(Direction d);
}
