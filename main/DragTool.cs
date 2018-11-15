using FMOD.Studio;
using UnityEngine;

public class DragTool : InterfaceTool
{
	private enum DragAxis
	{
		Invalid = -1,
		None,
		Horizontal,
		Vertical
	}

	public enum Mode
	{
		Brush,
		Box
	}

	[SerializeField]
	private Texture2D boxCursor;

	[SerializeField]
	private GameObject areaVisualizer;

	[SerializeField]
	private Color32 areaColour = new Color(1f, 1f, 1f, 0.5f);

	protected SpriteRenderer areaVisualizerSpriteRenderer;

	protected Vector3 placementPivot;

	protected bool interceptNumberKeysForPriority;

	private static int defaultLayerMask;

	private bool dragging;

	private Vector3 previousCursorPos;

	private Mode mode = Mode.Box;

	private DragAxis dragAxis = DragAxis.Invalid;

	protected bool canChangeDragAxis = true;

	protected Vector3 downPos;

	protected static int layerMask;

	public bool Dragging => dragging;

	protected virtual Mode GetMode()
	{
		return mode;
	}

	public static void SetLayerMask(int mask)
	{
		layerMask = mask;
	}

	public static void ClearLayerMask()
	{
		layerMask = defaultLayerMask;
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		dragging = false;
		SetMode(mode);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		KScreenManager.Instance.SetEventSystemEnabled(true);
		base.OnDeactivateTool(new_tool);
	}

	protected override void OnPrefabInit()
	{
		Game.Instance.Subscribe(1634669191, OnTutorialOpened);
		defaultLayerMask = (1 | LayerMask.GetMask("World", "Pickupable", "Place", "PlaceWithDepth", "BlockSelection", "Construction"));
		layerMask = defaultLayerMask;
		base.OnPrefabInit();
		if ((Object)visualizer != (Object)null)
		{
			visualizer = Util.KInstantiate(visualizer, null, null);
		}
		if ((Object)areaVisualizer != (Object)null)
		{
			areaVisualizer = Util.KInstantiate(areaVisualizer, null, null);
			areaVisualizer.SetActive(false);
			areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<SpriteRenderer>();
			areaVisualizer.transform.SetParent(base.transform);
			Renderer component = areaVisualizer.GetComponent<Renderer>();
			component.material.color = areaColour;
		}
	}

	protected override void OnCmpEnable()
	{
		dragging = false;
	}

