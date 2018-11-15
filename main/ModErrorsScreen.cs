using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModErrorsScreen : KScreen
{
	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	public static void ShowErrors(ICollection<ModError> errors)
	{
		GameObject parent = GameObject.Find("Canvas");
		ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, parent, false);
		modErrorsScreen.Initialize(errors);
		modErrorsScreen.gameObject.SetActive(true);
	}

	private void Initialize(ICollection<ModError> errors)
	{
		foreach (ModError error2 in errors)
		{
			ModError error = error2;
			HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject, true);
			LocText reference = hierarchyReferences.GetReference<LocText>("Title");
			LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
			KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
			GetErrorText(error.errorType, out string title, out string title_tooltip);
			reference.text = title;
			reference.GetComponent<ToolTip>().toolTip = title_tooltip;
			switch (error.modInfo.source)
			{
			case ModInfo.Source.Steam:
			{
				ulong value = ulong.Parse(error.modInfo.assetID);
				PublishedFileId_t id = new PublishedFileId_t(value);
				SteamUGCService.Subscribed subscribed = SteamUGCService.Instance.GetSubscribed(id);
				reference2.text = subscribed.title;
				reference3.onClick += delegate
				{
					OpenDetailsPage(error.modInfo.assetID);
				};
				break;
			}
			case ModInfo.Source.Local:
				reference2.text = error.modInfo.assetID;
				reference3.onClick += delegate
				{
					Application.OpenURL("file://" + error.modInfo.assetID);
				};
				break;
			}
		}
	}

	private static void GetErrorText(ModError.ErrorType err_type, out string title, out string title_tooltip)
	{
		if (err_type != 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		title = UI.FRONTEND.MOD_ERRORS.MOD_REQUIRED;
		title_tooltip = UI.FRONTEND.MOD_ERRORS.TOOLTIPS.MOD_REQUIRED;
	}

	private static void OpenDetailsPage(string key)
	{
		Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + key);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		closeButtonTitle.onClick += Deactivate;
		closeButton.onClick += Deactivate;
	}
}
