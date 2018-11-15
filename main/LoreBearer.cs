using STRINGS;
using System;
using UnityEngine;

public class LoreBearer : KMonoBehaviour
{
	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;

	private static readonly EventSystem.IntraObjectHandler<LoreBearer> RefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LoreBearer>(delegate(LoreBearer component, object data)
	{
		component.RefreshUserMenu(data);
	});

	public string content => Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".ENTRY");

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, RefreshUserMenuDelegate);
	}

	private void RefreshUserMenu(object data = null)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "action_follow_cam";
		string text = UI.USERMENUACTIONS.READLORE.NAME;
		System.Action on_click = OnClickRead;
		string tooltipText = UI.USERMENUACTIONS.READLORE.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
	}

	private Action<InfoDialogScreen> OpenCodex(string key)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			string entryForLock = CodexCache.GetEntryForLock(key);
			if (entryForLock == null)
			{
				KCrashReporter.Assert(false, "Missing codex entry: " + key);
			}
			else
			{
				ManagementMenu.Instance.OpenCodexToEntry(entryForLock);
			}
		};
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(base.gameObject.GetComponent<KSelectable>().GetProperName());
		if (BeenClicked)
		{
			infoDialogScreen.AddPlainText(BeenSearched);
		}
		else
		{
			BeenClicked = true;
			if (base.gameObject.name == "GeneShuffler")
			{
				Game.Instance.unlocks.Unlock("neuralvacillator");
			}
			if (base.gameObject.name == "PropDesk" || base.gameObject.name == "PropFacilityDesk" || base.gameObject.name == "PropReceptionDesk")
			{
				string text = Game.Instance.unlocks.UnlockNext("emails");
				if (text != null)
				{
					string str = "SEARCH" + UnityEngine.Random.Range(1, 6);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + str));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex(text));
				}
				else
				{
					string str2 = "SEARCH" + UnityEngine.Random.Range(1, 8);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + str2));
				}
			}
			else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
			{
				string text2 = Game.Instance.unlocks.UnlockNext("researchnotes");
				if (text2 != null)
				{
					string str3 = "SEARCH" + UnityEngine.Random.Range(1, 3);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + str3));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex(text2));
				}
				else
				{
					string str4 = "SEARCH1";
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str4));
				}
			}
			else if (base.gameObject.name == "HeadquartersComplete")
			{
				Game.Instance.unlocks.Unlock("pod_evacuation");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex("pod_evacuation"));
			}
			else if (base.gameObject.name == "PropFacilityDisplay")
			{
				Game.Instance.unlocks.Unlock("display_prop1");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex("display_prop1"));
			}
			else if (base.gameObject.name == "PropFacilityDisplay2")
			{
				Game.Instance.unlocks.Unlock("display_prop2");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex("display_prop2"));
			}
			else if (base.gameObject.name == "PropFacilityDisplay3")
			{
				Game.Instance.unlocks.Unlock("display_prop3");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex("display_prop3"));
			}
			else
			{
				string text3 = Game.Instance.unlocks.UnlockNext("journals");
				if (text3 != null)
				{
					string str5 = "SEARCH" + UnityEngine.Random.Range(1, 6);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + str5));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodex(text3));
				}
				else
				{
					string str6 = "SEARCH1";
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str6));
				}
			}
		}
	}
}
