using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class ImmigrantScreen : CharacterSelectionController
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton rejectButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private GameObject rejectConfirmationScreen;

	[SerializeField]
	private KButton confirmRejectionBtn;

	[SerializeField]
	private KButton cancelRejectionBtn;

	private static ImmigrantScreen instance;

	private Telepad telepad;

	private bool hasShown;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = false;
		base.OnSpawn();
		base.IsStarterMinion = false;
		rejectButton.onClick += OnRejectAll;
		confirmRejectionBtn.onClick += OnRejectionConfirmed;
		cancelRejectionBtn.onClick += OnRejectionCancelled;
		instance = this;
		title.text = UI.IMMIGRANTSCREEN.IMMIGRANTSCREENTITLE;
		proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.PROCEEDBUTTON;
		closeButton.onClick += delegate
		{
			Show(false);
		};
		Show(false);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Dialog_Popup", false));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
			MusicManager.instance.PlaySong("Music_SelectDuplicant", false);
			hasShown = true;
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
			}
			if (Immigration.Instance.ImmigrantsAvailable && hasShown)
			{
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
			}
		}
		base.OnShow(show);
	}

	public override void OnPressBack()
	{
		if (rejectConfirmationScreen.activeSelf)
		{
			OnRejectionCancelled();
		}
		else
		{
			base.OnPressBack();
		}
	}

	public override void Deactivate()
	{
		Show(false);
	}

	public static void InitializeImmigrantScreen(Telepad telepad)
	{
		instance.Initialize(telepad);
		instance.Show(true);
	}

	private void Initialize(Telepad telepad)
	{
		InitializeContainers();
		foreach (ITelepadDeliverableContainer container in containers)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if ((Object)characterContainer != (Object)null)
			{
				characterContainer.SetReshufflingState(false);
			}
		}
		this.telepad = telepad;
	}

	protected override void OnProceed()
	{
		telepad.OnAcceptDelivery(selectedDeliverables[0]);
		Show(false);
		containers.ForEach(delegate(ITelepadDeliverableContainer cc)
		{
			Object.Destroy(cc.GetGameObject());
		});
		containers.Clear();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.PlaySong("Stinger_NewDuplicant", false);
	}

	private void OnRejectAll()
	{
		rejectConfirmationScreen.transform.SetAsLastSibling();
		rejectConfirmationScreen.SetActive(true);
	}

	private void OnRejectionCancelled()
	{
		rejectConfirmationScreen.SetActive(false);
	}

	private void OnRejectionConfirmed()
	{
		telepad.RejectAll();
		containers.ForEach(delegate(ITelepadDeliverableContainer cc)
		{
			Object.Destroy(cc.GetGameObject());
		});
		containers.Clear();
		rejectConfirmationScreen.SetActive(false);
		Show(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
	}
}
