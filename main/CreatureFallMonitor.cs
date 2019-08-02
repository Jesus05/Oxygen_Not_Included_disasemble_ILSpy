using UnityEngine;

public class CreatureFallMonitor : GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>
{
	public class Def : BaseDef
	{
		public bool canSwim;
	}

	public new class Instance : GameInstance
	{
		public string anim = "fall";

		private Navigator navigator;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			navigator = master.GetComponent<Navigator>();
		}

		public void SnapToGround()
		{
			Vector3 position = base.smi.transform.GetPosition();
			Vector3 position2 = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Creatures);
			position2.x = position.x;
			base.smi.transform.SetPosition(position2);
			if (navigator.IsValidNavType(NavType.Floor))
			{
				navigator.SetCurrentNavType(NavType.Floor);
			}
			else if (navigator.IsValidNavType(NavType.Hover))
			{
				navigator.SetCurrentNavType(NavType.Hover);
			}
		}

		public bool ShouldFall()
		{
			if (base.gameObject.HasTag(GameTags.Stored))
			{
				return false;
			}
			Vector3 position = base.smi.transform.GetPosition();
			int num = Grid.PosToCell(position);
			if (Grid.IsValidCell(num) && Grid.Solid[num])
			{
				return false;
			}
			if (navigator.IsMoving())
			{
				return false;
			}
			if (CanSwimAtCurrentLocation(false))
			{
				return false;
			}
			if (navigator.CurrentNavType != NavType.Swim)
			{
				if (navigator.NavGrid.NavTable.IsValid(num, navigator.CurrentNavType))
				{
					return false;
				}
				if (navigator.CurrentNavType == NavType.Ceiling)
				{
					return true;
				}
				if (navigator.CurrentNavType == NavType.LeftWall)
				{
					return true;
				}
				if (navigator.CurrentNavType == NavType.RightWall)
				{
					return true;
				}
			}
			Vector3 pos = position;
			pos.y += FLOOR_DISTANCE;
			int num2 = Grid.PosToCell(pos);
			if (Grid.IsValidCell(num2) && Grid.Solid[num2])
			{
				return false;
			}
			return true;
		}

		public bool CanSwimAtCurrentLocation(bool check_head)
		{
			if (base.def.canSwim)
			{
				Vector3 position = base.transform.GetPosition();
				float num = 1f;
				if (!check_head)
				{
					num = 0.5f;
				}
				float y = position.y;
				Vector2 size = base.transform.GetComponent<KBoxCollider2D>().size;
				position.y = y + size.y * num;
				int cell = Grid.PosToCell(position);
				if (Grid.IsSubstantialLiquid(cell, 0.35f))
				{
					if (!GameComps.Gravities.Has(base.gameObject))
					{
						return true;
					}
					GravityComponent data = GameComps.Gravities.GetData(GameComps.Gravities.GetHandle(base.gameObject));
					if (data.velocity.magnitude < 2f)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public static float FLOOR_DISTANCE = -0.065f;

	public State grounded;

	public State falling;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grounded;
		grounded.ToggleBehaviour(GameTags.Creatures.Falling, (Instance smi) => smi.ShouldFall(), null);
	}
}
