public class Lure : GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>
{
	public class Def : BaseDef
	{
		public CellOffset[] lurePoints = new CellOffset[1]
		{
			default(CellOffset)
		};

		public int radius = 50;

		public Tag[] initialLures;
	}

	public new class Instance : GameInstance
	{
		private Tag[] lures;

		public HandleVector<int>.Handle partitionerEntry;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			if (base.def.initialLures != null)
			{
				SetActiveLures(base.def.initialLures);
			}
		}

		public void SetActiveLures(Tag[] lures)
		{
			this.lures = lures;
			if (lures == null || lures.Length == 0)
			{
				GoTo(base.sm.off);
			}
			else
			{
				GoTo(base.sm.on);
			}
		}

		public bool IsActive()
		{
			return GetCurrentState() == base.sm.on;
		}

		public bool HasAnyLure(Tag[] creature_lures)
		{
			if (lures == null || creature_lures == null)
			{
				return false;
			}
			foreach (Tag a in creature_lures)
			{
				Tag[] array = lures;
				foreach (Tag b in array)
				{
					if (a == b)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public State off;

	public State on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.DoNothing();
		on.Enter(AddToScenePartitioner).Exit(RemoveFromScenePartitioner);
	}

	private void AddToScenePartitioner(Instance smi)
	{
		Extents extents = new Extents(Grid.PosToCell(smi.transform.GetPosition()), smi.def.radius);
		smi.partitionerEntry = GameScenePartitioner.Instance.Add(name, smi, extents, GameScenePartitioner.Instance.lure, null);
	}

	private void RemoveFromScenePartitioner(Instance smi)
	{
		GameScenePartitioner.Instance.Free(ref smi.partitionerEntry);
	}
}
