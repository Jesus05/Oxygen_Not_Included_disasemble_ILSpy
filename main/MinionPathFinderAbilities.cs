using System.Diagnostics;
using UnityEngine;

public class MinionPathFinderAbilities : PathFinderAbilities
{
	private CellOffset[][] transitionVoidOffsets;

	public int maxUnderwaterCost;

	private int proxyID;

	private bool out_of_fuel = false;

	private bool idleNavMaskEnabled = false;

	public MinionPathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
		transitionVoidOffsets = new CellOffset[navigator.NavGrid.transitions.Length][];
		for (int i = 0; i < transitionVoidOffsets.Length; i++)
		{
			transitionVoidOffsets[i] = navigator.NavGrid.transitions[i].voidOffsets;
		}
	}

	protected override void Refresh(Navigator navigator)
	{
		proxyID = navigator.GetComponent<MinionIdentity>().assignableProxy.Get().GetComponent<KPrefabID>().InstanceID;
		maxUnderwaterCost = ((!PathFinder.IsSubmerged(Grid.PosToCell(navigator))) ? ((int)Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(navigator).GetTotalValue()) : 2147483647);
		out_of_fuel = navigator.HasTag(GameTags.JetSuitOutOfFuel);
	}

	public void SetIdleNavMaskEnabled(bool enabled)
	{
		idleNavMaskEnabled = enabled;
	}

	private static bool IsAccessPermitted(int proxyID, int cell, int from_cell)
	{
		if (Grid.HasAccessDoor[cell])
		{
			Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.NoLayer) - Grid.CellToPosCCC(from_cell, Grid.SceneLayer.NoLayer);
			Grid.Restriction.Directions directions = (Grid.Restriction.Directions)0;
			if (vector.x < 0f || vector.y > 0f)
			{
				directions |= Grid.Restriction.Directions.Left;
			}
			if (vector.x > 0f || vector.y < 0f)
			{
				directions |= Grid.Restriction.Directions.Right;
			}
			return Grid.HasPermission(cell, proxyID, directions);
		}
		return true;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (IsAccessPermitted(proxyID, path.cell, from_cell))
		{
			CellOffset[] array = transitionVoidOffsets[transition_id];
			foreach (CellOffset offset in array)
			{
				int cell = Grid.OffsetCell(from_cell, offset);
				if (!IsAccessPermitted(proxyID, cell, from_cell))
				{
					return false;
				}
			}
			if (path.navType == NavType.Tube && from_nav_type == NavType.Floor && !Grid.HasUsableTubeEntrance(from_cell, prefabInstanceID))
			{
				return false;
			}
			if (path.navType == NavType.Hover && (out_of_fuel || !path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack)))
			{
				return false;
			}
			Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
			PathFinder.PotentialPath.Flags pathFlags = PathFinder.PotentialPath.Flags.None;
			bool flag = path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks) && Grid.TryGetSuitMarkerFlags(from_cell, out flags, out pathFlags) && (flags & Grid.SuitMarker.Flags.Operational) != (Grid.SuitMarker.Flags)0;
			bool flag2 = SuitMarker.DoesTraversalDirectionRequireSuit(from_cell, path.cell, flags);
			bool flag3 = path.HasAnyFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack);
			if (flag)
			{
				bool flag4 = path.HasFlag(pathFlags);
				if (flag2)
				{
					if (!flag3 && !Grid.HasSuit(from_cell, prefabInstanceID))
					{
						return false;
					}
				}
				else if (flag3 && (flags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) != 0 && (!flag4 || !Grid.HasEmptyLocker(from_cell, prefabInstanceID)))
				{
					return false;
				}
			}
			if (idleNavMaskEnabled && (Grid.PreventIdleTraversal[path.cell] || Grid.PreventIdleTraversal[from_cell]))
			{
				return false;
			}
			if (path.HasFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit) || path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack) || path.navType == NavType.Tube || underwater_cost <= maxUnderwaterCost)
			{
				if (flag)
				{
					if (flag2)
					{
						if (!flag3)
						{
							path.SetFlags(pathFlags);
						}
					}
					else
					{
						path.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack);
					}
				}
				return true;
			}
			return false;
		}
		return false;
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void BeginSample(string region_name)
	{
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void EndSample()
	{
	}
}
