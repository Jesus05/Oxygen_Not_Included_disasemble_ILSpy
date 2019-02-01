using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventManager : KMonoBehaviour
{
	public enum NoiseEffect
	{
		Peaceful = 0,
		Quiet = 36,
		TossAndTurn = 45,
		WakeUp = 60,
		Passive = 80,
		Active = 106
	}

	public struct PolluterDisplay
	{
		public string name;

		public float value;

		public IPolluter provider;
	}

	public const float NO_NOISE_EFFECTORS = 0f;

	public const float MIN_LOUDNESS_THRESHOLD = 1f;

	private static AudioEventManager instance;

	private List<Pair<float, NoiseSplat>> removeTime = new List<Pair<float, NoiseSplat>>();

	private Dictionary<int, List<Polluter>> freePool = new Dictionary<int, List<Polluter>>();

	private Dictionary<int, List<Polluter>> inusePool = new Dictionary<int, List<Polluter>>();

	private HashSet<NoiseSplat> splats = new HashSet<NoiseSplat>();

	private UniformGrid<NoiseSplat> spatialSplats = new UniformGrid<NoiseSplat>();

	private List<PolluterDisplay> polluters = new List<PolluterDisplay>();

	public static AudioEventManager Get()
	{
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			if (App.IsExiting)
			{
				return null;
			}
			GameObject gameObject = GameObject.Find("/AudioEventManager");
			if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
			{
				gameObject = new GameObject();
				gameObject.name = "AudioEventManager";
			}
			instance = gameObject.GetComponent<AudioEventManager>();
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = gameObject.AddComponent<AudioEventManager>();
			}
		}
		return instance;
	}

	protected override void OnSpawn()
	{
		base.OnPrefabInit();
		spatialSplats.Reset(Grid.WidthInCells, Grid.HeightInCells, 16, 16);
	}

	public static float LoudnessToDB(float loudness)
	{
		return (!(loudness > 0f)) ? 0f : (10f * Mathf.Log10(loudness));
	}

	public static float DBToLoudness(float src_db)
	{
		return Mathf.Pow(10f, src_db / 10f);
	}

	public float GetDecibelsAtCell(int cell)
	{
		float loudness = Grid.Loudness[cell];
		float num = LoudnessToDB(loudness);
		return Mathf.Round(num * 2f) / 2f;
	}

	public static string GetLoudestNoisePolluterAtCell(int cell)
	{
		float num = float.NegativeInfinity;
		string result = null;
		AudioEventManager audioEventManager = Get();
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2 pos = new Vector2((float)vector2I.x, (float)vector2I.y);
		IEnumerator enumerator = audioEventManager.spatialSplats.GetAllIntersecting(pos).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NoiseSplat noiseSplat = (NoiseSplat)enumerator.Current;
				float loudness = noiseSplat.GetLoudness(cell);
				if (loudness > num)
				{
					result = noiseSplat.GetProvider().GetName();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return result;
	}

	public void ClearNoiseSplat(NoiseSplat splat)
	{
		if (splats.Contains(splat))
		{
			splats.Remove(splat);
			spatialSplats.Remove(splat);
		}
	}

	public void AddSplat(NoiseSplat splat)
	{
		splats.Add(splat);
		spatialSplats.Add(splat);
	}

	public NoiseSplat CreateNoiseSplat(Vector2 pos, int dB, int radius, string name, GameObject go)
	{
		Polluter polluter = GetPolluter(radius);
		polluter.SetAttributes(pos, dB, go, name);
		NoiseSplat noiseSplat = new NoiseSplat(polluter, 0f);
		polluter.SetSplat(noiseSplat);
		return noiseSplat;
	}

	public List<PolluterDisplay> GetPollutersForCell(int cell)
	{
		polluters.Clear();
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2 pos = new Vector2((float)vector2I.x, (float)vector2I.y);
		IEnumerator enumerator = spatialSplats.GetAllIntersecting(pos).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NoiseSplat noiseSplat = (NoiseSplat)enumerator.Current;
				float loudness = noiseSplat.GetLoudness(cell);
				if (!(loudness <= 0f))
				{
					PolluterDisplay item = default(PolluterDisplay);
					item.name = noiseSplat.GetName();
					item.value = LoudnessToDB(loudness);
					item.provider = noiseSplat.GetProvider();
					polluters.Add(item);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return polluters;
	}

	private void RemoveExpiredSplats()
	{
		if (removeTime.Count > 1)
		{
			removeTime.Sort((Pair<float, NoiseSplat> a, Pair<float, NoiseSplat> b) => a.first.CompareTo(b.first));
		}
		int num = -1;
		for (int i = 0; i < removeTime.Count; i++)
		{
			Pair<float, NoiseSplat> pair = removeTime[i];
			if (pair.first > Time.time)
			{
				break;
			}
			Pair<float, NoiseSplat> pair2 = removeTime[i];
			NoiseSplat second = pair2.second;
			if (second != null)
			{
				IPolluter provider = second.GetProvider();
				FreePolluter(provider as Polluter);
			}
			num = i;
		}
		for (int num2 = num; num2 >= 0; num2--)
		{
			removeTime.RemoveAt(num2);
		}
	}

	private void Update()
	{
		RemoveExpiredSplats();
	}

	private Polluter GetPolluter(int radius)
	{
		Polluter polluter = null;
		if (!freePool.ContainsKey(radius))
		{
			freePool.Add(radius, new List<Polluter>());
		}
		if (freePool[radius].Count > 0)
		{
			polluter = freePool[radius][0];
			freePool[radius].RemoveAt(0);
		}
		else
		{
			polluter = new Polluter(radius);
		}
		if (!inusePool.ContainsKey(radius))
		{
			inusePool.Add(radius, new List<Polluter>());
		}
		inusePool[radius].Add(polluter);
		return polluter;
	}

	private void FreePolluter(Polluter pol)
	{
		if (pol != null)
		{
			pol.Clear();
			inusePool[pol.radius].Remove(pol);
			freePool[pol.radius].Add(pol);
		}
	}

	public void PlayTimedOnceOff(Vector2 pos, int dB, int radius, string name, GameObject go, float time = 1f)
	{
		if (dB > 0 && radius > 0 && time > 0f)
		{
			Polluter polluter = GetPolluter(radius);
			polluter.SetAttributes(pos, dB, go, name);
			AddTimedInstance(polluter, time);
		}
	}

	private void AddTimedInstance(Polluter p, float time)
	{
		NoiseSplat noiseSplat = new NoiseSplat(p, time + Time.time);
		p.SetSplat(noiseSplat);
		removeTime.Add(new Pair<float, NoiseSplat>(time + Time.time, noiseSplat));
	}

	private static void SoundLog(long itemId, string message)
	{
		Debug.Log(" [" + itemId + "] \t" + message, null);
	}
}
