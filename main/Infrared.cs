using UnityEngine;

public class Infrared : MonoBehaviour
{
	public enum Mode
	{
		Disabled,
		Infrared,
		Disease
	}

	private RenderTexture minionTexture;

	private RenderTexture cameraTexture;

	private Mode mode;

	public static int temperatureParametersId;

	public static Infrared Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Awake()
	{
		temperatureParametersId = Shader.PropertyToID("_TemperatureParameters");
		Instance = this;
		OnResize();
		UpdateState();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, minionTexture);
		Graphics.Blit(source, dest);
	}

	private void OnResize()
	{
		if ((Object)minionTexture != (Object)null)
		{
			minionTexture.DestroyRenderTexture();
		}
		if ((Object)cameraTexture != (Object)null)
		{
			cameraTexture.DestroyRenderTexture();
		}
		int num = 2;
		minionTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, RenderTextureFormat.ARGB32);
		cameraTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, RenderTextureFormat.ARGB32);
		GetComponent<Camera>().targetTexture = cameraTexture;
	}

	public void SetMode(Mode mode)
	{
		Vector4 value;
		switch (mode)
		{
		case Mode.Disabled:
			value = Vector4.zero;
			break;
		case Mode.Disease:
			value = new Vector4(1f, 0f, 0f, 0f);
			GameComps.InfraredVisualizers.ClearOverlayColour();
			break;
		default:
			value = new Vector4(1f, 0f, 0f, 0f);
			break;
		}
		Shader.SetGlobalVector("_ColouredOverlayParameters", value);
		this.mode = mode;
		UpdateState();
	}

	private void UpdateState()
	{
		base.enabled = (mode != Mode.Disabled);
		if (base.enabled)
		{
			Update();
		}
	}

	private void Update()
	{
		switch (mode)
		{
		case Mode.Infrared:
			GameComps.InfraredVisualizers.UpdateTemperature();
			break;
		case Mode.Disease:
			GameComps.DiseaseContainers.UpdateOverlayColours();
			break;
		}
	}
}
