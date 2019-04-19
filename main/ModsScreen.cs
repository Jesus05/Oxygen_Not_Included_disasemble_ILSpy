using KMod;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModsScreen : KModalScreen
{
	private struct DisplayedMod
	{
		public RectTransform rect_transform;

		public int mod_index;
	}

	private class ModOrderingDragListener : DragMe.IDragListener
	{
		private List<DisplayedMod> mods;

		private ModsScreen screen;

		private int startDragIdx = -1;

		public ModOrderingDragListener(ModsScreen screen, List<DisplayedMod> mods)
		{
			this.screen = screen;
			this.mods = mods;
		}

		public void OnBeginDrag(Vector2 pos)
		{
			startDragIdx = GetDragIdx(pos);
		}

		public void OnEndDrag(Vector2 pos)
		{
			if (startDragIdx >= 0)
			{
				int dragIdx = GetDragIdx(pos);
				int num;
				if (dragIdx >= 0 && dragIdx != startDragIdx)
				{
					DisplayedMod displayedMod = mods[dragIdx];
					num = displayedMod.mod_index;
				}
				else
				{
					num = Global.Instance.modManager.mods.Count;
				}
				int target_index = num;
				Manager modManager = Global.Instance.modManager;
				DisplayedMod displayedMod2 = mods[startDragIdx];
				modManager.Reinsert(displayedMod2.mod_index, target_index, this);
				screen.BuildDisplay();
			}
		}

		private int GetDragIdx(Vector2 pos)
		{
			int result = -1;
			for (int i = 0; i < mods.Count; i++)
			{
				DisplayedMod displayedMod = mods[i];
				if (RectTransformUtility.RectangleContainsScreenPoint(displayedMod.rect_transform, pos))
				{
					result = i;
					break;
				}
			}
			return result;
		}
	}

	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton workshopButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	private List<DisplayedMod> displayedMods = new List<DisplayedMod>();

	private List<Label> mod_footprint = new List<Label>();

	protected override void OnActivate()
	{
		base.OnActivate();
		closeButtonTitle.onClick += Exit;
		closeButton.onClick += Exit;
		System.Action value = delegate
		{
			Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140");
		};
		workshopButton.onClick += value;
		Global.Instance.modManager.Sanitize();
		mod_footprint.Clear();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.enabled)
			{
				mod_footprint.Add(mod.label);
				if ((mod.loaded_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) == (mod.available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)))
				{
					mod.Uncrash();
				}
			}
		}
		BuildDisplay();
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Combine(modManager.on_update, new Manager.OnUpdate(RebuildDisplay));
	}

	protected override void OnDeactivate()
	{
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Remove(modManager.on_update, new Manager.OnUpdate(RebuildDisplay));
		base.OnDeactivate();
	}

	private void Exit()
	{
		Global.Instance.modManager.Save();
		if (!Global.Instance.modManager.MatchFootprint(mod_footprint, Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			Global.Instance.modManager.RestartDialog(UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.TITLE, UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, Deactivate, true, base.gameObject, null);
		}
		else
		{
			Deactivate();
		}
		Global.Instance.modManager.events.Clear();
	}

	private void RebuildDisplay(object change_source)
	{
		if (!object.ReferenceEquals(change_source, this))
		{
			BuildDisplay();
		}
	}

	private void BuildDisplay()
	{
		foreach (DisplayedMod displayedMod in displayedMods)
		{
			DisplayedMod current = displayedMod;
			if ((UnityEngine.Object)current.rect_transform != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(current.rect_transform.gameObject);
			}
		}
		displayedMods.Clear();
		ModOrderingDragListener listener = new ModOrderingDragListener(this, displayedMods);
		for (int i = 0; i != Global.Instance.modManager.mods.Count; i++)
		{
			Mod mod = Global.Instance.modManager.mods[i];
			if (mod.status != 0 && mod.status != Mod.Status.UninstallPending && mod.HasAnyContent(Content.LayerableFiles | Content.Strings | Content.DLL))
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject, false);
				displayedMods.Add(new DisplayedMod
				{
					rect_transform = hierarchyReferences.gameObject.GetComponent<RectTransform>(),
					mod_index = i
				});
				DragMe component = hierarchyReferences.GetComponent<DragMe>();
				component.listener = listener;
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				reference.text = mod.title;
				reference.GetComponent<ToolTip>().toolTip = mod.description;
				if (mod.crash_count != 0)
				{
					reference.color = Color.Lerp(Color.white, Color.red, (float)mod.crash_count / 3f);
				}
				KButton reference2 = hierarchyReferences.GetReference<KButton>("ManageButton");
				reference2.isInteractable = mod.is_managed;
				if (reference2.isInteractable)
				{
					reference2.GetComponent<ToolTip>().toolTip = mod.manage_tooltip;
					reference2.onClick += mod.on_managed;
				}
				MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
				toggle.ChangeState(mod.enabled ? 1 : 0);
				MultiToggle multiToggle = toggle;
				multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
				{
					OnToggleClicked(toggle, mod.label);
				});
				toggle.GetComponent<ToolTip>().OnToolTip = (() => (!mod.enabled) ? UI.FRONTEND.MODS.TOOLTIPS.DISABLED : UI.FRONTEND.MODS.TOOLTIPS.ENABLED);
				hierarchyReferences.gameObject.SetActive(true);
			}
		}
		foreach (DisplayedMod displayedMod2 in displayedMods)
		{
			DisplayedMod current2 = displayedMod2;
			current2.rect_transform.gameObject.SetActive(true);
		}
		if (displayedMods.Count != 0)
		{
			return;
		}
	}

	private void OnToggleClicked(MultiToggle toggle, Label mod)
	{
		Manager modManager = Global.Instance.modManager;
		bool flag = modManager.IsModEnabled(mod);
		flag = !flag;
		toggle.ChangeState(flag ? 1 : 0);
		modManager.EnableMod(mod, flag, this);
	}
}
