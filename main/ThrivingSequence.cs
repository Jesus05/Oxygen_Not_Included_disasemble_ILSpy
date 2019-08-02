using FMOD.Studio;
using System;
using System.Collections;
using UnityEngine;

public static class ThrivingSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(Sequence());
	}

	private static IEnumerator Sequence()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.Thriving.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS", false);
		Vector3 vector = Vector3.up * 5f;
		IEnumerator enumerator = Components.Telepads.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Telepad telepad = (Telepad)enumerator.Current;
				if ((UnityEngine.Object)telepad != (UnityEngine.Object)null)
				{
					GameObject gameObject = telepad.gameObject;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			IDisposable disposable2 = disposable = (enumerator as IDisposable);
			if (disposable != null)
			{
				disposable2.Dispose();
			}
		}
		CameraController.Instance.FadeOut(1f, 2f);
		yield return (object)new WaitForSecondsRealtime(1f);
		/*Error: Unable to find new state assignment for yield return*/;
	}
}
