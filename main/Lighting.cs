using System;
using UnityEngine;

[ExecuteInEditMode]
public class Lighting : MonoBehaviour
{
	public LightingSettings Settings;

	public static Lighting Instance;

	[NonSerialized]
	public bool disableLighting;

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private Color PremultiplyAlpha(Color c)
	{
		return c * c.a;
	}

	private void Update()
	{
		Shader.SetGlobalInt("_LiquidZ", -25);
		Shader.SetGlobalInt("_SceneLayerMax", 31);
		Shader.SetGlobalColor("_StateTransitionColour", Settings.StateTransitionColor);
		Shader.SetGlobalVector("_DigMapMapParameters", new Vector4(Settings.DigMapColour.r, Settings.DigMapColour.g, Settings.DigMapColour.b, Settings.DigMapScale));
		Shader.SetGlobalTexture("_DigDamageMap", Settings.DigDamageMap);
		Shader.SetGlobalTexture("_StateTransitionMap", Settings.StateTransitionMap);
		Shader.SetGlobalColor("_StateTransitionColor", Settings.StateTransitionColor);
		Shader.SetGlobalVector("_StateTransitionParameters", new Vector4(1f / Settings.StateTransitionUVScale, Settings.StateTransitionUVOffsetRate.x, Settings.StateTransitionUVOffsetRate.y, 0f));
		Shader.SetGlobalColor("_WaterTrimColor", Settings.WaterTrimColor);
		Shader.SetGlobalVector("_WaterParameters2", new Vector4(Settings.WaterTrimSize, Settings.WaterAlphaTrimSize, 0f, Settings.WaterAlphaThreshold));
		Shader.SetGlobalVector("_WaterWaveParameters", new Vector4(Settings.WaterWaveAmplitude, Settings.WaterWaveFrequency, Settings.WaterWaveSpeed, 0f));
		Shader.SetGlobalVector("_WaterWaveParameters2", new Vector4(Settings.WaterWaveAmplitude2, Settings.WaterWaveFrequency2, Settings.WaterWaveSpeed2, 0f));
		Shader.SetGlobalVector("_WaterDetailParameters", new Vector4(Settings.WaterCubeMapScale, Settings.WaterDetailTiling, Settings.WaterColorScale, Settings.WaterDetailTiling2));
		Shader.SetGlobalVector("_WaterDistortionParameters", new Vector4(Settings.WaterDistortionScaleStart, Settings.WaterDistortionScaleEnd, Settings.WaterDepthColorOpacityStart, Settings.WaterDepthColorOpacityEnd));
		Shader.SetGlobalVector("_BloomParameters", new Vector4(Settings.BloomScale, 0f, 0f, 0f));
		Shader.SetGlobalVector("_LiquidParameters2", new Vector4(Settings.LiquidMin, Settings.LiquidMax, Settings.LiquidCutoff, Settings.LiquidTransparency));
		Shader.SetGlobalVector("_GridParameters", new Vector4(Settings.GridLineWidth, Settings.GridSize, Settings.GridMinIntensity, Settings.GridMaxIntensity));
		Shader.SetGlobalColor("_GridColor", Settings.GridColor);
		Shader.SetGlobalVector("_EdgeGlowParameters", new Vector4(Settings.EdgeGlowCutoffStart, Settings.EdgeGlowCutoffEnd, Settings.EdgeGlowIntensity, 0f));
		if (disableLighting)
		{
			Shader.SetGlobalVector("_SubstanceParameters", new Vector4(1f, 1f, 1f, 1f));
			Shader.SetGlobalVector("_TileEdgeParameters", new Vector4(1f, 1f, 1f, 1f));
		}
		else
		{
			Shader.SetGlobalVector("_SubstanceParameters", new Vector4(Settings.substanceEdgeParameters.intensity, Settings.substanceEdgeParameters.edgeIntensity, Settings.substanceEdgeParameters.directSunlightScale, Settings.substanceEdgeParameters.power));
			Shader.SetGlobalVector("_TileEdgeParameters", new Vector4(Settings.tileEdgeParameters.intensity, Settings.tileEdgeParameters.edgeIntensity, Settings.tileEdgeParameters.directSunlightScale, Settings.tileEdgeParameters.power));
		}
		float w = (!((UnityEngine.Object)SimDebugView.Instance != (UnityEngine.Object)null) || !(SimDebugView.Instance.GetMode() == OverlayModes.Disease.ID)) ? 0f : 1f;
		if (disableLighting)
		{
			Shader.SetGlobalVector("_AnimParameters", new Vector4(1f, Settings.WorldZoneAnimBlend, 0f, w));
		}
		else
		{
			Shader.SetGlobalVector("_AnimParameters", new Vector4(Settings.AnimIntensity, Settings.WorldZoneAnimBlend, 0f, w));
		}
		Shader.SetGlobalVector("_GasOpacity", new Vector4(Settings.GasMinOpacity, Settings.GasMaxOpacity, 0f, 0f));
		Shader.SetGlobalColor("_DarkenTintBackground", Settings.DarkenTints[0]);
		Shader.SetGlobalColor("_DarkenTintMidground", Settings.DarkenTints[1]);
		Shader.SetGlobalColor("_DarkenTintForeground", Settings.DarkenTints[2]);
		Shader.SetGlobalColor("_BrightenOverlay", Settings.BrightenOverlayColour);
		Shader.SetGlobalColor("_ColdFG", PremultiplyAlpha(Settings.ColdColours[2]));
		Shader.SetGlobalColor("_ColdMG", PremultiplyAlpha(Settings.ColdColours[1]));
		Shader.SetGlobalColor("_ColdBG", PremultiplyAlpha(Settings.ColdColours[0]));
		Shader.SetGlobalColor("_HotFG", PremultiplyAlpha(Settings.HotColours[2]));
		Shader.SetGlobalColor("_HotMG", PremultiplyAlpha(Settings.HotColours[1]));
		Shader.SetGlobalColor("_HotBG", PremultiplyAlpha(Settings.HotColours[0]));
		Shader.SetGlobalVector("_TemperatureParallax", Settings.TemperatureParallax);
		Shader.SetGlobalVector("_ColdUVOffset1", new Vector4(Settings.ColdBGUVOffset.x, Settings.ColdBGUVOffset.y, Settings.ColdMGUVOffset.x, Settings.ColdMGUVOffset.y));
		Shader.SetGlobalVector("_ColdUVOffset2", new Vector4(Settings.ColdFGUVOffset.x, Settings.ColdFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalVector("_HotUVOffset1", new Vector4(Settings.HotBGUVOffset.x, Settings.HotBGUVOffset.y, Settings.HotMGUVOffset.x, Settings.HotMGUVOffset.y));
		Shader.SetGlobalVector("_HotUVOffset2", new Vector4(Settings.HotFGUVOffset.x, Settings.HotFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalColor("_DustColour", PremultiplyAlpha(Settings.DustColour));
		Shader.SetGlobalVector("_DustInfo", new Vector4(Settings.DustScale, Settings.DustMovement.x, Settings.DustMovement.y, Settings.DustMovement.z));
		Shader.SetGlobalTexture("_DustTex", Settings.DustTex);
		Shader.SetGlobalVector("_DebugShowInfo", new Vector4(Settings.ShowDust, Settings.ShowGas, Settings.ShowShadow, Settings.ShowTemperature));
		Shader.SetGlobalVector("_HeatHazeParameters", Settings.HeatHazeParameters);
		Shader.SetGlobalTexture("_HeatHazeTexture", Settings.HeatHazeTexture);
		Shader.SetGlobalVector("_ShineParams", new Vector4(Settings.ShineCenter.x, Settings.ShineCenter.y, Settings.ShineRange.x, Settings.ShineRange.y));
		Shader.SetGlobalVector("_ShineParams2", new Vector4(Settings.ShineZoomSpeed, 0f, 0f, 0f));
		Shader.SetGlobalFloat("_WorldZoneGasBlend", Settings.WorldZoneGasBlend);
		Shader.SetGlobalFloat("_WorldZoneLiquidBlend", Settings.WorldZoneLiquidBlend);
		Shader.SetGlobalFloat("_WorldZoneForegroundBlend", Settings.WorldZoneForegroundBlend);
		Shader.SetGlobalFloat("_WorldZoneSimpleAnimBlend", Settings.WorldZoneSimpleAnimBlend);
		Shader.SetGlobalColor("_CharacterLitColour", Settings.characterLighting.litColour);
		Shader.SetGlobalColor("_CharacterUnlitColour", Settings.characterLighting.unlitColour);
		Shader.SetGlobalTexture("_BuildingDamagedTex", Settings.BuildingDamagedTex);
		Shader.SetGlobalVector("_BuildingDamagedUVParameters", Settings.BuildingDamagedUVParameters);
		Shader.SetGlobalTexture("_DiseaseOverlayTex", Settings.DiseaseOverlayTex);
		Shader.SetGlobalVector("_DiseaseOverlayTexInfo", Settings.DiseaseOverlayTexInfo);
		if ((UnityEngine.Object)LightBuffer.Instance != (UnityEngine.Object)null && (UnityEngine.Object)LightBuffer.Instance.Texture != (UnityEngine.Object)null)
		{
			Shader.SetGlobalTexture("_LightBufferTex", LightBuffer.Instance.Texture);
		}
	}
}
