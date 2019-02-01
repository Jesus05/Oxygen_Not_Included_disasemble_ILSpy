using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarsConfig : ScriptableObject
{
	[Serializable]
	public struct BarData
	{
		public string barName;

		public Color barColor;

		public string barDescriptionKey;
	}

	public GameObject progressBarPrefab;

	public GameObject progressBarUIPrefab;

	public GameObject healthBarPrefab;

	public List<BarData> barColorDataList = new List<BarData>();

	public Dictionary<string, BarData> barColorMap = new Dictionary<string, BarData>();

	private static ProgressBarsConfig instance;

	public static ProgressBarsConfig Instance
	{
		get
		{
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = Resources.Load<ProgressBarsConfig>("ProgressBarsConfig");
				instance.Initialize();
			}
			return instance;
		}
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	public void Initialize()
	{
		foreach (BarData barColorData in barColorDataList)
		{
			BarData current = barColorData;
			barColorMap.Add(current.barName, current);
		}
	}

	public string GetBarDescription(string barName)
	{
		string result = "";
		if (IsBarNameValid(barName))
		{
			BarData barData = barColorMap[barName];
			result = Strings.Get(barData.barDescriptionKey);
		}
		return result;
	}

	public Color GetBarColor(string barName)
	{
		Color result = Color.clear;
		if (IsBarNameValid(barName))
		{
			BarData barData = barColorMap[barName];
			result = barData.barColor;
		}
		return result;
	}

	public bool IsBarNameValid(string barName)
	{
		if (!string.IsNullOrEmpty(barName))
		{
			if (barColorMap.ContainsKey(barName))
			{
				return true;
			}
			Debug.LogError($"No BarData found for the entry [ {barName} ]", null);
			return false;
		}
		Debug.LogError("The barName provided was null or empty. Don't do that.", null);
		return false;
	}
}
