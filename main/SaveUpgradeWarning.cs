using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveUpgradeWarning : KMonoBehaviour
{
	private struct Upgrade
	{
		public int major;

		public int minor;

		public System.Action action;

		public Upgrade(int major, int minor, System.Action action)
		{
			this.major = major;
			this.minor = minor;
			this.action = action;
		}
	}

	[MyCmpReq]
	private Game game;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game obj = game;
		obj.OnLoad = (Action<Game.GameSaveData>)Delegate.Combine(obj.OnLoad, new Action<Game.GameSaveData>(OnLoad));
	}

	protected override void OnCleanUp()
	{
		Game obj = game;
		obj.OnLoad = (Action<Game.GameSaveData>)Delegate.Remove(obj.OnLoad, new Action<Game.GameSaveData>(OnLoad));
		base.OnCleanUp();
	}

	private void OnLoad(Game.GameSaveData data)
	{
		List<Upgrade> list = new List<Upgrade>();
		list.Add(new Upgrade(7, 5, SuddenMoraleHelper));
		List<Upgrade> list2 = list;
		foreach (Upgrade item in list2)
		{
			Upgrade current = item;
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(current.major, current.minor))
			{
				current.action();
			}
		}
	}

	private void SuddenMoraleHelper()
	{
		Effect morale_effect = Db.Get().effects.Get("SuddenMoraleHelper");
		CustomizableDialogScreen screen = Util.KInstantiateUI<CustomizableDialogScreen>(ScreenPrefabs.Instance.CustomizableDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_BUFF, delegate
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				Effects component = item.GetComponent<Effects>();
				component.Add(morale_effect, true);
			}
			screen.Deactivate();
		});
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_DISABLE, delegate
		{
			SettingConfig morale = CustomGameSettingConfigs.Morale;
			CustomGameSettings.Instance.customGameMode = CustomGameSettings.CustomGameMode.Custom;
			CustomGameSettings.Instance.SetQualitySetting(morale, morale.GetLevel("Disabled").id);
			screen.Deactivate();
		});
		screen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER, Mathf.RoundToInt(morale_effect.duration / 600f)), UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_TITLE, null);
	}
}
