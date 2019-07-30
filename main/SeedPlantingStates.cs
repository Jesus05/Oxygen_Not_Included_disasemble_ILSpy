using STRINGS;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SeedPlantingStates : GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public PlantablePlot targetPlot;

		public int targetDirtPlotCell = Grid.InvalidCell;

		public Element plantElement = ElementLoader.FindElementByHash(SimHashes.Dirt);

		public Pickupable targetSeed;

		public int seed_cell = Grid.InvalidCell;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToPlantSeed);
		}
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	public State findSeed;

	public State moveToSeed;

	public State pickupSeed;

	public State findPlantLocation;

	public State moveToPlantLocation;

	public State moveToPlot;

	public State moveToDirt;

	public State planting;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static StateMachine<SeedPlantingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache7;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = findSeed;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.PLANTINGSEED.NAME, CREATURES.STATUSITEMS.PLANTINGSEED.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null).Exit(UnreserveSeed).Exit(DropAll)
			.Exit(RemoveMouthOverride);
		findSeed.Enter(delegate(Instance smi)
		{
			FindSeed(smi);
			if ((UnityEngine.Object)smi.targetSeed == (UnityEngine.Object)null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else
			{
				ReserveSeed(smi);
				smi.GoTo(moveToSeed);
			}
		});
		moveToSeed.MoveTo(GetSeedCell, findPlantLocation, behaviourcomplete, false);
		findPlantLocation.Enter(delegate(Instance smi)
		{
			if ((bool)smi.targetSeed)
			{
				FindDirtPlot(smi);
				if ((UnityEngine.Object)smi.targetPlot != (UnityEngine.Object)null || smi.targetDirtPlotCell != Grid.InvalidCell)
				{
					smi.GoTo(pickupSeed);
				}
				else
				{
					smi.GoTo(behaviourcomplete);
				}
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		pickupSeed.PlayAnim("gather").Enter(PickupComplete).OnAnimQueueComplete(moveToPlantLocation);
		moveToPlantLocation.Enter(delegate(Instance smi)
		{
			if ((UnityEngine.Object)smi.targetSeed == (UnityEngine.Object)null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else if ((UnityEngine.Object)smi.targetPlot != (UnityEngine.Object)null)
			{
				smi.GoTo(moveToPlot);
			}
			else if (smi.targetDirtPlotCell != Grid.InvalidCell)
			{
				smi.GoTo(moveToDirt);
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		moveToDirt.MoveTo((Instance smi) => smi.targetDirtPlotCell, planting, behaviourcomplete, false);
		moveToPlot.Enter(delegate(Instance smi)
		{
			if ((UnityEngine.Object)smi.targetPlot == (UnityEngine.Object)null || (UnityEngine.Object)smi.targetSeed == (UnityEngine.Object)null)
			{
				smi.GoTo(behaviourcomplete);
			}
		}).MoveTo(GetPlantableCell, planting, behaviourcomplete, false);
		planting.Enter(RemoveMouthOverride).PlayAnim("plant").Exit(PlantComplete)
			.OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToPlantSeed, false);
	}

	private static void AddMouthOverride(Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		KBatchedAnimController component2 = smi.GetComponent<KBatchedAnimController>();
		KAnim.Build.Symbol symbol = component2.AnimFiles[0].GetData().build.GetSymbol("sq_mouth_cheeks");
		if (symbol != null)
		{
			component.AddSymbolOverride("sq_mouth", symbol, 0);
		}
	}

	private static void RemoveMouthOverride(Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		component.TryRemoveSymbolOverride("sq_mouth", 0);
	}

	private static void PickupComplete(Instance smi)
	{
		if (!(bool)smi.targetSeed)
		{
			Debug.LogWarningFormat("PickupComplete seed {0} is null", smi.targetSeed);
		}
		else
		{
			UnreserveSeed(smi);
			int num = Grid.PosToCell(smi.targetSeed);
			if (smi.seed_cell != num)
			{
				Debug.LogWarningFormat("PickupComplete seed {0} moved {1} != {2}", smi.targetSeed, num, smi.seed_cell);
				smi.targetSeed = null;
			}
			else if (smi.targetSeed.HasTag(GameTags.Stored))
			{
				Debug.LogWarningFormat("PickupComplete seed {0} was stored by {1}", smi.targetSeed, smi.targetSeed.storage);
				smi.targetSeed = null;
			}
			else
			{
				smi.targetSeed = EntitySplitter.Split(smi.targetSeed, 1f, null);
				smi.GetComponent<Storage>().Store(smi.targetSeed.gameObject, false, false, true, false);
				AddMouthOverride(smi);
			}
		}
	}

	private static void PlantComplete(Instance smi)
	{
		PlantableSeed plantableSeed = (!(bool)smi.targetSeed) ? null : smi.targetSeed.GetComponent<PlantableSeed>();
		if ((bool)plantableSeed && CheckValidPlotCell(smi, plantableSeed, smi.targetDirtPlotCell, out PlantablePlot plot))
		{
			if ((bool)plot)
			{
				if ((UnityEngine.Object)plot.Occupant == (UnityEngine.Object)null)
				{
					plot.ForceDepositPickupable(smi.targetSeed);
				}
			}
			else
			{
				plantableSeed.TryPlant(true);
			}
		}
		smi.targetSeed = null;
		smi.seed_cell = Grid.InvalidCell;
		smi.targetPlot = null;
	}

	private static void DropAll(Instance smi)
	{
		smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
	}

	private static int GetPlantableCell(Instance smi)
	{
		int num = Grid.PosToCell(smi.targetPlot);
		if (Grid.IsValidCell(num))
		{
			return Grid.CellAbove(num);
		}
		return num;
	}

	private static void FindDirtPlot(Instance smi)
	{
		smi.targetDirtPlotCell = Grid.InvalidCell;
		PlantableSeed component = smi.targetSeed.GetComponent<PlantableSeed>();
		PlantableCellQuery plantableCellQuery = PathFinderQueries.plantableCellQuery.Reset(component, 20);
		Navigator component2 = smi.GetComponent<Navigator>();
		component2.RunQuery(plantableCellQuery);
		if (plantableCellQuery.result_cells.Count > 0)
		{
			smi.targetDirtPlotCell = plantableCellQuery.result_cells[UnityEngine.Random.Range(0, plantableCellQuery.result_cells.Count)];
		}
	}

	private static bool CheckValidPlotCell(Instance smi, PlantableSeed seed, int cell, out PlantablePlot plot)
	{
		plot = null;
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = (seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(cell) : Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.Solid[num])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[num, 1];
		if ((bool)gameObject)
		{
			plot = gameObject.GetComponent<PlantablePlot>();
			return (UnityEngine.Object)plot != (UnityEngine.Object)null;
		}
		return seed.TestSuitableGround(cell);
	}

	private static int GetSeedCell(Instance smi)
	{
		Debug.Assert(smi.targetSeed);
		Debug.Assert(smi.seed_cell != Grid.InvalidCell);
		return smi.seed_cell;
	}

	private static void FindSeed(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Pickupable targetSeed = null;
		int num = 100;
		IEnumerator enumerator = Components.Pickupables.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Pickupable pickupable = (Pickupable)enumerator.Current;
				if ((pickupable.HasTag(GameTags.Seed) || pickupable.HasTag(GameTags.CropSeed)) && !pickupable.HasTag(GameTags.Creatures.ReservedByCreature) && !(Vector2.Distance(smi.transform.position, pickupable.transform.position) > 25f))
				{
					int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable));
					if (navigationCost != -1 && navigationCost < num)
					{
						targetSeed = pickupable;
						num = navigationCost;
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		smi.targetSeed = targetSeed;
		smi.seed_cell = ((!(bool)smi.targetSeed) ? Grid.InvalidCell : Grid.PosToCell(smi.targetSeed));
	}

	private static void ReserveSeed(Instance smi)
	{
		GameObject gameObject = (!(bool)smi.targetSeed) ? null : smi.targetSeed.gameObject;
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveSeed(Instance smi)
	{
		GameObject go = (!(bool)smi.targetSeed) ? null : smi.targetSeed.gameObject;
		if ((UnityEngine.Object)smi.targetSeed != (UnityEngine.Object)null)
		{
			go.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}
}
