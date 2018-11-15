using FMOD.Studio;
using System.Collections;

public class SplashMessageScreen : KMonoBehaviour
{
	public KButton confirmButton;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
		StartCoroutine(ShowMessage());
	}

	private IEnumerator ShowMessage()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}
}
