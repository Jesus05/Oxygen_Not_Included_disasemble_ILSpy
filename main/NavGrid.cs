using HUSL;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NavGrid
{
	public struct Link
	{
		public int link;

		public NavType startNavType;

		public NavType endNavType;

		private byte _transitionId;

		private byte _cost;

		public byte transitionId
		{
			get
			{
				return _transitionId;
			}
			set
			{
				_transitionId = value;
			}
		}

		public byte cost
		{
			get
			{
				return _cost;
			}
			set
			{
				_cost = value;
			}
		}

		public Link(int link, NavType start_nav_type, NavType end_nav_type, byte transition_id, byte cost)
		{
			_transitionId = 0;
			_cost = 0;
			this.link = link;
			startNavType = start_nav_type;
			endNavType = end_nav_type;
			transitionId = transition_id;
			this.cost = cost;
		}
	}

	public struct NavTypeData
	{
		public NavType navType;

		public Vector2 animControllerOffset;

		public bool flipX;

		public bool flipY;

		public float rotation;

		public HashedString idleAnim;
	}

	public struct Transition
	{
		public NavType start;

		public NavType end;

		public NavAxis startAxis;

		public sbyte x;

		public sbyte y;

		public byte id;

		public byte cost;

		public bool isLooping;

		public bool isEscape;

		public string preAnim;

		public string anim;

		public CellOffset[] voidOffsets;

		public CellOffset[] solidOffsets;

		public NavOffset[] validNavOffsets;

		public NavOffset[] invalidNavOffsets;

		public bool isCritter;

		public Transition(NavType start, NavType end, int x, int y, NavAxis start_axis, bool is_looping, bool loop_has_pre, bool is_escape, int cost, string anim, CellOffset[] void_offsets, CellOffset[] solid_offsets, NavOffset[] valid_nav_offsets, NavOffset[] invalid_nav_offsets, bool critter = false)
		{
			DebugUtil.Assert(x <= 127 && x >= -128);
			DebugUtil.Assert(y <= 127 && y >= -128);
			DebugUtil.Assert(cost <= 255 && cost >= 0);
			id = byte.MaxValue;
			this.start = start;
			this.end = end;
			this.x = (sbyte)x;
			this.y = (sbyte)y;
			startAxis = start_axis;
			isLooping = is_looping;
			isEscape = is_escape;
			this.anim = anim;
			preAnim = string.Empty;
			this.cost = (byte)cost;
			if (string.IsNullOrEmpty(this.anim))
			{
				this.anim = start.ToString().ToLower() + "_" + end.ToString().ToLower() + "_" + x + "_" + y;
			}
			if (isLooping)
			{
				if (loop_has_pre)
				{
					preAnim = this.anim + "_pre";
				}
				this.anim += "_loop";
			}
			if (startAxis != 0)
			{
				this.anim += ((startAxis != NavAxis.X) ? "_y" : "_x");
			}
			voidOffsets = void_offsets;
			solidOffsets = solid_offsets;
			validNavOffsets = valid_nav_offsets;
			invalidNavOffsets = invalid_nav_offsets;
			isCritter = critter;
		}

		public int IsValid(int cell, NavTable nav_table)
		{
			if (!Grid.IsCellOffsetValid(cell, x, y))
			{
				return Grid.InvalidCell;
			}
			int num = Grid.OffsetCell(cell, x, y);
			if (!nav_table.IsValid(num, end))
			{
				return Grid.InvalidCell;
			}
			Grid.BuildFlags buildFlags = Grid.BuildFlags.Solid | Grid.BuildFlags.DupeImpassable;
			if (isCritter)
			{
				buildFlags |= Grid.BuildFlags.CritterImpassable;
			}
			CellOffset[] array = voidOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				CellOffset cellOffset = array[i];
				int num2 = Grid.OffsetCell(cell, cellOffset.x, cellOffset.y);
				if (Grid.IsValidCell(num2) && (Grid.BuildMasks[num2] & buildFlags) != 0)
				{
					if (isCritter)
					{
						return Grid.InvalidCell;
					}
					if ((Grid.BuildMasks[num2] & Grid.BuildFlags.DupePassable) == (Grid.BuildFlags)0)
					{
						return Grid.InvalidCell;
					}
				}
			}
			CellOffset[] array2 = solidOffsets;
			for (int j = 0; j < array2.Length; j++)
			{
				CellOffset cellOffset2 = array2[j];
				int num3 = Grid.OffsetCell(cell, cellOffset2.x, cellOffset2.y);
				if (Grid.IsValidCell(num3) && !Grid.Solid[num3])
				{
					return Grid.InvalidCell;
				}
			}
			NavOffset[] array3 = validNavOffsets;
			for (int k = 0; k < array3.Length; k++)
			{
				NavOffset navOffset = array3[k];
				int cell2 = Grid.OffsetCell(cell, navOffset.offset.x, navOffset.offset.y);
				if (!nav_table.IsValid(cell2, navOffset.navType))
				{
					return Grid.InvalidCell;
				}
			}
			NavOffset[] array4 = invalidNavOffsets;
			for (int l = 0; l < array4.Length; l++)
			{
				NavOffset navOffset2 = array4[l];
				int cell3 = Grid.OffsetCell(cell, navOffset2.offset.x, navOffset2.offset.y);
				if (nav_table.IsValid(cell3, navOffset2.navType))
				{
					return Grid.InvalidCell;
				}
			}
			if (start == NavType.Tube)
			{
				if (end == NavType.Tube)
				{
					GameObject gameObject = Grid.Objects[cell, 9];
					GameObject gameObject2 = Grid.Objects[num, 9];
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = (!(bool)gameObject) ? null : gameObject.GetComponent<TravelTubeUtilityNetworkLink>();
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink2 = (!(bool)gameObject2) ? null : gameObject2.GetComponent<TravelTubeUtilityNetworkLink>();
					if ((bool)travelTubeUtilityNetworkLink)
					{
						travelTubeUtilityNetworkLink.GetCells(out int linked_cell, out int linked_cell2);
						if (num != linked_cell && num != linked_cell2)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, num);
						if (utilityConnections == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections connections = Game.Instance.travelTubeSystem.GetConnections(num, false);
						if (connections != utilityConnections)
						{
							return Grid.InvalidCell;
						}
					}
					else if ((bool)travelTubeUtilityNetworkLink2)
					{
						travelTubeUtilityNetworkLink2.GetCells(out int linked_cell3, out int linked_cell4);
						if (cell != linked_cell3 && cell != linked_cell4)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections2 = UtilityConnectionsExtensions.DirectionFromToCell(num, cell);
						if (utilityConnections2 == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections connections2 = Game.Instance.travelTubeSystem.GetConnections(cell, false);
						if (connections2 != utilityConnections2)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						bool flag = startAxis == NavAxis.X;
						int cell4 = cell;
						for (int m = 0; m < 2; m++)
						{
							if ((flag && m == 0) || (!flag && m == 1))
							{
								int num4 = (x > 0) ? 1 : (-1);
								for (int n = 0; n < Mathf.Abs(x); n++)
								{
									UtilityConnections connections3 = Game.Instance.travelTubeSystem.GetConnections(cell4, false);
									if (num4 > 0 && (connections3 & UtilityConnections.Right) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									if (num4 < 0 && (connections3 & UtilityConnections.Left) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									cell4 = Grid.OffsetCell(cell4, num4, 0);
								}
							}
							else
							{
								int num5 = (y > 0) ? 1 : (-1);
								for (int num6 = 0; num6 < Mathf.Abs(y); num6++)
								{
									UtilityConnections connections4 = Game.Instance.travelTubeSystem.GetConnections(cell4, false);
									if (num5 > 0 && (connections4 & UtilityConnections.Up) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									if (num5 < 0 && (connections4 & UtilityConnections.Down) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									cell4 = Grid.OffsetCell(cell4, 0, num5);
								}
							}
						}
					}
				}
				else
				{
					UtilityConnections connections5 = Game.Instance.travelTubeSystem.GetConnections(cell, false);
					if (y > 0)
					{
						if (connections5 != UtilityConnections.Down)
						{
							return Grid.InvalidCell;
						}
					}
					else if (x > 0)
					{
						if (connections5 != UtilityConnections.Left)
						{
							return Grid.InvalidCell;
						}
					}
					else if (x < 0)
					{
						if (connections5 != UtilityConnections.Right)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						if (y >= 0)
						{
							return Grid.InvalidCell;
						}
						if (connections5 != UtilityConnections.Up)
						{
							return Grid.InvalidCell;
						}
					}
				}
			}
			else if (start == NavType.Floor && end == NavType.Tube)
			{
				int cell5 = Grid.OffsetCell(cell, x, y);
				UtilityConnections connections6 = Game.Instance.travelTubeSystem.GetConnections(cell5, false);
				if (connections6 != UtilityConnections.Up)
				{
					return Grid.InvalidCell;
				}
			}
			return num;
		}
	}

	public bool DebugViewAllPaths;

	public bool DebugViewValidCells;

	public bool[] DebugViewValidCellsType;

	public bool DebugViewValidCellsAll;

	public bool DebugViewLinks;

	public bool[] DebugViewLinkType;

	public bool DebugViewLinksAll;

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public Link[] Links;

	private HashSet<int> DirtyCells = new HashSet<int>();

	private HashSet<int> ExpandedDirtyCells = new HashSet<int>();

	private NavTableValidator[] Validators = new NavTableValidator[0];

	private CellOffset[] boundingOffsets;

	public string id;

	public bool updateEveryFrame;

	public PathFinder.PotentialScratchPad potentialScratchPad;

	public Action<HashSet<int>> OnNavGridUpdateComplete;

	public NavType[] ValidNavTypes;

	public NavTypeData[] navTypeData;

	private Color[] debugColorLookup;

	public NavTable NavTable
	{
		get;
		private set;
	}

	public NavGraph NavGraph
	{
		get;
		private set;
	}

	public Transition[] transitions
	{
		get;
		set;
	}

	public Transition[][] transitionsByNavType
	{
		get;
		private set;
	}

	public int updateRangeX
	{
		get;
		private set;
	}

	public int updateRangeY
	{
		get;
		private set;
	}

	public int maxLinksPerCell
	{
		get;
		private set;
	}

	public NavGrid(string id, Transition[] transitions, NavTypeData[] nav_type_data, CellOffset[] bounding_offsets, NavTableValidator[] validators, int update_range_x, int update_range_y, int max_links_per_cell)
	{
		this.id = id;
		Validators = validators;
		navTypeData = nav_type_data;
		this.transitions = transitions;
		boundingOffsets = bounding_offsets;
		List<NavType> list = new List<NavType>();
		updateRangeX = update_range_x;
		updateRangeY = update_range_y;
		maxLinksPerCell = max_links_per_cell + 1;
		for (int i = 0; i < transitions.Length; i++)
		{
			DebugUtil.Assert(i >= 0 && i <= 255);
			transitions[i].id = (byte)i;
			if (!list.Contains(transitions[i].start))
			{
				list.Add(transitions[i].start);
			}
			if (!list.Contains(transitions[i].end))
			{
				list.Add(transitions[i].end);
			}
		}
		ValidNavTypes = list.ToArray();
		DebugViewLinkType = new bool[ValidNavTypes.Length];
		DebugViewValidCellsType = new bool[ValidNavTypes.Length];
		NavType[] validNavTypes = ValidNavTypes;
		foreach (NavType nav_type in validNavTypes)
		{
			GetNavTypeData(nav_type);
		}
		Links = new Link[maxLinksPerCell * Grid.CellCount];
		NavTable = new NavTable(Grid.CellCount);
		this.transitions = transitions;
		transitionsByNavType = new Transition[10][];
		for (int k = 0; k < 10; k++)
		{
			List<Transition> list2 = new List<Transition>();
			NavType navType = (NavType)k;
			for (int l = 0; l < transitions.Length; l++)
			{
				Transition item = transitions[l];
				if (item.start == navType)
				{
					list2.Add(item);
				}
			}
			transitionsByNavType[k] = list2.ToArray();
		}
		foreach (NavTableValidator navTableValidator in validators)
		{
			NavTableValidator navTableValidator2 = navTableValidator;
			navTableValidator2.onDirty = (Action<int>)Delegate.Combine(navTableValidator2.onDirty, new Action<int>(AddDirtyCell));
		}
		potentialScratchPad = new PathFinder.PotentialScratchPad(maxLinksPerCell);
		InitializeGraph();
		NavGraph = new NavGraph(Grid.CellCount, this);
	}

	public static NavType MirrorNavType(NavType nav_type)
	{
		switch (nav_type)
		{
		case NavType.LeftWall:
			return NavType.RightWall;
		case NavType.RightWall:
			return NavType.LeftWall;
		default:
			return nav_type;
		}
	}

	public NavTypeData GetNavTypeData(NavType nav_type)
	{
		NavTypeData[] array = navTypeData;
		for (int i = 0; i < array.Length; i++)
		{
			NavTypeData result = array[i];
			if (result.navType == nav_type)
			{
				return result;
			}
		}
		throw new Exception("Missing nav type data for nav type:" + nav_type.ToString());
	}

	public bool HasNavTypeData(NavType nav_type)
	{
		NavTypeData[] array = this.navTypeData;
		for (int i = 0; i < array.Length; i++)
		{
			NavTypeData navTypeData = array[i];
			if (navTypeData.navType == nav_type)
			{
				return true;
			}
		}
		return false;
	}

	public HashedString GetIdleAnim(NavType nav_type)
	{
		NavTypeData navTypeData = GetNavTypeData(nav_type);
		return navTypeData.idleAnim;
	}

	public void InitializeGraph()
	{
		NavGridUpdater.InitializeNavGrid(NavTable, ValidNavTypes, Validators, boundingOffsets, maxLinksPerCell, Links, transitionsByNavType);
	}

	public void UpdateGraph()
	{
		foreach (int dirtyCell in DirtyCells)
		{
			for (int i = -updateRangeY; i <= updateRangeY; i++)
			{
				for (int j = -updateRangeX; j <= updateRangeX; j++)
				{
					int num = Grid.OffsetCell(dirtyCell, j, i);
					if (Grid.IsValidCell(num))
					{
						ExpandedDirtyCells.Add(num);
					}
				}
			}
		}
		UpdateGraph(ExpandedDirtyCells);
		DirtyCells.Clear();
		ExpandedDirtyCells.Clear();
	}

	public void UpdateGraph(HashSet<int> dirty_nav_cells)
	{
		NavGridUpdater.UpdateNavGrid(NavTable, ValidNavTypes, Validators, boundingOffsets, maxLinksPerCell, Links, transitionsByNavType, dirty_nav_cells);
		if (OnNavGridUpdateComplete != null)
		{
			OnNavGridUpdateComplete(dirty_nav_cells);
		}
	}

	public static void DebugDrawPath(int start_cell, int end_cell)
	{
		Vector3 vector = Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
		Vector3 vector2 = Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
	}

	public static void DebugDrawPath(PathFinder.Path path)
	{
		if (path.nodes != null)
		{
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				PathFinder.Path.Node node = path.nodes[i];
				int cell = node.cell;
				PathFinder.Path.Node node2 = path.nodes[i + 1];
				DebugDrawPath(cell, node2.cell);
			}
		}
	}

	private void DebugDrawValidCells()
	{
		Color color = Color.white;
		int cellCount = Grid.CellCount;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				NavType nav_type = (NavType)j;
				if (NavTable.IsValid(i, nav_type) && DrawNavTypeCell(nav_type, ref color))
				{
					DebugExtension.DebugPoint(NavTypeHelper.GetNavPos(i, nav_type), color, 1f, 0f, false);
				}
			}
		}
	}

	private void DebugDrawLinks()
	{
		Color color = Color.white;
		for (int i = 0; i < Grid.CellCount; i++)
		{
			int num = i * maxLinksPerCell;
			for (int link = Links[num].link; link != InvalidCell; link = Links[num].link)
			{
				Vector3 navPos = NavTypeHelper.GetNavPos(i, Links[num].startNavType);
				if (DrawNavTypeLink(Links[num].startNavType, ref color) || DrawNavTypeLink(Links[num].endNavType, ref color))
				{
					Vector3 navPos2 = NavTypeHelper.GetNavPos(link, Links[num].endNavType);
				}
				num++;
			}
		}
	}

	private bool DrawNavTypeLink(NavType nav_type, ref Color color)
	{
		color = NavTypeColor(nav_type);
		if (DebugViewLinksAll)
		{
			return true;
		}
		for (int i = 0; i < ValidNavTypes.Length; i++)
		{
			if (ValidNavTypes[i] == nav_type)
			{
				return DebugViewLinkType[i];
			}
		}
		return false;
	}

	private bool DrawNavTypeCell(NavType nav_type, ref Color color)
	{
		color = NavTypeColor(nav_type);
		if (DebugViewValidCellsAll)
		{
			return true;
		}
		for (int i = 0; i < ValidNavTypes.Length; i++)
		{
			if (ValidNavTypes[i] == nav_type)
			{
				return DebugViewValidCellsType[i];
			}
		}
		return false;
	}

	public void DebugUpdate()
	{
		if (DebugViewValidCells)
		{
			DebugDrawValidCells();
		}
		if (DebugViewLinks)
		{
			DebugDrawLinks();
		}
	}

	public void AddDirtyCell(int cell)
	{
		DirtyCells.Add(cell);
	}

	public void Clear()
	{
		NavTableValidator[] validators = Validators;
		foreach (NavTableValidator navTableValidator in validators)
		{
			navTableValidator.Clear();
		}
	}

	private Color NavTypeColor(NavType navType)
	{
		if (debugColorLookup == null)
		{
			debugColorLookup = new Color[10];
			for (int i = 0; i < 10; i++)
			{
				double num = (double)i / 10.0;
				IList<double> list = ColorConverter.HUSLToRGB(new double[3]
				{
					num * 360.0,
					100.0,
					50.0
				});
				debugColorLookup[i] = new Color((float)list[0], (float)list[1], (float)list[2]);
			}
		}
		return debugColorLookup[(uint)navType];
	}
}
