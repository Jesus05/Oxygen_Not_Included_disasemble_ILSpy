using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class KSelectable : KMonoBehaviour
{
	private const float hoverHighlight = 0.25f;

	private const float selectHighlight = 0.2f;

	public string entityName;

	public string entityGender;

	private bool selected;

	[SerializeField]
	private bool selectable = true;

	[SerializeField]
	private bool disableSelectMarker;

	private StatusItemGroup statusItemGroup;

	public bool IsSelected => selected;

	public bool IsSelectable
	{
		get
		{
			return selectable && base.isActiveAndEnabled;
		}
		set
		{
			selectable = value;
		}
	}

	public bool DisableSelectMarker => disableSelectMarker;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemGroup = new StatusItemGroup(base.gameObject);
		KPrefabID component = GetComponent<KPrefabID>();
		if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
		{
			goto IL_002a;
		}
		goto IL_002a;
		IL_002a:
		if (entityName == null || entityName.Length <= 0)
		{
			SetName(base.name);
		}
		if (entityGender == null)
		{
			entityGender = "NB";
		}
	}

	public virtual string GetName()
	{
		if (entityName == null || entityName == string.Empty || entityName.Length <= 0)
		{
			Output.LogWithObj(base.gameObject, "Warning Item has blank name!");
			return base.name;
		}
		return entityName;
	}

	public void SetStatusIndicatorOffset(Vector3 offset)
	{
		if (statusItemGroup != null)
		{
			statusItemGroup.SetOffset(offset);
		}
	}

	public void SetName(string name)
	{
		entityName = name;
	}

	public void SetGender(string Gender)
	{
		entityGender = Gender;
	}

	public float GetZoom()
	{
		float num = 1f;
		Bounds bounds = Util.GetBounds(base.gameObject);
		Vector3 extents = bounds.extents;
		float x = extents.x;
		Vector3 extents2 = bounds.extents;
		return 1.05f * Mathf.Max(x, extents2.y);
	}

	public Vector3 GetPortraitLocation()
	{
		Vector3 vector = default(Vector3);
		return Util.GetBounds(base.gameObject).center;
	}

	private void ClearHighlight()
	{
		Trigger(-1201923725, false);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.HighlightColour = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void ApplyHighlight(float highlight)
	{
		Trigger(-1201923725, true);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.HighlightColour = new Color(highlight, highlight, highlight, highlight);
		}
	}

	public void Select()
	{
		selected = true;
		ClearHighlight();
		ApplyHighlight(0.2f);
		Trigger(-1503271301, true);
	}

	public void Unselect()
	{
		if (selected)
		{
			selected = false;
			ClearHighlight();
			Trigger(-1503271301, false);
		}
	}

	public void Hover(bool playAudio)
	{
		ClearHighlight();
		if (!DebugHandler.HideUI)
		{
			ApplyHighlight(0.25f);
		}
		if (playAudio)
		{
			PlayHoverSound();
		}
	}

	private void PlayHoverSound()
	{
		if (!((UnityEngine.Object)GetComponent<CellSelectionObject>() != (UnityEngine.Object)null))
		{
			UISounds.PlaySound(UISounds.Sound.Object_Mouseover);
		}
	}

	public void Unhover()
	{
		if (!selected)
		{
			ClearHighlight();
		}
	}

	public Guid ToggleStatusItem(StatusItem status_item, bool on, object data = null)
	{
		if (on)
		{
			return AddStatusItem(status_item, data);
		}
		return RemoveStatusItem(status_item, false);
	}

	public Guid ToggleStatusItem(StatusItem status_item, Guid guid, bool show, object data = null)
	{
		if (show)
		{
			if (guid != Guid.Empty)
			{
				return guid;
			}
			return AddStatusItem(status_item, data);
		}
		if (guid != Guid.Empty)
		{
			return RemoveStatusItem(guid, false);
		}
		return guid;
	}

	public Guid SetStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		return statusItemGroup.SetStatusItem(category, status_item, data);
	}

	public Guid ReplaceStatusItem(Guid guid, StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		if (guid != Guid.Empty)
		{
			statusItemGroup.RemoveStatusItem(guid, false);
		}
		return AddStatusItem(status_item, data);
	}

	public Guid AddStatusItem(StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		return statusItemGroup.AddStatusItem(status_item, data, null);
	}

	public Guid RemoveStatusItem(StatusItem status_item, bool immediate = false)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		statusItemGroup.RemoveStatusItem(status_item, immediate);
		return Guid.Empty;
	}

	public Guid RemoveStatusItem(Guid guid, bool immediate = false)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		statusItemGroup.RemoveStatusItem(guid, immediate);
		return Guid.Empty;
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		if (statusItemGroup == null)
		{
			return false;
		}
		return statusItemGroup.HasStatusItem(status_item);
	}

	public StatusItemGroup.Entry GetStatusItem(StatusItemCategory category)
	{
		return statusItemGroup.GetStatusItem(category);
	}

	public StatusItemGroup GetStatusItemGroup()
	{
		return statusItemGroup;
	}

	protected override void OnLoadLevel()
	{
		OnCleanUp();
		base.OnLoadLevel();
	}

	protected override void OnCleanUp()
	{
		if (statusItemGroup != null)
		{
			statusItemGroup.Destroy();
			statusItemGroup = null;
		}
		if (selected && (UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)this)
			{
				SelectTool.Instance.Select(null, true);
			}
			else
			{
				Unselect();
			}
		}
		base.OnCleanUp();
	}
}
