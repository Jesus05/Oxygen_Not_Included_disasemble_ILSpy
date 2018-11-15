using Klei.AI;
using System;

public class SandboxSettings
{
	private KPrefabID entity;

	private Element element;

	private Disease disease;

	private int brushSize;

	private float noiseScale;

	private float noiseDensity;

	private float mass;

	private bool instantBuild;

	public float temperature;

	public float temperatureAdditive;

	public int diseaseCount;

	public System.Action OnChangeElement;

	public System.Action OnChangeDisease;

	public System.Action OnChangeEntity;

	public System.Action OnChangeBrushSize;

	public System.Action OnChangeNoiseScale;

	public System.Action OnChangeNoiseDensity;

	public KPrefabID Entity
	{
		get
		{
			return entity;
		}
		set
		{
			entity = value;
		}
	}

	public Element Element
	{
		get
		{
			return element;
		}
		set
		{
			SelectElement(value);
		}
	}

	public Disease Disease
	{
		get
		{
			return disease;
		}
		set
		{
			SelectDisease(value);
		}
	}

	public int BrushSize
	{
		get
		{
			return brushSize;
		}
		set
		{
			SetBrushSize(value);
		}
	}

	public float NoiseScale
	{
		get
		{
			return noiseScale;
		}
		set
		{
			SetNoiseScale(value);
		}
	}

	public float NoiseDensity
	{
		get
		{
			return noiseDensity;
		}
		set
		{
			SetNoiseDensity(value);
		}
	}

	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			mass = value;
		}
	}

	public bool InstantBuild
	{
		get
		{
			return instantBuild;
		}
		set
		{
			instantBuild = value;
		}
	}

	public void SelectEntity(KPrefabID entity)
	{
		this.entity = entity;
		OnChangeEntity();
	}

	public void SelectElement(Element element)
	{
		this.element = element;
		OnChangeElement();
	}

	public void SelectDisease(Disease disease)
	{
		this.disease = disease;
		OnChangeDisease();
	}

	public void SetBrushSize(int size)
	{
		brushSize = size;
		OnChangeBrushSize();
	}

	public void SetNoiseScale(float amount)
	{
		noiseScale = amount;
		OnChangeNoiseScale();
	}

	public void SetNoiseDensity(float amount)
	{
		noiseDensity = amount;
		OnChangeNoiseDensity();
	}
}
