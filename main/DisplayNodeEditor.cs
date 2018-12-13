using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using NodeEditorFramework;
using ProcGen.Noise;
using ProcGenGame;
using System;
using UnityEngine;

[Node(false, "Noise/Display", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class DisplayNodeEditor : BaseNodeEditor
{
	public enum DisplayType
	{
		DefaultColour,
		ElementColourBiome,
		ElementColourFeature
	}

	private const string Id = "displayNodeEditor";

	[SerializeField]
	public DisplayType displayType;

	private const int width = 256;

	private const int height = 256;

	private Texture2D texture;

	private ElementBandConfiguration biome;

	private string[] biomeOptions;

	private string[] featureOptions;

	public override string GetID => "displayNodeEditor";

	public override Type GetObjectType => typeof(DisplayNodeEditor);

	public override NoiseBase GetTarget()
	{
		return null;
	}

	public override Node Create(Vector2 pos)
	{
		DisplayNodeEditor displayNodeEditor = ScriptableObject.CreateInstance<DisplayNodeEditor>();
		displayNodeEditor.rect = new Rect(pos.x, pos.y, 266f, 301f);
		displayNodeEditor.name = "Noise Display Node";
		displayNodeEditor.CreateInput("Source Node", "IModule3D", NodeSide.Left, 40f);
		return displayNodeEditor;
	}

	public override bool Calculate()
	{
		if (!allInputsReady() || base.settings == null)
		{
			return false;
		}
		IModule3D value = Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		InitSettings();
		Vector2f lowerBound = base.settings.lowerBound;
		Vector2f upperBound = base.settings.upperBound;
		NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, base.settings.seamless);
		noiseMapBuilderPlane.SetSize(256, 256);
		noiseMapBuilderPlane.SourceModule = value;
		Vector2 zero = Vector2.zero;
		float[] noise = WorldGen.GenerateNoise(zero, base.settings.zoom, noiseMapBuilderPlane, 256, 256, null);
		if (base.settings.normalise)
		{
			WorldGen.Normalise(noise);
		}
		GetColourDelegate getColourDelegate = null;
		switch (displayType)
		{
		case DisplayType.DefaultColour:
			getColourDelegate = ((int cell) => Color.HSVToRGB((40f + 320f * noise[cell]) / 360f, 1f, 1f));
			break;
		case DisplayType.ElementColourBiome:
		case DisplayType.ElementColourFeature:
			getColourDelegate = delegate(int cell)
			{
				if (biome == null)
				{
					return Color.black;
				}
				float num = noise[cell];
				Element element = ElementLoader.FindElementByName(biome[biome.Count - 1].content);
				for (int i = 0; i < biome.Count; i++)
				{
					if (num < biome[i].maxValue)
					{
						element = ElementLoader.FindElementByName(biome[i].content);
						break;
					}
				}
				return element.substance.uiColour;
			};
			break;
		}
		if (getColourDelegate != null)
		{
			SetColours(getColourDelegate);
		}
		return true;
	}

	private void SetColours(GetColourDelegate getColourCall)
	{
		texture = SimDebugView.CreateTexture(out byte[] textureBytes, 256, 256);
		for (int i = 0; i < 65536; i++)
		{
			Color color = getColourCall(i);
			int num = i * 4;
			textureBytes[num] = (byte)(Mathf.Min(color.r, 1f) * 255f);
			textureBytes[num + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
			textureBytes[num + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
			textureBytes[num + 3] = byte.MaxValue;
		}
		texture.LoadRawTextureData(textureBytes);
		texture.Apply();
	}

	private void InitSettings()
	{
		if (WorldGen.Settings == null)
		{
			WorldGen.LoadSettings();
		}
	}

	private void GetBiomeOptions()
	{
		if (biomeOptions == null)
		{
			InitSettings();
			biomeOptions = WorldGen.Settings.biomes.GetNames();
		}
	}

	private void GetFeatureOptions()
	{
		if (featureOptions == null)
		{
			InitSettings();
			featureOptions = WorldGen.Settings.features.GetNames();
		}
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
