using FMOD.Studio;
using ProcGenGame;
using STRINGS;
using UnityEngine;

public class MinionSelectScreen : CharacterSelectionController
{
	[SerializeField]
	private NewBaseScreen newBasePrefab;

	[SerializeField]
	private WattsonMessage wattsonMessagePrefab;

	public const string WattsonGameObjName = "WattsonMessage";

	public KButton backButton;

	protected override void OnPrefabInit()
	{
		base.IsStarterMinion = true;
		base.OnPrefabInit();
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 2f, true);
		}
		GameObject parent = GameObject.Find("ScreenSpaceOverlayCanvas");
		GameObject gameObject = Util.KInstantiateUI(wattsonMessagePrefab.gameObject, parent, false);
		gameObject.name = "WattsonMessage";
		gameObject.SetActive(false);
		Game.Instance.Subscribe(-1992507039, OnBaseAlreadyCreated);
		backButton.onClick += delegate
		{
			LoadScreen.ForceStopGame();
			WorldGen.Reset();
			App.LoadScene("frontend");
		};
		InitializeContainers();
	}

	protected override void OnSpawn()
	{
		OnDeliverableAdded();
		EnableProceedButton();
		proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.EMBARK;
		containers.ForEach(delegate(ITelepadDeliverableContainer container)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if ((Object)characterContainer != (Object)null)
			{
				characterContainer.DisableSelectButton();
			}
		});
	}

	protected override void OnProceed()
	{
		Util.KInstantiateUI(newBasePrefab.gameObject, GameScreenManager.Instance.ssOverlayCanvas, false);
		MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot, STOP_MODE.ALLOWFADEOUT);
		selectedDeliverables.Clear();
		foreach (CharacterContainer container in containers)
		{
			selectedDeliverables.Add(container.Stats);
		}
		NewBaseScreen.Instance.SetStartingMinionStats(selectedDeliverables.ToArray());
		if (OnProceedEvent != null)
		{
			OnProceedEvent();
		}
		Game.Instance.Trigger(-838649377, null);
		BuildWatermark.Instance.gameObject.SetActive(false);
		Deactivate();
	}

	private void OnBaseAlreadyCreated(object data)
	{
		Game.Instance.StopFE();
		Game.Instance.StartBE();
		Game.Instance.SetGameStarted();
		Deactivate();
	}

	private void ReshuffleAll()
	{
		if (OnReshuffleEvent != null)
		{
			OnReshuffleEvent(base.IsStarterMinion);
		}
	}

	public override void OnPressBack()
	{
		foreach (ITelepadDeliverableContainer container in containers)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if ((Object)characterContainer != (Object)null)
			{
				characterContainer.ForceStopEditingTitle();
			}
		}
	}
}
