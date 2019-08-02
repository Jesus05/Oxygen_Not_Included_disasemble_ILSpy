using System.Collections.Generic;

public class SuitWearer : GameStateMachine<SuitWearer, SuitWearer.Instance>
{
	public new class Instance : GameInstance
	{
		private List<int> suitReservations = new List<int>();

		private List<int> emptyLockerReservations = new List<int>();

		private Navigator navigator;

		private int prefabInstanceID;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = master.GetComponent<Navigator>();
			navigator.SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
			prefabInstanceID = navigator.GetComponent<KPrefabID>().InstanceID;
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
					Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
					PathFinder.PotentialPath.Flags pathFlags = PathFinder.PotentialPath.Flags.None;
					if (Grid.TryGetSuitMarkerFlags(cell, out flags, out pathFlags))
					{
						bool flag3 = (pathFlags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != PathFinder.PotentialPath.Flags.None;
						bool flag4 = (pathFlags & PathFinder.PotentialPath.Flags.HasJetPack) != PathFinder.PotentialPath.Flags.None;
						bool flag5 = flag2 || flag;
						bool flag6 = flag3 == flag && flag4 == flag2;
						int source_cell = cell;
						PathFinder.Path.Node node2 = path.nodes[i + 1];
						bool flag7 = SuitMarker.DoesTraversalDirectionRequireSuit(source_cell, node2.cell, flags);
						if (flag7 && !flag5)
						{
							Grid.ReserveSuit(cell, prefabInstanceID, true);
							suitReservations.Add(cell);
							if (flag3)
							{
								flag = true;
							}
							if (flag4)
							{
								flag2 = true;
							}
						}
						else if (!flag7 && flag6 && Grid.HasEmptyLocker(cell, prefabInstanceID))
						{
							Grid.ReserveEmptyLocker(cell, prefabInstanceID, true);
							emptyLockerReservations.Add(cell);
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

		public void UnreserveSuits()
		{
			foreach (int suitReservation in suitReservations)
			{
				if (Grid.HasSuitMarker[suitReservation])
				{
					Grid.ReserveSuit(suitReservation, prefabInstanceID, false);
				}
			}
			suitReservations.Clear();
			foreach (int emptyLockerReservation in emptyLockerReservations)
			{
				if (Grid.HasSuitMarker[emptyLockerReservation])
				{
					Grid.ReserveEmptyLocker(emptyLockerReservation, prefabInstanceID, false);
				}
			}
			emptyLockerReservations.Clear();
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
