using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InterfaceTool : KMonoBehaviour
{
	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}

	public const float MaxClickDistance = 0.02f;

	public const float DepthBias = -0.15f;

	public GameObject visualizer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public string placeSound;

	protected bool populateHitsList;

	[NonSerialized]
	public bool hasFocus;

	[SerializeField]
	protected Texture2D cursor;

	public Vector2 cursorOffset = new Vector2(2f, 2f);

	public System.Action OnDeactivate;

	private static Texture2D activeCursor = null;

	private static HashedString toolActivatedViewMode = OverlayModes.None.ID;

	protected HashedString viewMode = OverlayModes.None.ID;

	private HoverTextConfiguration hoverTextConfiguration;

	private KSelectable hoverOverride;

	public KSelectable hover;

	protected int layerMask;

	protected SelectMarker selectMarker;

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;

	private List<KSelectable> hits = new List<KSelectable>();

	protected bool playedSoundThisFrame;

	private List<Intersection> intersections = new List<Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private int hitCycleCount;

	[CompilerGenerated]
	private static Predicate<Intersection> _003C_003Ef__mg_0024cache0;

	public HashedString ViewMode => viewMode;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		hoverTextConfiguration = GetComponent<HoverTextConfiguration>();
	}

	public void ActivateTool()
	{
		OnActivateTool();
		OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
		Game.Instance.Trigger(1174281782, this);
	}

	public virtual bool ShowHoverUI()
	{
		bool result = false;
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if ((UnityEngine.Object)current != (UnityEngine.Object)null)
		{
			Vector3 mousePos = KInputManager.GetMousePos();
			float x = mousePos.x;
			Vector3 mousePos2 = KInputManager.GetMousePos();
			Vector3 v = new Vector3(x, mousePos2.y, 0f);
			PointerEventData pointerEventData = new PointerEventData(current);
			pointerEventData.position = v;
			current.RaycastAll(pointerEventData, castResults);
			result = (castResults.Count == 0);
		}
		return result;
	}

	protected virtual void OnActivateTool()
	{
		if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && viewMode != OverlayModes.None.ID)
		{
			OverlayScreen.Instance.ToggleOverlay(viewMode, true);
			toolActivatedViewMode = viewMode;
		}
		SetCursor(cursor, cursorOffset, CursorMode.Auto);
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		OnDeactivateTool(new_tool);
		if (((UnityEngine.Object)new_tool == (UnityEngine.Object)null || (UnityEngine.Object)new_tool == (UnityEngine.Object)SelectTool.Instance) && toolActivatedViewMode != OverlayModes.None.ID && toolActivatedViewMode == SimDebugView.Instance.GetMode())
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, true);
			toolActivatedViewMode = OverlayModes.None.ID;
		}
	}

	public virtual void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = null;
	}

	protected virtual void OnDeactivateTool(InterfaceTool new_tool)
	{
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		isAppFocused = focusStatus;
	}

	public virtual string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	public virtual void OnMouseMove(Vector3 cursor_pos)
	{
		if (!((UnityEngine.Object)visualizer == (UnityEngine.Object)null) && isAppFocused)
		{
			int cell = Grid.PosToCell(cursor_pos);
			cursor_pos = Grid.CellToPosCBC(cell, visualizerLayer);
			cursor_pos.z += -0.15f;
			visualizer.transform.SetLocalPosition(cursor_pos);
		}
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
	}

	public virtual void OnLeftClickDown(Vector3 cursor_pos)
	{
	}

	public virtual void OnLeftClickUp(Vector3 cursor_pos)
	{
	}

	public virtual void OnRightClickDown(Vector3 cursor_pos, KButtonEvent e)
	{
	}

	public virtual void OnRightClickUp(Vector3 cursor_pos)
	{
	}

	public virtual void OnFocus(bool focus)
	{
		if ((UnityEngine.Object)visualizer != (UnityEngine.Object)null)
		{
			visualizer.SetActive(focus);
		}
		hasFocus = focus;
	}

	protected Vector2 GetRegularizedPos(Vector2 input, bool minimize)
	{
		Vector3 vector = new Vector3(Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, 0f);
		int cell = Grid.PosToCell(input);
		return Grid.CellToPosCCC(cell, Grid.SceneLayer.Background) + ((!minimize) ? vector : (-vector));
	}

	protected void SetCursor(Texture2D new_cursor, Vector2 offset, CursorMode mode)
	{
		if ((UnityEngine.Object)new_cursor != (UnityEngine.Object)activeCursor)
		{
			activeCursor = new_cursor;
			Cursor.SetCursor(new_cursor, offset, mode);
		}
	}

	protected void UpdateHoverElements(List<KSelectable> hits)
	{
		if ((UnityEngine.Object)hoverTextConfiguration != (UnityEngine.Object)null)
		{
			hoverTextConfiguration.UpdateHoverElements(hits);
		}
	}

	public virtual void LateUpdate()
	{
		if (populateHitsList)
		{
			if (isAppFocused)
			{
				int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
				if (Grid.IsValidCell(cell))
				{
					hits.Clear();
					GetSelectablesUnderCursor(hits);
					KSelectable objectUnderCursor = GetObjectUnderCursor(false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, null);
					UpdateHoverElements(hits);
					if (!hasFocus && (UnityEngine.Object)hoverOverride == (UnityEngine.Object)null)
					{
						ClearHover();
					}
					else if ((UnityEngine.Object)objectUnderCursor != (UnityEngine.Object)hover)
					{
						ClearHover();
						hover = objectUnderCursor;
						if ((UnityEngine.Object)objectUnderCursor != (UnityEngine.Object)null)
						{
							Game.Instance.Trigger(2095258329, objectUnderCursor.gameObject);
							objectUnderCursor.Hover(!playedSoundThisFrame);
							playedSoundThisFrame = true;
						}
					}
					playedSoundThisFrame = false;
				}
			}
		}
		else
		{
			UpdateHoverElements(null);
		}
	}

	public void GetSelectablesUnderCursor(List<KSelectable> hits)
	{
		if ((UnityEngine.Object)hoverOverride != (UnityEngine.Object)null)
		{
			hits.Add(hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 mousePos = KInputManager.GetMousePos();
		float x2 = mousePos.x;
		Vector3 mousePos2 = KInputManager.GetMousePos();
		float y2 = mousePos2.y;
		Vector3 position = main.transform.GetPosition();
		Vector3 position2 = new Vector3(x2, y2, 0f - position.z);
		Vector3 pos = main.ScreenToWorldPoint(position2);
		Vector2 pos2 = new Vector2(pos.x, pos.y);
		int cell = Grid.PosToCell(pos);
		if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
		{
			Game.Instance.statusItemRenderer.GetIntersections(pos2, hits);
			ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
			GameScenePartitioner.Instance.GatherEntries((int)pos2.x, (int)pos2.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
			pooledList.Sort((ScenePartitionerEntry x, ScenePartitionerEntry y) => SortHoverCards(x, y));
			foreach (ScenePartitionerEntry item in pooledList)
			{
				KCollider2D kCollider2D = item.obj as KCollider2D;
				if (!((UnityEngine.Object)kCollider2D == (UnityEngine.Object)null) && kCollider2D.Intersects(new Vector2(pos2.x, pos2.y)))
				{
					KSelectable kSelectable = kCollider2D.GetComponent<KSelectable>();
					if ((UnityEngine.Object)kSelectable == (UnityEngine.Object)null)
					{
						kSelectable = kCollider2D.GetComponentInParent<KSelectable>();
					}
					if (!((UnityEngine.Object)kSelectable == (UnityEngine.Object)null) && kSelectable.isActiveAndEnabled && !hits.Contains(kSelectable) && kSelectable.IsSelectable)
					{
						hits.Add(kSelectable);
					}
				}
			}
			pooledList.Recycle();
		}
	}

	public void SetLinkCursor(bool set)
	{
		SetCursor((!set) ? cursor : Assets.GetTexture("cursor_hand"), (!set) ? cursorOffset : Vector2.zero, CursorMode.Auto);
	}

	protected T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
	{
		intersections.Clear();
		GetObjectUnderCursor2D(intersections, condition, layerMask);
		intersections.RemoveAll((Predicate<Intersection>)is_component_null);
		if (intersections.Count <= 0)
		{
			prevIntersectionGroup.Clear();
			return (T)null;
		}
		curIntersectionGroup.Clear();
		foreach (Intersection intersection3 in intersections)
		{
			Intersection current = intersection3;
			curIntersectionGroup.Add((Component)current.component);
		}
		if (!prevIntersectionGroup.Equals(curIntersectionGroup))
		{
			hitCycleCount = 0;
			prevIntersectionGroup = curIntersectionGroup;
		}
		intersections.Sort((Comparison<Intersection>)((Intersection a, Intersection b) => SortSelectables(a.component as KMonoBehaviour, b.component as KMonoBehaviour)));
		int index = 0;
		if (cycleSelection)
		{
			index = hitCycleCount % intersections.Count;
			Intersection intersection = intersections[index];
			if ((UnityEngine.Object)intersection.component != (UnityEngine.Object)previous_selection || (UnityEngine.Object)previous_selection == (UnityEngine.Object)null)
			{
				index = 0;
				hitCycleCount = 0;
			}
			else
			{
				index = ++hitCycleCount % intersections.Count;
			}
		}
		Intersection intersection2 = intersections[index];
		return intersection2.component as T;
	}

	private void GetObjectUnderCursor2D<T>(List<Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 mousePos = KInputManager.GetMousePos();
		float x = mousePos.x;
		Vector3 mousePos2 = KInputManager.GetMousePos();
		float y = mousePos2.y;
		Vector3 position = main.transform.GetPosition();
		Vector3 position2 = new Vector3(x, y, 0f - position.z);
		Vector3 pos = main.ScreenToWorldPoint(position2);
		Vector2 pos2 = new Vector2(pos.x, pos.y);
		if ((UnityEngine.Object)hoverOverride != (UnityEngine.Object)null)
		{
			intersections.Add(new Intersection
			{
				component = hoverOverride,
				distance = -100f
			});
		}
		int cell = Grid.PosToCell(pos);
		if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
		{
			Game.Instance.statusItemRenderer.GetIntersections(pos2, intersections);
			ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
			int x2 = 0;
			int y2 = 0;
			Grid.CellToXY(cell, out x2, out y2);
			GameScenePartitioner.Instance.GatherEntries(x2, y2, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
			foreach (ScenePartitionerEntry item in (List<ScenePartitionerEntry>)pooledList)
			{
				KCollider2D kCollider2D = item.obj as KCollider2D;
				if (!((UnityEngine.Object)kCollider2D == (UnityEngine.Object)null) && kCollider2D.Intersects(new Vector2(pos.x, pos.y)))
				{
					T val = kCollider2D.GetComponent<T>();
					if ((UnityEngine.Object)val == (UnityEngine.Object)null)
					{
						val = kCollider2D.GetComponentInParent<T>();
					}
					if (!((UnityEngine.Object)val == (UnityEngine.Object)null) && ((1 << val.gameObject.layer) & layer_mask) != 0 && !((UnityEngine.Object)val == (UnityEngine.Object)null) && (condition == null || condition(val)))
					{
						Vector3 position3 = val.transform.GetPosition();
						float num = position3.z - pos.z;
						bool flag = false;
						for (int i = 0; i < intersections.Count; i++)
						{
							Intersection value = intersections[i];
							if ((UnityEngine.Object)value.component.gameObject == (UnityEngine.Object)val.gameObject)
							{
								value.distance = Mathf.Min(value.distance, num);
								intersections[i] = value;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							intersections.Add(new Intersection
							{
								component = val,
								distance = num
							});
						}
					}
				}
			}
			pooledList.Recycle();
		}
	}

	private int SortSelectables(KMonoBehaviour x, KMonoBehaviour y)
	{
		if ((UnityEngine.Object)x == (UnityEngine.Object)null && (UnityEngine.Object)y == (UnityEngine.Object)null)
		{
			return 0;
		}
		if ((UnityEngine.Object)x == (UnityEngine.Object)null)
		{
			return -1;
		}
		if ((UnityEngine.Object)y == (UnityEngine.Object)null)
		{
			return 1;
		}
		Vector3 position = x.transform.GetPosition();
		ref float z = ref position.z;
		Vector3 position2 = y.transform.GetPosition();
		int num = z.CompareTo(position2.z);
		return (num != 0) ? num : x.GetInstanceID().CompareTo(y.GetInstanceID());
	}

	public void SetHoverOverride(KSelectable hover_override)
	{
		hoverOverride = hover_override;
	}

	private int SortHoverCards(ScenePartitionerEntry x, ScenePartitionerEntry y)
	{
		KMonoBehaviour x2 = x.obj as KMonoBehaviour;
		KMonoBehaviour y2 = y.obj as KMonoBehaviour;
		return SortSelectables(x2, y2);
	}

	private static bool is_component_null(Intersection intersection)
	{
		return !(bool)intersection.component;
	}

	protected void ClearHover()
	{
		if ((UnityEngine.Object)hover != (UnityEngine.Object)null)
		{
			KSelectable kSelectable = hover;
			hover = null;
			kSelectable.Unhover();
			Game.Instance.Trigger(-1201923725, null);
		}
	}
}
