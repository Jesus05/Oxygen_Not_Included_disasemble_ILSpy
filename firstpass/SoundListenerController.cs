using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundListenerController : MonoBehaviour
{
	private VCA loopingVCA;

	public static SoundListenerController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Start()
	{
		if (RuntimeManager.IsInitialized)
		{
			RuntimeManager.StudioSystem.getVCA("vca:/Looping", out loopingVCA);
		}
		else
		{
			base.enabled = false;
		}
	}

	public void SetLoopingVolume(float volume)
	{
		loopingVCA.setVolume(volume);
	}

	private void Update()
	{
		Audio audio = Audio.Get();
		Vector3 position = Camera.main.transform.GetPosition();
		float a = (Camera.main.orthographicSize - audio.listenerMinOrthographicSize) / (audio.listenerReferenceOrthographicSize - audio.listenerMinOrthographicSize);
		a = Mathf.Max(a, 0f);
		float num = position.z = 0f - audio.listenerMinZ - a * (audio.listenerReferenceZ - audio.listenerMinZ);
		base.transform.SetPosition(position);
	}
}
