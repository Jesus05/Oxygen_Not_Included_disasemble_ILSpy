using UnityEngine;

public class Polluter : IPolluter
{
	private int _radius;

	private int decibels;

	private Vector2 position;

	private string sourceName;

	private GameObject gameObject;

	private NoiseSplat splat;

	public int radius
	{
		get
		{
			return _radius;
		}
		private set
		{
			_radius = value;
			if (_radius == 0)
			{
				Debug.LogFormat("[{0}] has a 0 radius noise, this will disable it", GetName());
			}
		}
	}

	public Polluter(int radius)
	{
		this.radius = radius;
	}

	public void SetAttributes(Vector2 pos, int dB, GameObject go, string name)
	{
		position = pos;
		sourceName = name;
		decibels = dB;
		gameObject = go;
	}

	public string GetName()
	{
		return sourceName;
	}

	public int GetRadius()
	{
		return radius;
	}

	public int GetNoise()
	{
		return decibels;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}

	public void SetSplat(NoiseSplat new_splat)
	{
		if (new_splat == null && splat != null)
		{
			Clear();
		}
		splat = new_splat;
		if (splat != null)
		{
			AudioEventManager.Get().AddSplat(splat);
		}
	}

	public void Clear()
	{
		if (splat != null)
		{
			AudioEventManager.Get().ClearNoiseSplat(splat);
			splat.Clear();
			splat = null;
		}
	}

	public Vector2 GetPosition()
	{
		return position;
	}
}
