using Klei.AI;

public class CreaturePathFinderAbilities : PathFinderAbilities
{
	public int maxUnderwaterCost;

	public CreaturePathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Refresh()
	{
		int cell = Grid.PosToCell(base.navigator);
		if (PathFinder.IsSubmerged(cell))
		{
			maxUnderwaterCost = 2147483647;
		}
		else
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(base.navigator);
			maxUnderwaterCost = ((attributeInstance == null) ? 2147483647 : ((int)attributeInstance.GetTotalValue()));
		}
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (underwater_cost > maxUnderwaterCost)
		{
			return false;
		}
		return true;
	}
}
