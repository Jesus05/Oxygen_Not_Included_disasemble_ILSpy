public interface IGroupProber
{
	void SetProberCell(int cell, PathGrid pathGrid);

	void ProxyProberCell(int cell, bool set);

	bool ReleasePathGrid(PathGrid pathGrid);
}
