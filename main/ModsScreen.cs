using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModsScreen : KModalScreen, SteamUGCService.IUGCEventHandler
{
	private class ModOrderingDragListener : DragMe.IDragListener
	{
		private List<RectTransform> rectTransforms;

		private ModsScreen screen;

		private int startDragIdx = -1;

		public ModOrderingDragListener(ModsScreen screen, List<RectTransform> displayed_items)
		{
			this.screen = screen;
			rectTransforms = displayed_items;
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
				if (dragIdx >= 0 && dragIdx != startDragIdx)
				{
					Global.Instance.modManager.Reorder(startDragIdx, dragIdx);
					screen.RebuildDisplay();
				}
			}
		}

		private int GetDragIdx(Vector2 pos)
		{
			int result = -1;
			for (int i = 0; i < rectTransforms.Count; i++)
			{
				if (RectTransformUtility.RectangleContainsScreenPoint(rectTransforms[i], pos))
				{
					result = i;
					break;
				}
			}
			return result;
		}
	}

	public const string TAG_MOD = "mod";

	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	private List<RectTransform> displayedMods = new List<RectTransform>();

	private bool triggerGameRestart;

	private static void OpenDetailsPage(string key)
	{
		Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + key);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		closeButtonTitle.onClick += Exit;
		closeButton.onClick += Exit;
		Global.Instance.modManager.UpdateSteamCodeModSubscriptions();
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Add(this);
		}
		RebuildDisplay();
	}

	protected override void OnDeactivate()
	{
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Remove(this);
		}
		base.OnDeactivate();
	}

	private void Exit()
	{
		if (triggerGameRestart)
		{
			Global.Instance.modManager.Save();
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(UI.FRONTEND.MODS.REQUIRES_RESTART.ToString(), App.instance.Restart, Deactivate, null, null, null, null, null, null);
		}
		else
		{
			Deactivate();
		}
	}

	private void RebuildDisplay()
	{
		foreach (RectTransform displayedMod in displayedMods)
		{
			if ((UnityEngine.Object)displayedMod != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(displayedMod.gameObject);
			}
		}
		displayedMods.Clear();
		ModOrderingDragListener listener = new ModOrderingDragListener(this, displayedMods);
		ICollection<ModInfo> installedMods = Global.Instance.modManager.GetInstalledMods();
		foreach (ModInfo item in installedMods)
		{
			ModInfo mod = item;
			bool flag = true;
			string text = null;
			string text2 = null;
			bool isInteractable = true;
			switch (mod.source)
			{
			case ModInfo.Source.Steam:
				flag = false;
				if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
				{
					ulong value = ulong.Parse(mod.assetID);
					PublishedFileId_t id = new PublishedFileId_t(value);
					SteamUGCService.Subscribed subscribed = SteamUGCService.Instance.GetSubscribed(id);
					if (subscribed != null)
					{
						flag = true;
						text = subscribed.title;
						text2 = subscribed.description;
					}
				}
				break;
			case ModInfo.Source.Local:
				flag = true;
				text = mod.description;
				isInteractable = false;
				break;
			}
			if (flag)
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject, false);
				displayedMods.Add(hierarchyReferences.gameObject.GetComponent<RectTransform>());
				DragMe component = hierarchyReferences.GetComponent<DragMe>();
				component.listener = listener;
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				reference.text = text;
				if (text2 != null)
				{
					reference.GetComponent<ToolTip>().toolTip = text2;
				}
				KButton reference2 = hierarchyReferences.GetReference<KButton>("ManageButton");
				reference2.isInteractable = isInteractable;
				reference2.GetComponent<ToolTip>().toolTip = UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION;
				reference2.onClick += delegate
				{
					Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + mod.assetID);
				};
				MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
				toggle.ChangeState(mod.enabled ? 1 : 0);
				MultiToggle multiToggle = toggle;
				multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
				{
					OnToggleClicked(toggle, mod);
				});
				toggle.GetComponent<ToolTip>().OnToolTip = (() => (!Global.Instance.modManager.IsModEnabled(mod)) ? UI.FRONTEND.MODS.TOOLTIPS.DISABLED : UI.FRONTEND.MODS.TOOLTIPS.ENABLED);
				hierarchyReferences.gameObject.SetActive(true);
			}
			else
			{
				Global.Instance.modManager.UninstallMod(mod);
			}
		}
		foreach (RectTransform displayedMod2 in displayedMods)
		{
			displayedMod2.gameObject.SetActive(true);
		}
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		string asset_id = pCallback.m_nPublishedFileId.m_PublishedFileId.ToString();
		ModInfo info = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.Mod, asset_id, "UNSUBSCRIBED", string.Empty, 0uL);
		Global.Instance.modManager.UninstallMod(info);
	}

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
	}

	public void OnUGCRefresh()
	{
		RebuildDisplay();
	}

	private void OnToggleClicked(MultiToggle toggle, ModInfo info)
	{
		ModManager modManager = Global.Instance.modManager;
		bool flag = modManager.IsModEnabled(info);
		flag = !flag;
		toggle.ChangeState(flag ? 1 : 0);
		if (flag)
		{
			modManager.EnableMod(info);
		}
		else
		{
			modManager.DisableMod(info);
		}
		triggerGameRestart = true;
	}
}
