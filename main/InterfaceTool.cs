using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InterfaceTool : KMonoBehaviour
{
	public const float MaxClickDistance = 0.02f;

	public const float DepthBias = -0.15f;

	public GameObject visualizer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public string placeSound;

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

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;

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
			OverlayScreen.Instance.ToggleOverlay(viewMode);
			toolActivatedViewMode = viewMode;
		}
		SetCursor(cursor, cursorOffset, CursorMode.Auto);
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		OnDeactivateTool(new_tool);
		if (((UnityEngine.Object)new_tool == (UnityEngine.Object)null || (UnityEngine.Object)new_tool == (UnityEngine.Object)SelectTool.Instance) && toolActivatedViewMode != OverlayModes.None.ID && toolActivatedViewMode == SimDebugView.Instance.GetMode())
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
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
		UpdateHoverElements(null);
	}

	public void SetLinkCursor(bool set)
	{
		SetCursor((!set) ? cursor : Assets.GetTexture("cursor_hand"), (!set) ? cursorOffset : Vector2.zero, CursorMode.Auto);
	}
}
