#define STRICT_CHECKING
using Klei.AI;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei
{
	public static class SimUtil
	{
		public struct DiseaseInfo
		{
			public byte idx;

			public int count;

			public static readonly DiseaseInfo Invalid = new DiseaseInfo
			{
				idx = 255,
				count = 0
			};
		}

		private const int MAX_ALPHA_COUNT = 1000000;

		private static float MIN_DISEASE_LOG_SUBTRACTION = 2f;

		private static float MAX_DISEASE_LOG_RANGE = 6f;

		public static float CalculateEnergyFlow(float source_temp, float source_thermal_conductivity, float dest_temp, float dest_thermal_conductivity, float surface_area = 1f, float thickness = 1f)
		{
			float num = source_temp - dest_temp;
			return num * Math.Min(source_thermal_conductivity, dest_thermal_conductivity) * (surface_area / thickness);
		}

		public static float CalculateEnergyFlow(int cell, float dest_temp, float dest_specific_heat_capacity, float dest_thermal_conductivity, float surface_area = 1f, float thickness = 1f)
		{
			float num = Grid.Mass[cell];
			if (num <= 0f)
			{
				return 0f;
			}
			Element element = Grid.Element[cell];
			if (element.IsVacuum)
			{
				return 0f;
			}
			float source_temp = Grid.Temperature[cell];
			float thermalConductivity = element.thermalConductivity;
			float num2 = CalculateEnergyFlow(source_temp, thermalConductivity, dest_temp, dest_thermal_conductivity, surface_area, thickness);
			return num2 * 0.001f;
		}

		public static float ClampEnergyTransfer(float dt, float source_temp, float source_mass, float source_specific_heat_capacity, float dest_temp, float dest_mass, float dest_specific_heat_capacity, float max_watts_transferred)
		{
			return ClampEnergyTransfer(dt, source_temp, source_mass * source_specific_heat_capacity, dest_temp, dest_mass * dest_specific_heat_capacity, max_watts_transferred);
		}

		public static float ClampEnergyTransfer(float dt, float source_temp, float source_heat_capacity, float dest_temp, float dest_heat_capacity, float max_watts_transferred)
		{
			float num = max_watts_transferred * dt / 1000f;
			CheckValidValue(num);
			float min = Math.Min(source_temp, dest_temp);
			float max = Math.Max(source_temp, dest_temp);
			float value = source_temp - num / source_heat_capacity;
			float value2 = dest_temp + num / dest_heat_capacity;
			CheckValidValue(value);
			CheckValidValue(value2);
			value = Mathf.Clamp(value, min, max);
			value2 = Mathf.Clamp(value2, min, max);
			float num2 = Math.Abs(value - source_temp);
			float num3 = Math.Abs(value2 - dest_temp);
			float val = num2 * source_heat_capacity;
			float val2 = num3 * dest_heat_capacity;
			float num4 = (!(max_watts_transferred < 0f)) ? 1f : (-1f);
			float num5 = Math.Min(val, val2) * num4;
			CheckValidValue(num5);
			return num5;
		}

		private static float GetMassAreaScale(Element element)
		{
			return (!element.IsGas) ? 0.01f : 10f;
		}

		public static float CalculateEnergyFlowCreatures(int cell, float creature_temperature, float creature_shc, float creature_thermal_conductivity, float creature_surface_area = 1f, float creature_surface_thickness = 1f)
		{
			return CalculateEnergyFlow(cell, creature_temperature, creature_shc, creature_thermal_conductivity, creature_surface_area, creature_surface_thickness);
		}

		public static float EnergyFlowToTemperatureDelta(float kilojoules, float specific_heat_capacity, float mass)
		{
			if (kilojoules * specific_heat_capacity * mass == 0f)
			{
				return 0f;
			}
			return kilojoules / (specific_heat_capacity * mass);
		}

		public static float CalculateFinalTemperature(float mass1, float temp1, float mass2, float temp2)
		{
			float num = mass1 * temp1;
			float num2 = mass2 * temp2;
			float num3 = num + num2;
			float val = num3 / (mass1 + mass2);
			float val2;
			float val3;
			if (temp1 > temp2)
			{
				val2 = temp2;
				val3 = temp1;
			}
			else
			{
				val2 = temp1;
				val3 = temp2;
			}
			return Math.Max(val2, Math.Min(val3, val));
		}

		[Conditional("STRICT_CHECKING")]
		public static void CheckValidValue(float value)
		{
			if (!float.IsNaN(value) && !float.IsInfinity(value))
			{
				return;
			}
		}

		public static DiseaseInfo CalculateFinalDiseaseInfo(DiseaseInfo a, DiseaseInfo b)
		{
			return CalculateFinalDiseaseInfo(a.idx, a.count, b.idx, b.count);
		}

		public static DiseaseInfo CalculateFinalDiseaseInfo(byte src1_idx, int src1_count, byte src2_idx, int src2_count)
		{
			DiseaseInfo result = default(DiseaseInfo);
			if (src1_idx == src2_idx)
			{
				result.idx = src1_idx;
				result.count = src1_count + src2_count;
			}
			else if (src1_idx == 255)
			{
				result.idx = src2_idx;
				result.count = src2_count;
			}
			else if (src2_idx == 255)
			{
				result.idx = src1_idx;
				result.count = src1_count;
			}
			else
			{
				Disease disease = Db.Get().Diseases[src1_idx];
				Disease disease2 = Db.Get().Diseases[src2_idx];
				float num = disease.strength * (float)src1_count;
				float num2 = disease2.strength * (float)src2_count;
				if (num > num2)
				{
					int num3 = (int)((float)src2_count - num / num2 * (float)src1_count);
					if (num3 < 0)
					{
						result.idx = src1_idx;
						result.count = -num3;
					}
					else
					{
						result.idx = src2_idx;
						result.count = num3;
					}
				}
				else
				{
					int num4 = (int)((float)src1_count - num2 / num * (float)src2_count);
					if (num4 < 0)
					{
						result.idx = src2_idx;
						result.count = -num4;
					}
					else
					{
						result.idx = src1_idx;
						result.count = num4;
					}
				}
			}
			if (result.count <= 0)
			{
				result.count = 0;
				result.idx = byte.MaxValue;
			}
			return result;
		}

		public static byte DiseaseCountToAlpha254(int count)
		{
			float num = Mathf.Log((float)count, 10f);
			num /= MAX_DISEASE_LOG_RANGE;
			num = Math.Max(0f, Math.Min(1f, num));
			num -= MIN_DISEASE_LOG_SUBTRACTION / MAX_DISEASE_LOG_RANGE;
			num = Math.Max(0f, num);
			num /= 1f - MIN_DISEASE_LOG_SUBTRACTION / MAX_DISEASE_LOG_RANGE;
			return (byte)(num * 254f);
		}

		public static float DiseaseCountToAlpha(int count)
		{
			return (float)(int)DiseaseCountToAlpha254(count) / 255f;
		}

		public static DiseaseInfo GetPercentOfDisease(PrimaryElement pe, float percent)
		{
			DiseaseInfo result = default(DiseaseInfo);
			result.idx = pe.DiseaseIdx;
			result.count = (int)((float)pe.DiseaseCount * percent);
			return result;
		}
	}
}
