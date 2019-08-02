using FMOD.Studio;
using System.Collections;
using UnityEngine;

public class SelectTool : InterfaceTool
{
	public KSelectable selected;

	protected int cell_new;

	private int selectedCell;

	public static SelectTool Instance;

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	private KSelectable previousSelection;

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
		populateHitsList = true;
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

	public void Focus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		if ((Object)selectable != (Object)null)
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
		if (!((Object)new_selected == (Object)previousSelection))
		{
			previousSelection = new_selected;
			if ((Object)selected != (Object)null)
			{
				selected.Unselect();
			}
			GameObject gameObject = null;
			if ((Object)new_selected != (Object)null)
			{
				SelectToolHoverTextCard component = GetComponent<SelectToolHoverTextCard>();
				if ((Object)component != (Object)null)
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
				if ((Object)new_selected == (Object)hover)
				{
					ClearHover();
				}
				new_selected.Select();
				gameObject = new_selected.gameObject;
				selectMarker.SetTargetTransform(gameObject.transform);
				selectMarker.gameObject.SetActive(!new_selected.DisableSelectMarker);
			}
			else if ((Object)selectMarker != (Object)null)
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
