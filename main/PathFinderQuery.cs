public class PathFinderQuery
{
	protected int resultCell;

	private NavType resultNavType;

	public virtual bool IsMatch(int cell, int parent_cell, int cost)
	{
		return true;
	}

	public void SetResult(int cell, int cost, NavType nav_type)
	{
		resultCell = cell;
		resultNavType = nav_type;
	}

	public void ClearResult()
	{
		resultCell = -1;
	}

	public virtual int GetResultCell()
	{
		return resultCell;
	}

	public NavType GetResultNavType()
	{
		return resultNavType;
	}
}
