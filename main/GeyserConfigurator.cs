using Klei;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GeyserConfigurator : KMonoBehaviour
{
	public class GeyserType
	{
		public string id;

		public HashedString idHash;

		public SimHashes element;

		public float temperature;

		public float minRatePerCycle;

		public float maxRatePerCycle;

		public float maxPressure;

		public SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;

		public float minIterationLength;

		public float maxIterationLength;

		public float minIterationPercent;

		public float maxIterationPercent;

		public float minYearLength;

		public float maxYearLength;

		public float minYearPercent;

		public float maxYearPercent;

		public GeyserType(string id, SimHashes element, float temperature, float minRatePerCycle, float maxRatePerCycle, float maxPressure, float minIterationLength = 60f, float maxIterationLength = 1140f, float minIterationPercent = 0.1f, float maxIterationPercent = 0.9f, float minYearLength = 15000f, float maxYearLength = 135000f, float minYearPercent = 0.4f, float maxYearPercent = 0.8f)
		{
			this.id = id;
			idHash = id;
			this.element = element;
			this.temperature = temperature;
			this.minRatePerCycle = minRatePerCycle;
			this.maxRatePerCycle = maxRatePerCycle;
			this.maxPressure = maxPressure;
			this.minIterationLength = minIterationLength;
			this.maxIterationLength = maxIterationLength;
			this.minIterationPercent = minIterationPercent;
			this.maxIterationPercent = maxIterationPercent;
			this.minYearLength = minYearLength;
			this.maxYearLength = maxYearLength;
			this.minYearPercent = minYearPercent;
			this.maxYearPercent = maxYearPercent;
			if (geyserTypes == null)
			{
				geyserTypes = new List<GeyserType>();
			}
			geyserTypes.Add(this);
		}

		public GeyserType AddDisease(SimUtil.DiseaseInfo diseaseInfo)
		{
			this.diseaseInfo = diseaseInfo;
			return this;
		}
	}

	[Serializable]
	public class GeyserInstanceConfiguration
	{
		public HashedString typeId;

		public float rateRoll;

		public float iterationLengthRoll;

		public float iterationPercentRoll;

		public float yearLengthRoll;

		public float yearPercentRoll;

		private float scaledRate;

		private float scaledIterationLength;

		private float scaledIterationPercent;

		private float scaledYearLength;

		private float scaledYearPercent;

		private bool didInit;

		public GeyserType geyserType => FindType(typeId);

		private void Init()
		{
			if (!didInit)
			{
				didInit = true;
				scaledRate = Resample(rateRoll, geyserType.minRatePerCycle, geyserType.maxRatePerCycle);
				scaledIterationLength = Resample(iterationLengthRoll, geyserType.minIterationLength, geyserType.maxIterationLength);
				scaledIterationPercent = Resample(iterationPercentRoll, geyserType.minIterationPercent, geyserType.maxIterationPercent);
				scaledYearLength = Resample(yearLengthRoll, geyserType.minYearLength, geyserType.maxYearLength);
				scaledYearPercent = Resample(yearPercentRoll, geyserType.minYearPercent, geyserType.maxYearPercent);
			}
		}

		public float GetMaxPressure()
		{
			return geyserType.maxPressure;
		}

		public float GetIterationLength()
		{
			Init();
			return scaledIterationLength;
		}

		public float GetIterationPercent()
		{
			Init();
			return scaledIterationPercent;
		}

		public float GetOnDuration()
		{
			return GetIterationLength() * GetIterationPercent();
		}

		public float GetOffDuration()
		{
			return GetIterationLength() * (1f - GetIterationPercent());
		}

		public float GetMassPerCycle()
		{
			Init();
			return scaledRate;
		}

		public float GetEmitRate()
		{
			float num = 600f / GetIterationLength();
			float num2 = GetMassPerCycle() / num;
			return num2 / GetOnDuration();
		}

		public float GetYearLength()
		{
			Init();
			return scaledYearLength;
		}

		public float GetYearPercent()
		{
			Init();
			return scaledYearPercent;
		}

		public float GetYearOnDuration()
		{
			return GetYearLength() * GetYearPercent();
		}

		public float GetYearOffDuration()
		{
			return GetYearLength() * (1f - GetYearPercent());
		}

		public SimHashes GetElement()
		{
			return geyserType.element;
		}

		public float GetTemperature()
		{
			return geyserType.temperature;
		}

		public byte GetDiseaseIdx()
		{
			return geyserType.diseaseInfo.idx;
		}

		public int GetDiseaseCount()
		{
			return geyserType.diseaseInfo.count;
		}

		private float Resample(float t, float min, float max)
		{
			float num = 6f;
			float num2 = 0.002472623f;
			float num3 = t * (1f - num2 * 2f) + num2;
			float num4 = 0f - Mathf.Log(1f / num3 - 1f);
			num4 = (num4 + num) / (num * 2f);
			return num4 * (max - min) + min;
		}
	}

	private static List<GeyserType> geyserTypes;

	public HashedString presetType;

	public float presetMin;

	public float presetMax = 1f;

	public static GeyserType FindType(HashedString typeId)
	{
		GeyserType geyserType = null;
		if (typeId != HashedString.Invalid)
		{
			geyserType = geyserTypes.Find((GeyserType t) => (HashedString)t.id == typeId);
		}
		if (geyserType == null)
		{
			Debug.LogError($"Tried finding a geyser with id {typeId.ToString()} but it doesn't exist!");
		}
		return geyserType;
	}

	public GeyserInstanceConfiguration MakeConfiguration()
	{
		return CreateRandomInstance(presetType, presetMin, presetMax);
	}

	private GeyserInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		int globalWorldSeed = SaveLoader.Instance.worldDetailSave.globalWorldSeed;
		int num = globalWorldSeed;
		Vector3 position = base.transform.GetPosition();
		int num2 = num + (int)position.x;
		Vector3 position2 = base.transform.GetPosition();
		globalWorldSeed = num2 + (int)position2.y;
		System.Random randomSource = new System.Random(globalWorldSeed);
		GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserInstanceConfiguration();
		geyserInstanceConfiguration.typeId = typeId;
		geyserInstanceConfiguration.rateRoll = Roll(randomSource, min, max);
		geyserInstanceConfiguration.iterationLengthRoll = Roll(randomSource, 0f, 1f);
		geyserInstanceConfiguration.iterationPercentRoll = Roll(randomSource, min, max);
		geyserInstanceConfiguration.yearLengthRoll = Roll(randomSource, 0f, 1f);
		geyserInstanceConfiguration.yearPercentRoll = Roll(randomSource, min, max);
		return geyserInstanceConfiguration;
	}

	private float Roll(System.Random randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}
}
