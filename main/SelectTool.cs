using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SelectTool : InterfaceTool
{
	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}

	public KSelectable selected;

	public KSelectable hover;

	protected int cell_new;

	private int selectedCell;

	private KSelectable hoverOverride;

	public static SelectTool Instance;

	protected int defaultLayerMask;

	protected int layerMask;

	protected SelectMarker selectMarker;

	private bool appHasFocus = true;

	private List<KSelectable> hits = new List<KSelectable>();

	private int hitCycleCount;

	private List<Intersection> intersections = new List<Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	private KSelectable previousSelection;

	private bool playedSoundThisFrame;

	[CompilerGenerated]
	private static Predicate<Intersection> _003C_003Ef__mg_0024cache0;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		defaultLayerMask = (1 | LayerMask.GetMask("World", "Pickupable", "Place", "PlaceWithDepth", "BlockSelection", "Construction", "Selection"));
		layerMask = defaultLayerMask;
		selectMarker = Util.KInstantiateUI<SelectMarker>(EntityPrefabs.Instance.SelectMarker, GameScreenManager.Instance.worldSpaceCanvas, false);
		selectMarker.gameObject.SetActive(false);
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		ToolMenu.Instance.PriorityScreen.ResetPriority();
		Select(null, false);
	}

	public void SetLayerMask(int mask)
	{
		layerMask = mask;
		ClearHover();
		LateUpdate();
	}

	public void ClearLayerMask()
	{
		layerMask = defaultLayerMask;
	}

	public int GetDefaultLayerMask()
	{
		return defaultLayerMask;
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ClearHover();
		Select(null, false);
	}

	private void OnApplicationFocus(bool app_has_focus)
	{
		appHasFocus = app_has_focus;
	}

	public override void LateUpdate()
	{
		if (appHasFocus)
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
			pooledList.Sort(delegate(ScenePartitionerEntry x, ScenePartitionerEntry y)
			{
				Vector3 position3 = (x.obj as Transform).GetPosition();
				ref float z = ref position3.z;
				Vector3 position4 = (y.obj as Transform).GetPosition();
				return z.CompareTo(position4.z);
			});
			GameScenePartitioner.Instance.GatherEntries((int)pos2.x, (int)pos2.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
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

	private static bool is_component_null(Intersection intersection)
	{
		return !(bool)intersection.component;
	}

	private T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
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
		intersections.Sort((Comparison<Intersection>)((Intersection a, Intersection b) => (a.distance == b.distance) ? a.component.GetInstanceID().CompareTo(b.component.GetInstanceID()) : a.distance.CompareTo(b.distance)));
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

	private void ClearHover()
	{
		if ((UnityEngine.Object)hover != (UnityEngine.Object)null)
		{
			KSelectable kSelectable = hover;
			hover = null;
			kSelectable.Unhover();
			Game.Instance.Trigger(-1201923725, null);
		}
	}

	public void SetHoverOverride(KSelectable hover_override)
	{
		hoverOverride = hover_override;
	}

	public void Focus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		if ((UnityEngine.Object)selectable != (UnityEngine.Object)null)
		{
			pos = selectable.transform.GetPosition();
		}
		pos.z = -40f;
		pos += offset;
		CameraController.Instance.SetTargetPos(pos, 8f, true);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		Focus(pos, selectable, offset);
		Select(selectable, false);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable)
	{
		SelectAndFocus(pos, selectable, Vector3.zero);
	}

	public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
	{
		delayedNextSelection = new_selected;
		delayedSkipSound = skipSound;
		StartCoroutine(DoSelectNextFrame());
	}

	private IEnumerator DoSelectNextFrame()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void Select(KSelectable new_selected, bool skipSound = false)
	{
		if (!((UnityEngine.Object)new_selected == (UnityEngine.Object)previousSelection))
		{
			previousSelection = new_selected;
			if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
			{
				selected.Unselect();
			}
			GameObject gameObject = null;
			if ((UnityEngine.Object)new_selected != (UnityEngine.Object)null)
			{
				SelectToolHoverTextCard component = GetComponent<SelectToolHoverTextCard>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					int currentSelectedSelectableIndex = component.currentSelectedSelectableIndex;
					int recentNumberOfDisplayedSelectables = component.recentNumberOfDisplayedSelectables;
					if (recentNumberOfDisplayedSelectables != 0)
					{
						currentSelectedSelectableIndex = (currentSelectedSelectableIndex + 1) % recentNumberOfDisplayedSelectables;
						if (!skipSound)
						{
							if (recentNumberOfDisplayedSelectables == 1)
							{
								KFMOD.PlayOneShot(GlobalAssets.GetSound("Select_empty", false));
							}
							else
							{
								EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Select_full", false), Vector3.zero);
								instance.setParameterValue("selection", (float)currentSelectedSelectableIndex);
								SoundEvent.EndOneShot(instance);
							}
							playedSoundThisFrame = true;
						}
					}
				}
				if ((UnityEngine.Object)new_selected == (UnityEngine.Object)hover)
				{
					ClearHover();
				}
				new_selected.Select();
				gameObject = new_selected.gameObject;
				selectMarker.SetTargetTransform(gameObject.transform);
				selectMarker.gameObject.SetActive(!new_selected.DisableSelectMarker);
			}
			else if ((UnityEngine.Object)selectMarker != (UnityEngine.Object)null)
			{
				selectMarker.gameObject.SetActive(false);
			}
			selected = new_selected;
			Game.Instance.Trigger(-1503271301, gameObject);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		KSelectable objectUnderCursor = GetObjectUnderCursor(true, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, selected);
		selectedCell = Grid.PosToCell(cursor_pos);
		Select(objectUnderCursor, false);
	}

	public int GetSelectedCell()
	{
		return selectedCell;
	}
}
