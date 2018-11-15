using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using System;

public class ConduitDiseaseManager : KCompactedVector<ConduitDiseaseManager.Data>
{
	public struct Data
	{
		public byte diseaseIdx;

		public byte elemIdx;

		public int diseaseCount;

		public float accumulatedError;

		public float mass;

		public HandleVector<int>.Handle temperatureHandle;

		public ElemGrowthInfo growthInfo;

		public Data(HandleVector<int>.Handle temperature_handle, byte elem_idx, float mass, byte disease_idx, int disease_count)
		{
			diseaseIdx = disease_idx;
			elemIdx = elem_idx;
			this.mass = mass;
			diseaseCount = disease_count;
			accumulatedError = 0f;
			temperatureHandle = temperature_handle;
			growthInfo = GetGrowthInfo(disease_idx, elem_idx);
		}
	}

	private ConduitTemperatureManager temperatureManager;

	public ConduitDiseaseManager(ConduitTemperatureManager temperature_manager)
		: base(0)
	{
		temperatureManager = temperature_manager;
	}

	private static ElemGrowthInfo GetGrowthInfo(byte disease_idx, byte elem_idx)
	{
		if (disease_idx == 255)
		{
			return Disease.DEFAULT_GROWTH_INFO;
		}
		Disease disease = Db.Get().Diseases[disease_idx];
		return disease.elemGrowthInfo[elem_idx];
	}

	public HandleVector<int>.Handle Allocate(HandleVector<int>.Handle temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		byte elem_idx = (byte)ElementLoader.GetElementIndex(contents.element);
		Data initial_data = new Data(temperature_handle, elem_idx, contents.mass, contents.diseaseIdx, contents.diseaseCount);
		return Allocate(initial_data);
	}

	public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
	{
		Data data = GetData(handle);
		data.diseaseCount = contents.diseaseCount;
		if (contents.diseaseIdx != data.diseaseIdx)
		{
			data.diseaseIdx = contents.diseaseIdx;
			byte elem_idx = (byte)ElementLoader.GetElementIndex(contents.element);
			data.growthInfo = GetGrowthInfo(contents.diseaseIdx, elem_idx);
		}
		SetData(handle, data);
	}

	public void Sim200ms(float dt)
	{
		using (new KProfiler.Region("ConduitDiseaseManager.SimUpdate", null))
		{
			for (int i = 0; i < data.Count; i++)
			{
				Data value = data[i];
				if (value.diseaseIdx != 255)
				{
					float accumulatedError = value.accumulatedError;
					accumulatedError += value.growthInfo.CalculateDiseaseCountDelta(value.diseaseCount, value.mass, dt);
					Disease disease = Db.Get().Diseases[value.diseaseIdx];
					float temperature = temperatureManager.GetTemperature(value.temperatureHandle);
					float half_life_in_seconds = Disease.CalculateRangeHalfLife(temperature, ref disease.temperatureRange, ref disease.temperatureHalfLives);
					float num = Disease.HalfLifeToGrowthRate(half_life_in_seconds, dt);
					accumulatedError += (float)value.diseaseCount * num - (float)value.diseaseCount;
					int num2 = (int)accumulatedError;
					value.accumulatedError = accumulatedError - (float)num2;
					value.diseaseCount += num2;
					if (value.diseaseCount <= 0)
					{
						value.diseaseCount = 0;
						value.diseaseIdx = byte.MaxValue;
						value.accumulatedError = 0f;
					}
					data[i] = value;
				}
			}
		}
	}

	public void ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		Data data = GetData(h);
		data.diseaseCount = Math.Max(0, data.diseaseCount + disease_count_delta);
		if (data.diseaseCount == 0)
		{
			data.diseaseIdx = byte.MaxValue;
		}
		SetData(h, data);
	}

	public void AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		Data data = GetData(h);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, data.diseaseIdx, data.diseaseCount);
		data.diseaseIdx = diseaseInfo.idx;
		data.diseaseCount = diseaseInfo.count;
		SetData(h, data);
	}
}
