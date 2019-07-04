using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WattsonMessage : KScreen
{
	public class Tuning : TuningData<Tuning>
	{
		public float initialOrthographicSize;
	}

	private const float STARTTIME = 0.1f;

	private const float ENDTIME = 6.6f;

	private const float ALPHA_SPEED = 0.01f;

	private const float expandedHeight = 300f;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private KButton button;

	[SerializeField]
	[EventRef]
	private string dialogSound;

	private List<KScreen> hideScreensWhileActive = new List<KScreen>();

	private bool startFade = false;

	private List<SchedulerHandle> scheduleHandles = new List<SchedulerHandle>();

	private static readonly HashedString[] WorkLoopAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game.Instance.Subscribe(-122303817, OnNewBaseCreated);
	}

	private IEnumerator ExpandPanel()
	{
		yield return (object)new WaitForSecondsRealtime(5f);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private IEnumerator CollapsePanel()
	{
		float height2 = 300f;
		if (height2 > 1f)
		{
			Vector2 sizeDelta = dialog.rectTransform().sizeDelta;
			height2 = Mathf.Lerp(sizeDelta.y, 0f, Time.unscaledDeltaTime * 15f);
			RectTransform rectTransform = dialog.rectTransform();
			Vector2 sizeDelta2 = dialog.rectTransform().sizeDelta;
			rectTransform.sizeDelta = new Vector2(sizeDelta2.x, height2);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		Deactivate();
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		hideScreensWhileActive.Add(NotificationScreen.Instance);
		hideScreensWhileActive.Add(OverlayMenu.Instance);
		if ((Object)PlanScreen.Instance != (Object)null)
		{
			hideScreensWhileActive.Add(PlanScreen.Instance);
		}
		if ((Object)BuildMenu.Instance != (Object)null)
		{
			hideScreensWhileActive.Add(BuildMenu.Instance);
		}
		hideScreensWhileActive.Add(ManagementMenu.Instance);
		hideScreensWhileActive.Add(ToolMenu.Instance);
		hideScreensWhileActive.Add(ToolMenu.Instance.PriorityScreen);
		hideScreensWhileActive.Add(ResourceCategoryScreen.Instance);
		hideScreensWhileActive.Add(TopLeftControlScreen.Instance);
		hideScreensWhileActive.Add(DateTime.Instance);
		hideScreensWhileActive.Add(BuildWatermark.Instance);
		foreach (KScreen item in hideScreensWhileActive)
		{
			item.Show(false);
		}
	}

	public void Update()
	{
		if (startFade)
		{
			Color color = bg.color;
			color.a -= 0.01f;
			if (color.a <= 0f)
			{
				color.a = 0f;
			}
			bg.color = color;
		}
	}

	protected override void OnActivate()
	{
		Debug.Log("WattsonMessage OnActivate");
		base.OnActivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NewBaseSetupSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().IntroNIS);
		AudioMixer.instance.activeNIS = true;
		button.onClick += delegate
		{
			StartCoroutine(CollapsePanel());
		};
		dialog.GetComponent<KScreen>().Show(false);
		startFade = false;
		GameObject telepad = GameUtil.GetTelepad();
		if ((Object)telepad != (Object)null)
		{
			KAnimControllerBase kac = telepad.GetComponent<KAnimControllerBase>();
			kac.Play(WorkLoopAnims, KAnim.PlayMode.Loop);
			for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
			{
				int idx = i + 1;
				MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
				Transform transform = minionIdentity.gameObject.transform;
				Vector3 position = telepad.transform.GetPosition();
				float x = position.x + (float)idx - 1.5f;
				Vector3 position2 = telepad.transform.GetPosition();
				float y = position2.y;
				Vector3 position3 = minionIdentity.gameObject.transform.GetPosition();
				transform.SetPosition(new Vector3(x, y, position3.z));
				GameObject gameObject = minionIdentity.gameObject;
				ChoreProvider chore_provider = gameObject.GetComponent<ChoreProvider>();
				EmoteChore chorePre = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[1]
				{
					"portalbirth_pre_" + idx
				}, KAnim.PlayMode.Loop, false);
				UIScheduler.Instance.Schedule("DupeBirth", (float)idx * 0.5f, delegate
				{
					chorePre.Cancel("Done looping");
					new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[1]
					{
						"portalbirth_" + idx
					}, null);
				}, null, null);
			}
			UIScheduler.Instance.Schedule("Welcome", 6.6f, delegate
			{
				kac.Play(new HashedString[2]
				{
					"working_pst",
					"idle"
				}, KAnim.PlayMode.Once);
			}, null, null);
			CameraController.Instance.DisableUserCameraControl = true;
		}
		else
		{
			Debug.LogWarning("Failed to spawn telepad - does the starting base template lack a 'Headquarters' ?");
		}
		scheduleHandles.Add(UIScheduler.Instance.Schedule("GoHome", 0.1f, delegate
		{
			CameraController.Instance.SetOrthographicsSize(TuningData<Tuning>.Get().initialOrthographicSize);
			CameraController.Instance.CameraGoHome(1f);
			startFade = true;
			StartCoroutine(ExpandPanel());
			MusicManager.instance.PlaySong("Music_WattsonMessage", false);
		}, null, null));
		scheduleHandles.Add(UIScheduler.Instance.Schedule("WelcomeDialog", 7.6f, delegate
		{
			SpeedControlScreen.Instance.Pause(false);
			KFMOD.PlayOneShot(dialogSound);
			dialog.GetComponent<KScreen>().Activate();
			dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
			dialog.GetComponent<KScreen>().Show(true);
		}, null, null));
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().IntroNIS, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StartPersistentSnapshots();
		MusicManager.instance.StopSong("Music_WattsonMessage", true, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.PlayDynamicMusic();
		AudioMixer.instance.activeNIS = false;
		DemoTimer.Instance.CountdownActive = true;
		SpeedControlScreen.Instance.Unpause(false);
		CameraController.Instance.DisableUserCameraControl = false;
		foreach (SchedulerHandle scheduleHandle in scheduleHandles)
		{
			scheduleHandle.ClearScheduler();
		}
		UIScheduler.Instance.Schedule("fadeInUI", 0.5f, delegate
		{
			GameScheduler.Instance.Schedule("BasicTutorial", 1.5f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Basics);
			}, null, null);
			GameScheduler.Instance.Schedule("WelcomeTutorial", 2f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Welcome);
			}, null, null);
			foreach (KScreen item in hideScreensWhileActive)
			{
				item.SetShouldFadeIn(true);
				item.Show(true);
			}
			CameraController.Instance.SetMaxOrthographicSize(20f);
			Game.Instance.timelapser.SaveScreenshot();
		}, null, null);
		Game.Instance.SetGameStarted();
		if ((Object)TopLeftControlScreen.Instance != (Object)null)
		{
			TopLeftControlScreen.Instance.RefreshName();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			CameraController.Instance.CameraGoHome(2f);
			Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	private void OnNewBaseCreated(object data)
	{
		base.gameObject.SetActive(true);
	}
}
