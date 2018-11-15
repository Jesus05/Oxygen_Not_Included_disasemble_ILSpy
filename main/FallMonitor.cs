using UnityEngine;

public class FallMonitor : GameStateMachine<FallMonitor, FallMonitor.Instance>
{
	public class EntombedStates : State
	{
		public State recovering;

		public State stuck;
	}

	public new class Instance : GameInstance
	{
		private CellOffset[] entombedEscapeOffsets = new CellOffset[7]
		{
			new CellOffset(0, 1),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, -1),
			new CellOffset(-1, -1)
		};

		private Navigator navigator;

		private bool flipRecoverEmote;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = GetComponent<Navigator>();
			Pathfinding.Instance.FlushNavGridsOnLoad();
		}

		public void Recover()
		{
			int cell = Grid.PosToCell(navigator);
			NavGrid.Transition[] transitions = navigator.NavGrid.transitions;
			int num = 0;
			NavGrid.Transition transition;
			int num2;
			while (true)
			{
				if (num >= transitions.Length)
				{
					return;
				}
				transition = transitions[num];
				if (transition.isEscape && navigator.CurrentNavType == transition.start)
				{
					num2 = transition.IsValid(cell, navigator.NavGrid.NavTable, Grid.BitFields);
					if (Grid.InvalidCell != num2)
					{
						break;
					}
				}
				num++;
			}
			Vector2I vector2I = Grid.CellToXY(cell);
			Vector2I vector2I2 = Grid.CellToXY(num2);
			flipRecoverEmote = (vector2I2.x < vector2I.x);
			navigator.BeginTransition(transition);
		}

		public void RecoverEmote()
		{
			int num = Random.Range(0, 9);
			if (num == 8)
			{
				ChoreProvider component = base.master.GetComponent<ChoreProvider>();
				new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_floor_missing_kanim", new HashedString[1]
				{
					"react"
				}, KAnim.PlayMode.Once, flipRecoverEmote);
			}
		}

		public void LandFloor()
		{
			navigator.SetCurrentNavType(NavType.Floor);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public void AttemptInitialRecovery()
		{
			if (!base.gameObject.HasTag(GameTags.Incapacitated))
			{
				int cell = Grid.PosToCell(navigator);
				NavGrid.Transition[] transitions = navigator.NavGrid.transitions;
				int num = 0;
				while (true)
				{
					if (num >= transitions.Length)
					{
						return;
					}
					NavGrid.Transition transition = transitions[num];
					if (transition.isEscape && navigator.CurrentNavType == transition.start)
					{
						int num2 = transition.IsValid(cell, navigator.NavGrid.NavTable, Grid.BitFields);
						if (Grid.InvalidCell != num2)
						{
							break;
						}
					}
					num++;
				}
				base.smi.GoTo(base.smi.sm.recoverinitialfall);
			}
		}

		public bool CanRecoverToLadder()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			return navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !base.gameObject.HasTag(GameTags.Incapacitated);
		}

		public void MountLadder()
		{
			navigator.SetCurrentNavType(NavType.Ladder);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public bool CanRecoverToPole()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			return navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole) && !base.gameObject.HasTag(GameTags.Incapacitated);
		}

		public void MountPole()
		{
			navigator.SetCurrentNavType(NavType.Pole);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public bool IsFalling()
		{
			if (navigator.IsMoving())
			{
				return false;
			}
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			int cell2 = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(cell2))
			{
				return false;
			}
			bool flag = navigator.NavGrid.NavTable.IsValid(cell, navigator.CurrentNavType);
			return !flag;
		}

		public void UpdateFalling()
		{
			bool value = false;
			bool flag = false;
			if (!navigator.IsMoving())
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				bool flag2 = navigator.NavGrid.NavTable.IsValid(num, navigator.CurrentNavType) && (!base.gameObject.HasTag(GameTags.Incapacitated) || (navigator.CurrentNavType != NavType.Ladder && navigator.CurrentNavType != NavType.Pole));
				flag = (!flag2 && ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2])));
				value = (!flag2 && !flag);
			}
			base.sm.isFalling.Set(value, base.smi);
			base.sm.isEntombed.Set(flag, base.smi);
		}

		public bool IsCellSafe(int cell)
		{
			return navigator.NavGrid.NavTable.IsValid(cell, navigator.CurrentNavType);
		}

		public void TryEntombedEscape()
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			CellOffset[] array = entombedEscapeOffsets;
			foreach (CellOffset offset in array)
			{
				int cell2 = Grid.OffsetCell(cell, offset);
				if (IsCellSafe(cell2))
				{
					base.transform.SetPosition(Grid.CellToPosCBC(cell2, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop(false);
					base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					UpdateFalling();
					GoTo(base.sm.standing);
					return;
				}
			}
			CellOffset[] array2 = entombedEscapeOffsets;
			foreach (CellOffset offset2 in array2)
			{
				int num = Grid.OffsetCell(cell, offset2);
				int num2 = Grid.CellAbove(num);
				if (Grid.IsValidCell(num) && Grid.IsValidCell(num2) && !Grid.Solid[num] && !Grid.Solid[num2])
				{
					base.transform.SetPosition(Grid.CellToPosCBC(num, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop(false);
					base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					UpdateFalling();
					GoTo(base.sm.standing);
					return;
				}
			}
			GoTo(base.sm.entombed.stuck);
		}
	}

	public State standing;

	public State falling_pre;

	public State falling;

	public EntombedStates entombed;

	public State recoverladder;

	public State recoverpole;

	public State recoverinitialfall;

	public State landfloor;

	public State instorage;

	public BoolParameter isEntombed;

	public BoolParameter isFalling;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = standing;
		root.EventTransition(GameHashes.OnStore, instorage, null).Update("CheckLanded", delegate(Instance smi, float dt)
		{
			smi.UpdateFalling();
		}, UpdateRate.SIM_33ms, true);
		standing.ParamTransition(isEntombed, entombed, (Instance smi, bool p) => p).ParamTransition(isFalling, falling_pre, (Instance smi, bool p) => p);
		falling_pre.Enter("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		}).Enter("AttemptInitialRecovery", delegate(Instance smi)
		{
			smi.AttemptInitialRecovery();
		}).GoTo(falling)
			.ToggleBrain("falling_pre");
		falling.ToggleBrain("falling").PlayAnim("fall_pre").QueueAnim("fall_loop", true, null)
			.ParamTransition(isEntombed, entombed, (Instance smi, bool p) => p)
			.Transition(recoverladder, (Instance smi) => smi.CanRecoverToLadder(), UpdateRate.SIM_33ms)
			.Transition(recoverpole, (Instance smi) => smi.CanRecoverToPole(), UpdateRate.SIM_33ms)
			.ToggleGravity(landfloor);
		recoverinitialfall.ToggleBrain("recoverinitialfall").Enter("Recover", delegate(Instance smi)
		{
			smi.Recover();
		}).EventTransition(GameHashes.DestinationReached, standing, null)
			.EventTransition(GameHashes.NavigationFailed, standing, null)
			.Exit(delegate(Instance smi)
			{
				smi.RecoverEmote();
			});
		landfloor.Enter("Land", delegate(Instance smi)
		{
			smi.LandFloor();
		}).GoTo(standing);
		recoverladder.ToggleBrain("recoverladder").PlayAnim("floor_ladder_0_0").Enter("MountLadder", delegate(Instance smi)
		{
			smi.MountLadder();
		})
			.OnAnimQueueComplete(standing);
		recoverpole.ToggleBrain("recoverpole").PlayAnim("floor_pole_0_0").Enter("MountPole", delegate(Instance smi)
		{
			smi.MountPole();
		})
			.OnAnimQueueComplete(standing);
		instorage.EventTransition(GameHashes.OnStore, standing, null);
		entombed.DefaultState(entombed.recovering);
		entombed.recovering.Enter("TryEntombedEscape", delegate(Instance smi)
		{
			smi.TryEntombedEscape();
		});
		entombed.stuck.Enter("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		}).ToggleChore((Instance smi) => new EntombedChore(smi.master), standing).ParamTransition(isEntombed, standing, (Instance smi, bool p) => !p);
	}
}
