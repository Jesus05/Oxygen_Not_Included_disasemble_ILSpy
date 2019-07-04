using System;
using UnityEngine;

public class FlowOffsetRenderer : KMonoBehaviour
{
	private float GasPhase0;

	private float GasPhase1;

	public float PhaseMultiplier;

	public float NoiseInfluence;

	public float NoiseScale;

	public float OffsetSpeed;

	public string OffsetTextureName;

	public string ParametersName;

	public Vector2 MinFlow0;

	public Vector2 MinFlow1;

	public Vector2 LiquidGasMask;

	[SerializeField]
	private Material FlowMaterial;

	[SerializeField]
	private bool forceUpdate = false;

	private TextureLerper FlowLerper;

	public RenderTexture[] OffsetTextures = new RenderTexture[2];

	private int OffsetIdx;

	private float CurrentTime;

	protected override void OnSpawn()
	{
		FlowMaterial = new Material(Shader.Find("Klei/Flow"));
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		OnResize();
		DoUpdate(0.1f);
	}

	private void OnResize()
	{
		for (int i = 0; i < OffsetTextures.Length; i++)
		{
			if ((UnityEngine.Object)OffsetTextures[i] != (UnityEngine.Object)null)
			{
				OffsetTextures[i].DestroyRenderTexture();
			}
			OffsetTextures[i] = new RenderTexture(Screen.width / 2, Screen.height / 2, 0, RenderTextureFormat.ARGBHalf);
			OffsetTextures[i].filterMode = FilterMode.Bilinear;
			OffsetTextures[i].name = "FlowOffsetTexture";
		}
	}

	private void LateUpdate()
	{
		if ((Time.deltaTime > 0f && Time.timeScale > 0f) || forceUpdate)
		{
			float num = Time.deltaTime / Time.timeScale;
			DoUpdate(num * Time.timeScale / 4f + num * 0.5f);
		}
	}

	private void DoUpdate(float dt)
	{
		CurrentTime += dt;
		float num = CurrentTime * PhaseMultiplier;
		num -= (float)(int)num;
		float num2 = num - (float)(int)num;
		float y = 1f;
		if (num2 <= GasPhase0)
		{
			y = 0f;
		}
		GasPhase0 = num2;
		float z = 1f;
		float num3 = num + 0.5f - (float)(int)(num + 0.5f);
		if (num3 <= GasPhase1)
		{
			z = 0f;
		}
		GasPhase1 = num3;
		Shader.SetGlobalVector(ParametersName, new Vector4(GasPhase0, 0f, 0f, 0f));
		Shader.SetGlobalVector("_NoiseParameters", new Vector4(NoiseInfluence, NoiseScale, 0f, 0f));
		RenderTexture renderTexture = OffsetTextures[OffsetIdx];
		OffsetIdx = (OffsetIdx + 1) % 2;
		RenderTexture renderTexture2 = OffsetTextures[OffsetIdx];
		Material flowMaterial = FlowMaterial;
		flowMaterial.SetTexture("_PreviousOffsetTex", renderTexture);
		flowMaterial.SetVector("_FlowParameters", new Vector4(Time.deltaTime * OffsetSpeed, y, z, 0f));
		flowMaterial.SetVector("_MinFlow", new Vector4(MinFlow0.x, MinFlow0.y, MinFlow1.x, MinFlow1.y));
		flowMaterial.SetVector("_VisibleArea", new Vector4(0f, 0f, (float)Grid.WidthInCells, (float)Grid.HeightInCells));
		flowMaterial.SetVector("_LiquidGasMask", new Vector4(LiquidGasMask.x, LiquidGasMask.y, 0f, 0f));
		Graphics.Blit(renderTexture, renderTexture2, flowMaterial);
		Shader.SetGlobalTexture(OffsetTextureName, renderTexture2);
	}
}
