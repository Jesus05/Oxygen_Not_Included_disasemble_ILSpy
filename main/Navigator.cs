using STRINGS;
using System;
using System.IO;
using UnityEngine;

public class Navigator : StateMachineComponent<Navigator.StatesInstance>, ISaveLoadableDetails, ISim4000ms
{
	public class ActiveTransition
	{
		public int x;

		public int y;

		public bool isLooping;

		public NavType start;

		public NavType end;

		public HashedString preAnim;

		public HashedString anim;

		public float speed;

		public float animSpeed = 1f;

		public Func<bool> isCompleteCB;

		public NavGrid.Transition navGridTransition;

		public ActiveTransition(NavGrid.Transition transition, float default_speed)
		{
			x = transition.x;
			y = transition.y;
			isLooping = transition.isLooping;
			start = transition.start;
			end = transition.end;
			preAnim = transition.preAnim;
			anim = transition.anim;
			speed = default_speed;
			navGridTransition = transition;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Navigator, object>.GameInstance
	{
		public StatesInstance(Navigator master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Navigator>
	{
		public TargetParameter moveTarget;

		public State moving;

		public State arrived;

		public State failed;

		public State stopped;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = stopped;
			saveHistory = true;
			moving.Enter(delegate(StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
			}).Update("UpdateNavigator", delegate(StatesInstance smi, float dt)
			{
				smi.master.Sim33ms(dt);
			}, UpdateRate.SIM_33ms, true).Exit(delegate(StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementSleep);
			});
			arrived.TriggerOnEnter(GameHashes.DestinationReached, null).GoTo(stopped);
			failed.TriggerOnEnter(GameHashes.NavigationFailed, null).GoTo(stopped);
			stopped.DoNothing();
		}
	}

	public bool DebugDrawPath;

	[MyCmpAdd]
	public PathProber PathProber;

	[MyCmpAdd]
	private Facing facing;

	public float defaultSpeed = 1f;

	public TransitionDriver transitionDriver;

	public string NavGridName;

	public bool updateProber;

	public int maxProbingRadius;

	public PathFinder.PotentialPath.Flags flags;

	private LoggerFS log;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;

	private PathFinderAbilities abilities;

	[MyCmpReq]
	private KSelectable selectable;

	[NonSerialized]
	public PathFinder.Path path;

	public NavType CurrentNavType;

	private int AnchorCell;

	private KPrefabID targetLocator;

	private int reservedCell = NavigationReservations.InvalidReservation;

	private NavTactic tactic;

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnSelectObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnStoreDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnStore(data);
	});

	public KMonoBehaviour target
	{
		get;
		set;
	}

	public CellOffset[] targetOffsets
	{
		get;
		private set;
	}

	public NavGrid NavGrid
	{
		get;
		private set;
	}

	public void Serialize(BinaryWriter writer)
	{
		byte currentNavType = (byte)CurrentNavType;
		writer.Write(currentNavType);
	}

	public void Deserialize(IReader reader)
	{
		byte b = reader.ReadByte();
		NavType navType = (NavType)b;
		bool flag = false;
		NavType[] validNavTypes = NavGrid.ValidNavTypes;
		foreach (NavType navType2 in validNavTypes)
		{
			if (navType2 == navType)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			CurrentNavType = navType;
		}
	}

