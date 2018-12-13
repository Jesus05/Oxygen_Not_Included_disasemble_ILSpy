using UnityEngine;

internal struct AccessControlNavMask
{
	private CellOffset[][] transitionVoidOffsets;

	public AccessControlNavMask(Navigator navigator)
	{
		transitionVoidOffsets = new CellOffset[navigator.NavGrid.transitions.Length][];
		for (int i = 0; i < transitionVoidOffsets.Length; i++)
		{
			transitionVoidOffsets[i] = navigator.NavGrid.transitions[i].voidOffsets;
		}
	}

	private bool CanTraverse(Navigator agent, int cell, int from_cell)
	{
		if (!Grid.HasAccessDoor[cell])
		{
			return true;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((Object)gameObject == (Object)null)
		{
			return true;
		}
		AccessControl component = gameObject.GetComponent<AccessControl>();
		if (!((Object)component == (Object)null))
		{
			AccessControl.Permission permission = component.GetPermission(agent.gameObject);
			switch (permission)
			{
			case AccessControl.Permission.Neither:
				return false;
			case AccessControl.Permission.Both:
				return true;
			default:
			{
				Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.NoLayer) - Grid.CellToPosCCC(from_cell, Grid.SceneLayer.NoLayer);
				Door component2 = component.GetComponent<Door>();
				if (component2.GetComponent<Rotatable>().IsRotated)
				{
					if ((permission == AccessControl.Permission.GoLeft && vector.y > 0f) || (permission == AccessControl.Permission.GoRight && vector.y < 0f))
					{
						return true;
					}
				}
				else if ((permission == AccessControl.Permission.GoLeft && vector.x < 0f) || (permission == AccessControl.Permission.GoRight && vector.x > 0f))
				{
					return true;
				}
				return false;
			}
			}
		}
		return true;
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id)
	{
		if (!CanTraverse(agent, path.cell, from_cell))
		{
			return false;
		}
		CellOffset[] array = transitionVoidOffsets[transition_id];
		foreach (CellOffset offset in array)
		{
			int cell = Grid.OffsetCell(from_cell, offset);
			if (!CanTraverse(agent, cell, from_cell))
			{
				return false;
			}
		}
		return true;
	}
}
