using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashMessageScreen : KMonoBehaviour
{
	public KButton confirmButton;

	public LayoutElement bodyText;

	public bool previewInEditor;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
		};
	}

	private void OnEnable()
	{
		LayoutElement component = confirmButton.GetComponent<LayoutElement>();
		LocText componentInChildren = confirmButton.GetComponentInChildren<LocText>();
		if (Screen.width > 2560)
		{
			component.minWidth = 720f;
			component.minHeight = 128f;
			bodyText.minWidth = 840f;
			componentInChildren.fontSizeMax = 24f;
		}
		else if (Screen.width > 1920)
		{
			component.minWidth = 720f;
			component.minHeight = 128f;
			bodyText.minWidth = 700f;
			componentInChildren.fontSizeMax = 24f;
		}
		else if (Screen.width > 1280)
		{
			component.minWidth = 440f;
			component.minHeight = 64f;
			bodyText.minWidth = 480f;
			componentInChildren.fontSizeMax = 18f;
		}
		else
		{
			component.minWidth = 300f;
			component.minHeight = 48f;
			bodyText.minWidth = 300f;
			componentInChildren.fontSizeMax = 16f;
		}
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