	protected override void OnPrefabInit()
	{
		transitionDriver = new TransitionDriver(this);
		targetLocator = Util.KInstantiate(Assets.GetPrefab(TargetLocator.ID), null, null).GetComponent<KPrefabID>();
		targetLocator.gameObject.SetActive(true);
		log = new LoggerFS("Navigator", 35);
		simRenderLoadBalance = true;
		autoRegisterSimRender = false;
		NavGrid = Pathfinding.Instance.GetNavGrid(NavGridName);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<PathProber>().SetValidNavTypes(NavGrid.ValidNavTypes, maxProbingRadius);
		Subscribe(1623392196, OnDefeatedDelegate);
		Subscribe(-1506500077, OnDefeatedDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1503271301, OnSelectObjectDelegate);
		Subscribe(856640610, OnStoreDelegate);
		if (updateProber)
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}
		SetCurrentNavType(CurrentNavType);
	}

	public bool IsMoving()
	{
		return base.smi.IsInsideState(base.smi.sm.moving);
	}

	public bool GoTo(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1]
			{
				default(CellOffset)
			};
		}
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return GoTo(targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
	}

	public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1]
			{
				default(CellOffset)
			};
		}
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return GoTo(targetLocator, offsets, tactic);
	}

	public void UpdateTarget(int cell)
	{
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
	}

	public bool GoTo(KMonoBehaviour target, CellOffset[] offsets, NavTactic tactic)
	{
		if (tactic == null)
		{
			tactic = NavigationTactics.ReduceTravelDistance;
		}
		base.smi.GoTo(base.smi.sm.moving);
		base.smi.sm.moveTarget.Set(target.gameObject, base.smi);
		this.tactic = tactic;
		this.target = target;
		targetOffsets = offsets;
		ClearReservedCell();
		AdvancePath(true);
		return IsMoving();
	}

	public void BeginTransition(NavGrid.Transition transition)
	{
		transitionDriver.EndTransition();
		base.smi.GoTo(base.smi.sm.moving);
		ActiveTransition transition2 = new ActiveTransition(transition, defaultSpeed);
		transitionDriver.BeginTransition(this, transition2);
	}

	private bool ValidatePath(ref PathFinder.Path path)
	{
		PathFinderAbilities currentAbilities = GetCurrentAbilities();
		return PathFinder.ValidatePath(NavGrid, currentAbilities, ref path);
	}

	public void AdvancePath(bool trigger_advance = true)
	{
		int num = Grid.PosToCell(this);
		int num2;
		bool flag;
		if ((UnityEngine.Object)target == (UnityEngine.Object)null)
		{
			Trigger(-766531887, null);
			Stop(false);
		}
		else
		{
			if (num != reservedCell || CurrentNavType == NavType.Tube)
			{
				flag = false;
				num2 = Grid.PosToCell(target);
				if (reservedCell == NavigationReservations.InvalidReservation)
				{
					flag = true;
				}
				else if (!CanReach(reservedCell))
				{
					flag = true;
				}
				else if (!Grid.IsCellOffsetOf(reservedCell, num2, targetOffsets))
				{
					flag = true;
				}
				else if (path.IsValid())
				{
					int num3 = num;
					PathFinder.Path.Node node = path.nodes[0];
					if (num3 == node.cell)
					{
						NavType currentNavType = CurrentNavType;
						PathFinder.Path.Node node2 = path.nodes[0];
						if (currentNavType == node2.navType)
						{
							flag = !ValidatePath(ref path);
							goto IL_01a4;
						}
					}
					int num4 = num;
					PathFinder.Path.Node node3 = path.nodes[1];
					if (num4 == node3.cell)
					{
						NavType currentNavType2 = CurrentNavType;
						PathFinder.Path.Node node4 = path.nodes[1];
						if (currentNavType2 == node4.navType)
						{
							path.nodes.RemoveAt(0);
							flag = !ValidatePath(ref path);
							goto IL_01a4;
						}
					}
					flag = true;
				}
				else
				{
					flag = true;
				}
				goto IL_01a4;
			}
			Stop(true);
		}
		goto IL_02a2;
		IL_01a4:
		if (flag)
		{
			int cellPreferences = tactic.GetCellPreferences(num2, targetOffsets, this);
			SetReservedCell(cellPreferences);
			if (reservedCell != NavigationReservations.InvalidReservation)
			{
				PathFinder.UpdatePath(potential_path: new PathFinder.PotentialPath(num, CurrentNavType, flags), nav_grid: NavGrid, abilities: GetCurrentAbilities(), query: PathFinderQueries.cellQuery.Reset(reservedCell), path: ref path);
			}
			else
			{
				Stop(false);
			}
		}
		if (path.IsValid())
		{
			NavGrid.Transition[] transitions = NavGrid.transitions;
			PathFinder.Path.Node node5 = path.nodes[1];
			BeginTransition(transitions[node5.transitionId]);
		}
		else if (path.HasArrived())
		{
			Stop(true);
		}
		else
		{
			ClearReservedCell();
			Stop(false);
		}
		goto IL_02a2;
		IL_02a2:
		if (trigger_advance)
		{
			Trigger(1347184327, null);
		}
	}

	public NavGrid.Transition GetNextTransition()
	{
		NavGrid.Transition[] transitions = NavGrid.transitions;
		PathFinder.Path.Node node = path.nodes[1];
		return transitions[node.transitionId];
	}

	public void Stop(bool arrived_at_destination = false)
	{
		target = null;
		targetOffsets = null;
		path.Clear();
		base.smi.sm.moveTarget.Set(null, base.smi);
		transitionDriver.EndTransition();
		HashedString idleAnim = NavGrid.GetIdleAnim(CurrentNavType);
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		component.Play(idleAnim, KAnim.PlayMode.Loop, 1f, 0f);
		if (arrived_at_destination)
		{
			base.smi.GoTo(base.smi.sm.arrived);
		}
		else if (base.smi.GetCurrentState() == base.smi.sm.moving)
		{
			ClearReservedCell();
			base.smi.GoTo(base.smi.sm.failed);
		}
	}

	private void Sim33ms(float dt)
	{
		if (IsMoving())
		{
			transitionDriver.UpdateTransition(dt);
		}
	}

	public void Sim4000ms(float dt)
	{
		UpdateProbe();
	}

	public void UpdateProbe()
	{
		int cell = Grid.PosToCell(this);
		if (Grid.IsValidCell(cell))
		{
			PathProber.UpdateProbe(NavGrid, cell, CurrentNavType, GetCurrentAbilities(), flags, true);
		}
	}

	public void DrawPath()
	{
		if (base.gameObject.activeInHierarchy && IsMoving())
		{
			NavPathDrawer.Instance.DrawPath(GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), path);
		}
	}

	private void OnDefeated(object data)
	{
		ClearReservedCell();
		Stop(false);
	}

	private void ClearReservedCell()
	{
		if (reservedCell != NavigationReservations.InvalidReservation)
		{
			NavigationReservations.Instance.RemoveOccupancy(reservedCell);
			reservedCell = NavigationReservations.InvalidReservation;
		}
	}

	private void SetReservedCell(int cell)
	{
		ClearReservedCell();
		reservedCell = cell;
		NavigationReservations.Instance.AddOccupancy(cell);
	}

	public int GetReservedCell()
	{
		return reservedCell;
	}

	public int GetAnchorCell()
	{
		return AnchorCell;
	}

	public bool IsValidNavType(NavType nav_type)
	{
		return NavGrid.HasNavTypeData(nav_type);
	}

	public void SetCurrentNavType(NavType nav_type)
	{
		CurrentNavType = nav_type;
		AnchorCell = NavTypeHelper.GetAnchorCell(nav_type, Grid.PosToCell(this));
		NavGrid.NavTypeData navTypeData = NavGrid.GetNavTypeData(CurrentNavType);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Vector2 one = Vector2.one;
		if (navTypeData.flipX)
		{
			one.x = -1f;
		}
		if (navTypeData.flipY)
		{
			one.y = -1f;
		}
		component.navMatrix = Matrix2x3.Translate(navTypeData.animControllerOffset * 200f) * Matrix2x3.Rotate(navTypeData.rotation) * Matrix2x3.Scale(one);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			object buttonInfo;
			string iconName;
			string text;
			System.Action on_click;
			string tooltipText;
			if ((UnityEngine.Object)NavPathDrawer.Instance.GetNavigator() != (UnityEngine.Object)this)
			{
				iconName = "action_navigable_regions";
				text = UI.USERMENUACTIONS.DRAWPATHS.NAME;
				on_click = OnDrawPaths;
				tooltipText = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				tooltipText = "action_navigable_regions";
				text = UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF;
				on_click = OnDrawPaths;
				iconName = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 0.1f);
			UserMenu userMenu = Game.Instance.userMenu;
			GameObject gameObject = base.gameObject;
			iconName = "action_follow_cam";
			text = UI.USERMENUACTIONS.FOLLOWCAM.NAME;
			on_click = OnFollowCam;
			tooltipText = UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP;
			userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 0.3f);
		}
	}

	private void OnFollowCam()
	{
		if ((UnityEngine.Object)CameraController.Instance.followTarget == (UnityEngine.Object)base.transform)
		{
			CameraController.Instance.ClearFollowTarget();
		}
		else
		{
			CameraController.Instance.SetFollowTarget(base.transform);
		}
	}

	private void OnDrawPaths()
	{
		if ((UnityEngine.Object)NavPathDrawer.Instance.GetNavigator() != (UnityEngine.Object)this)
		{
			NavPathDrawer.Instance.SetNavigator(this);
		}
		else
		{
			NavPathDrawer.Instance.ClearNavigator();
		}
	}

	private void OnSelectObject(object data)
	{
		NavPathDrawer.Instance.ClearNavigator();
	}

	public void OnStore(object data)
	{
		if (data is Storage || (data != null && (bool)data))
		{
			Stop(false);
		}
	}

	public PathFinderAbilities GetCurrentAbilities()
	{
		abilities.Refresh();
		return abilities;
	}

	public void SetAbilities(PathFinderAbilities abilities)
	{
		this.abilities = abilities;
	}

	public bool CanReach(IApproachable approachable)
	{
		return CanReach(approachable.GetCell(), approachable.GetOffsets());
	}

	public bool CanReach(int cell, CellOffset[] offsets)
	{
		foreach (CellOffset offset in offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			if (CanReach(cell2))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanReach(int cell)
	{
		return GetNavigationCost(cell) != -1;
	}

	public int GetNavigationCost(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return -1;
		}
		return PathProber.GetCost(cell);
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return PathProber.GetNavigationCostIgnoreProberOffset(cell, offsets);
	}

	public int GetNavigationCost(int cell, CellOffset[] offsets)
	{
		int num = -1;
		foreach (CellOffset offset in offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			int navigationCost = GetNavigationCost(cell2);
			if (num == -1)
			{
				num = navigationCost;
			}
			else if (navigationCost != -1 && navigationCost < num)
			{
				num = navigationCost;
			}
		}
		return num;
	}

	public int GetNavigationCost(IApproachable approachable)
	{
		return GetNavigationCost(approachable.GetCell(), approachable.GetOffsets());
	}

	public void RunQuery(PathFinderQuery query)
	{
		int cell = Grid.PosToCell(this);
		PathFinder.Run(potential_path: new PathFinder.PotentialPath(cell, CurrentNavType, flags), nav_grid: NavGrid, abilities: GetCurrentAbilities(), query: query);
	}

	public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		flags |= new_flags;
	}

	public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		flags &= (PathFinder.PotentialPath.Flags)(byte)(~(uint)new_flags);
	}
}
