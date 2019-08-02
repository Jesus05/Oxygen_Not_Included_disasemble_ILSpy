using FMOD.Studio;
using System.Collections;
using UnityEngine;

public static class ReachedDistantPlanetSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(Sequence());
	}

	private static IEnumerator Sequence()
	{
		Vector3 cameraTagetMid = Vector3.zero;
		Vector3 zero = Vector3.zero;
		foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
		{
			if (item.state != 0 && SpacecraftManager.instance.GetSpacecraftDestination(item.id).GetDestinationType().Id == Db.Get().SpaceDestinationTypes.Wormhole.Id)
			{
				foreach (RocketModule rocketModule in item.launchConditions.rocketModules)
				{
					if ((Object)rocketModule.GetComponent<RocketEngine>() != (Object)null)
					{
						cameraTagetMid = rocketModule.gameObject.transform.position + Vector3.up * 7f;
						break;
					}
				}
				Vector3 vector = cameraTagetMid + Vector3.up * 20f;
			}
		}
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		CameraController.Instance.FadeOut(1f, 1f);
		yield return (object)new WaitForSecondsRealtime(3f);
		/*Error: Unable to find new state assignment for yield return*/;
	}
}
