using UnityEngine;

public class LightBufferCompositor : MonoBehaviour
{
	[SerializeField]
	private Material material;

	[SerializeField]
	private Material blurMaterial;

	private bool particlesEnabled = true;

	private void Start()
	{
		material = new Material(Shader.Find("Klei/PostFX/LightBufferCompositor"));
		material.SetTexture("_InvalidTex", Assets.instance.invalidAreaTex);
		blurMaterial = new Material(Shader.Find("Klei/PostFX/Blur"));
		OnShadersReloaded();
		ShaderReloader.Register(OnShadersReloaded);
	}

	private void OnEnable()
	{
		OnShadersReloaded();
	}

	private void DownSample4x(Texture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, blurMaterial, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	[ContextMenu("ToggleParticles")]
	private void ToggleParticles()
	{
		particlesEnabled = !particlesEnabled;
		UpdateMaterialState();
	}

	public void SetParticlesEnabled(bool enabled)
	{
		particlesEnabled = enabled;
		UpdateMaterialState();
	}

	private void UpdateMaterialState()
	{
		if (particlesEnabled)
		{
			material.DisableKeyword("DISABLE_TEMPERATURE_PARTICLES");
		}
		else
		{
			material.EnableKeyword("DISABLE_TEMPERATURE_PARTICLES");
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (!((Object)PropertyTextures.instance == (Object)null))
		{
			Texture texture = PropertyTextures.instance.GetTexture(PropertyTextures.Property.Temperature);
			texture.name = "temperature_tex";
			RenderTexture temporary = RenderTexture.GetTemporary(Screen.width / 8, Screen.height / 8);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(texture, temporary, blurMaterial);
			Shader.SetGlobalTexture("_BlurredTemperature", temporary);
			material.SetTexture("_LightBufferTex", LightBuffer.Instance.Texture);
			Graphics.Blit(src, dest, material);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}

	private void OnShadersReloaded()
	{
		if ((Object)material != (Object)null && (Object)Lighting.Instance != (Object)null)
		{
			material.SetTexture("_EmberTex", Lighting.Instance.Settings.EmberTex);
			material.SetTexture("_FrostTex", Lighting.Instance.Settings.FrostTex);
			material.SetTexture("_Thermal1Tex", Lighting.Instance.Settings.Thermal1Tex);
			material.SetTexture("_Thermal2Tex", Lighting.Instance.Settings.Thermal2Tex);
		}
	}
}