	protected override void OnCmpDisable()
	{
		if ((Object)visualizer != (Object)null)
		{
			visualizer.SetActive(false);
		}
		if ((Object)areaVisualizer != (Object)null)
		{
			areaVisualizer.SetActive(false);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		cursor_pos -= placementPivot;
		dragging = true;
		downPos = cursor_pos;
		previousCursorPos = cursor_pos;
		KScreenManager.Instance.SetEventSystemEnabled(false);
		switch (GetMode())
		{
		case Mode.Brush:
			if ((Object)visualizer != (Object)null)
			{
				AddDragPoint(cursor_pos);
			}
			break;
		case Mode.Box:
			if ((Object)visualizer != (Object)null)
			{
				visualizer.SetActive(false);
			}
			if ((Object)areaVisualizer != (Object)null)
			{
				areaVisualizer.SetActive(true);
				areaVisualizer.transform.SetPosition(cursor_pos);
				areaVisualizerSpriteRenderer.size = new Vector2(0.01f, 0.01f);
			}
			break;
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		cursor_pos -= placementPivot;
		KScreenManager.Instance.SetEventSystemEnabled(true);
		dragAxis = DragAxis.Invalid;
		if (dragging)
		{
			dragging = false;
			Mode mode = GetMode();
			if (mode == Mode.Box && (Object)areaVisualizer != (Object)null)
			{
				areaVisualizer.SetActive(false);
				Grid.PosToXY(downPos, out int x, out int y);
				int num = x;
				int num2 = y;
				Grid.PosToXY(cursor_pos, out int x2, out int y2);
				if (x2 < x)
				{
					Util.Swap(ref x, ref x2);
				}
				if (y2 < y)
				{
					Util.Swap(ref y, ref y2);
				}
				for (int i = y; i <= y2; i++)
				{
					for (int j = x; j <= x2; j++)
					{
						int cell = Grid.XYToCell(j, i);
						if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
						{
							int value = i - num2;
							int value2 = j - num;
							value = Mathf.Abs(value);
							value2 = Mathf.Abs(value2);
							OnDragTool(cell, value + value2);
						}
					}
				}
				string sound = GlobalAssets.GetSound(GetConfirmSound(), false);
				KMonoBehaviour.PlaySound(sound);
				OnDragComplete(downPos, cursor_pos);
			}
		}
	}

	protected virtual string GetConfirmSound()
	{
		return "Tile_Confirm";
	}

	protected virtual string GetDragSound()
	{
		return "Tile_Drag";
	}

	public override string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		if (dragging)
		{
			if (Input.GetKey((KeyCode)Global.Instance.GetInputManager().GetDefaultController().GetInputForAction(Action.DragStraight)))
			{
				Vector3 vector = cursorPos - downPos;
				if ((canChangeDragAxis || dragAxis == DragAxis.Invalid) && vector.sqrMagnitude > 0.707f)
				{
					if (Mathf.Abs(vector.x) < Mathf.Abs(vector.y))
					{
						dragAxis = DragAxis.Vertical;
					}
					else
					{
						dragAxis = DragAxis.Horizontal;
					}
				}
			}
			else
			{
				dragAxis = DragAxis.Invalid;
			}
			switch (dragAxis)
			{
			case DragAxis.Horizontal:
				cursorPos.y = downPos.y;
				break;
			case DragAxis.Vertical:
				cursorPos.x = downPos.x;
				break;
			}
		}
		base.OnMouseMove(cursorPos);
		if (dragging)
		{
			switch (GetMode())
			{
			case Mode.Brush:
				AddDragPoints(cursorPos, previousCursorPos);
				break;
			case Mode.Box:
			{
				Vector2 input = Vector3.Max(downPos, cursorPos);
				Vector2 input2 = Vector3.Min(downPos, cursorPos);
				input = GetRegularizedPos(input, false);
				input2 = GetRegularizedPos(input2, true);
				Vector2 vector2 = input - input2;
				Vector2 vector3 = (input + input2) * 0.5f;
				areaVisualizer.transform.SetPosition(new Vector2(vector3.x, vector3.y));
				if (areaVisualizerSpriteRenderer.size != vector2)
				{
					string sound = GlobalAssets.GetSound(GetDragSound(), false);
					if (sound != null)
					{
						int num = (int)(input.x - input2.x + (input.y - input2.y) - 1f);
						EventInstance instance = SoundEvent.BeginOneShot(sound, areaVisualizer.transform.GetPosition());
						instance.setParameterValue("tileCount", (float)num);
						SoundEvent.EndOneShot(instance);
					}
				}
				areaVisualizerSpriteRenderer.size = vector2;
				break;
			}
			}
			previousCursorPos = cursorPos;
		}
	}

	protected virtual void OnDragTool(int cell, int distFromOrigin)
	{
	}

	protected virtual void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
	{
	}

	private void AddDragPoint(Vector3 cursorPos)
	{
		int cell = Grid.PosToCell(cursorPos);
		if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
		{
			OnDragTool(cell, 0);
		}
	}

	private void AddDragPoints(Vector3 cursorPos, Vector3 previousCursorPos)
	{
		Vector3 a = cursorPos - previousCursorPos;
		float magnitude = a.magnitude;
		float num = Grid.CellSizeInMeters * 0.25f;
		int num2 = 1 + (int)(magnitude / num);
		a.Normalize();
		for (int i = 0; i < num2; i++)
		{
			Vector3 cursorPos2 = previousCursorPos + a * ((float)i * num);
			AddDragPoint(cursorPos2);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (interceptNumberKeysForPriority)
		{
			HandlePriortyKeysDown(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (interceptNumberKeysForPriority)
		{
			HandlePriorityKeysUp(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void HandlePriortyKeysDown(KButtonEvent e)
	{
		Action action = e.GetAction();
		if (Action.Plan1 <= action && action <= Action.Plan9 && e.TryConsume(action))
		{
			int num = (int)(action - 36 + 1);
			if (num <= 9)
			{
				ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, num), true);
				return;
			}
		}
		if (!e.Consumed)
		{
			e.TryConsume(Action.Plan10);
		}
	}

	private void HandlePriorityKeysUp(KButtonEvent e)
	{
		Action action = e.GetAction();
		if (Action.Plan1 <= action && action <= Action.Plan10)
		{
			e.TryConsume(action);
		}
	}

	protected void SetMode(Mode newMode)
	{
		mode = newMode;
		switch (mode)
		{
		case Mode.Brush:
			if ((Object)areaVisualizer != (Object)null)
			{
				areaVisualizer.SetActive(false);
			}
			if ((Object)visualizer != (Object)null)
			{
				visualizer.SetActive(true);
			}
			SetCursor(cursor, cursorOffset, CursorMode.Auto);
			break;
		case Mode.Box:
			if ((Object)visualizer != (Object)null)
			{
				visualizer.SetActive(true);
			}
			mode = Mode.Box;
			SetCursor(boxCursor, cursorOffset, CursorMode.Auto);
			break;
		}
	}

	public override void OnFocus(bool focus)
	{
		switch (GetMode())
		{
		case Mode.Brush:
			if ((Object)visualizer != (Object)null)
			{
				visualizer.SetActive(focus);
			}
			hasFocus = focus;
			break;
		case Mode.Box:
			if ((Object)visualizer != (Object)null && !dragging)
			{
				visualizer.SetActive(focus);
			}
			hasFocus = (focus || dragging);
			break;
		}
		base.OnFocus(focus);
	}

	private void OnTutorialOpened(object data)
	{
		dragging = false;
	}

	public override bool ShowHoverUI()
	{
		return dragging || base.ShowHoverUI();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}
}
