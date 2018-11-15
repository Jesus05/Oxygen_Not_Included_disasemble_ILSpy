public class TravelTubeUtilityNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.travelTubeSystem.AddLink(cell1, cell2);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.travelTubeSystem.RemoveLink(cell1, cell2);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.travelTubeSystem;
	}
}
