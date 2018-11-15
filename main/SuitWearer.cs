using System.Collections.Generic;
using UnityEngine;

public class SuitWearer : GameStateMachine<SuitWearer, SuitWearer.Instance>
{
	public new class Instance : GameInstance
	{
		private struct Reservation
		{
			public SuitMarker suitMarker;

			public bool isForEquipping;
		}

		private List<Reservation> reservations = new List<Reservation>();

		private Navigator navigator;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = master.GetComponent<Navigator>();
			navigator.SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
			KBatchedAnimController component = master.GetComponent<KBatchedAnimController>();
			component.SetSymbolVisiblity("snapto_neck", false);
		}

		public void OnPathAdvanced(object data)
		{
			if (navigator.CurrentNavType == NavType.Hover && (navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) == PathFinder.PotentialPath.Flags.None)
			{
				navigator.SetCurrentNavType(NavType.Floor);
			}
			UnreserveSuits();
			ReserveSuits();
		}

		public void ReserveSuits()
		{
			PathFinder.Path path = navigator.path;
			if (path.nodes != null)
			{
				bool flag = (navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != PathFinder.PotentialPath.Flags.None;
				bool flag2 = (navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) != PathFinder.PotentialPath.Flags.None;
				for (int i = 0; i < path.nodes.Count - 1; i++)
				{
					PathFinder.Path.Node node = path.nodes[i];
					int cell = node.cell;
					Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(cell);
					if (navigationFeature != null)
					{
						SuitMarker suitMarker = navigationFeature as SuitMarker;
						if (!((Object)suitMarker == (Object)null))
						{
							bool flag3 = (suitMarker.PathFlag & PathFinder.PotentialPath.Flags.HasAtmoSuit) != PathFinder.PotentialPath.Flags.None;
							bool flag4 = (suitMarker.PathFlag & PathFinder.PotentialPath.Flags.HasJetPack) != PathFinder.PotentialPath.Flags.None;
							bool flag5 = flag2 || flag;
							bool flag6 = flag3 == flag && flag4 == flag2;
							SuitMarker suitMarker2 = suitMarker;
							int source_cell = cell;
							PathFinder.Path.Node node2 = path.nodes[i + 1];
							bool flag7 = suitMarker2.DoesTraversalDirectionRequireSuit(source_cell, node2.cell);
							Reservation reservation;
							if (flag7 && !flag5)
							{
								reservation = default(Reservation);
								reservation.suitMarker = suitMarker;
								reservation.isForEquipping = flag7;
								Reservation item = reservation;
								suitMarker.Reserve(this, flag7);
								reservations.Add(item);
								if (flag3)
								{
									flag = true;
								}
								if (flag4)
								{
									flag2 = true;
								}
							}
							else if (!flag7 && flag6 && suitMarker.IsUnequipAvailableForSuitWearer(this))
							{
								reservation = default(Reservation);
								reservation.suitMarker = suitMarker;
								reservation.isForEquipping = flag7;
								Reservation item2 = reservation;
								suitMarker.Reserve(this, flag7);
								reservations.Add(item2);
								if (flag3)
								{
									flag = false;
								}
								if (flag4)
								{
									flag2 = false;
								}
							}
						}
					}
				}
			}
		}

		public void UnreserveSuits()
		{
			foreach (Reservation reservation in reservations)
			{
				Reservation current = reservation;
				if (!((Object)current.suitMarker == (Object)null))
				{
					current.suitMarker.Unreserve(this, current.isForEquipping);
				}
			}
			reservations.Clear();
		}
	}

	public State suit;

	public State nosuit;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventHandler(GameHashes.PathAdvanced, delegate(Instance smi, object data)
		{
			smi.OnPathAdvanced(data);
		}).DoNothing();
		suit.DoNothing();
		nosuit.DoNothing();
	}
}
