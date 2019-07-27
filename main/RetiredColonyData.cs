using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RetiredColonyData
{
	public static class DataIDs
	{
		public static string OxygenProduced = "oxygenProduced";

		public static string CaloriesProduced = "caloriesProduced";

		public static string PowerProduced = "powerProduced";

		public static string PowerWasted = "powerWasted";

		public static string WorkTime = "workTime";

		public static string TravelTime = "travelTime";

		public static string AverageWorkTime = "averageWorkTime";

		public static string AverageTravelTime = "averageTravelTime";

		public static string LiveDuplicants = "liveDuplicants";

		public static string AverageStressCreated = "averageStressCreated";

		public static string AverageStressRemoved = "averageStressRemoved";

		public static string AverageGerms = "averageGerms";
	}

	public class RetiredColonyStatistic
	{
		public string id;

		public Tuple<float, float>[] value;

		public string name;

		public string nameX;

		public string nameY;

		public RetiredColonyStatistic(string id, Tuple<float, float>[] data, string name, string axisNameX, string axisNameY)
		{
			this.id = id;
			value = data;
			this.name = name;
			nameX = axisNameX;
			nameY = axisNameY;
		}

		public Tuple<float, float> GetByMaxValue()
		{
			if (value.Length != 0)
			{
				int num = -1;
				float num2 = -1f;
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i].second > num2)
					{
						num2 = value[i].second;
						num = i;
					}
				}
				if (num == -1)
				{
					num = 0;
				}
				return value[num];
			}
			return new Tuple<float, float>(0f, 0f);
		}

		public Tuple<float, float> GetByMaxKey()
		{
			if (value.Length != 0)
			{
				int num = -1;
				float num2 = -1f;
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i].first > num2)
					{
						num2 = value[i].first;
						num = i;
					}
				}
				return value[num];
			}
			return new Tuple<float, float>(0f, 0f);
		}
	}

	public class RetiredDuplicantData
	{
		public string name;

		public int age;

		public int skillPointsGained;

		public Dictionary<string, string> accessories;
	}

	public string colonyName
	{
		get;
		set;
	}

	public int cycleCount
	{
		get;
		set;
	}

	public string date
	{
		get;
		set;
	}

	public string[] achievements
	{
		get;
		set;
	}

	public RetiredDuplicantData[] Duplicants
	{
		get;
		set;
	}

	public List<Tuple<string, int>> buildings
	{
		get;
		set;
	}

	public RetiredColonyStatistic[] Stats
	{
		get;
		set;
	}

	public RetiredColonyData(string colonyName, int cycleCount, string date, string[] achievements, MinionAssignablesProxy[] minions, BuildingComplete[] buildingCompletes)
	{
		this.colonyName = colonyName;
		this.cycleCount = cycleCount;
		this.achievements = achievements;
		this.date = date;
		Duplicants = new RetiredDuplicantData[(minions != null) ? minions.Length : 0];
		for (int i = 0; i < Duplicants.Length; i++)
		{
			Duplicants[i] = new RetiredDuplicantData();
			Duplicants[i].name = minions[i].GetProperName();
			Duplicants[i].age = (int)Mathf.Floor((float)GameClock.Instance.GetCycle() - minions[i].GetArrivalTime());
			Duplicants[i].skillPointsGained = minions[i].GetTotalSkillpoints();
			Duplicants[i].accessories = new Dictionary<string, string>();
			if ((Object)minions[i].GetTargetGameObject().GetComponent<Accessorizer>() != (Object)null)
			{
				foreach (ResourceRef<Accessory> accessory in minions[i].GetTargetGameObject().GetComponent<Accessorizer>().GetAccessories())
				{
					if (accessory.Get() != null)
					{
						Duplicants[i].accessories.Add(accessory.Get().slot.Id, accessory.Get().Id);
					}
				}
			}
			else
			{
				StoredMinionIdentity component = minions[i].GetTargetGameObject().GetComponent<StoredMinionIdentity>();
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Eyes.Id, Db.Get().Accessories.Get(component.bodyData.eyes).Id);
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Arm.Id, Db.Get().Accessories.Get(component.bodyData.arms).Id);
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Body.Id, Db.Get().Accessories.Get(component.bodyData.body).Id);
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hair.Id, Db.Get().Accessories.Get(component.bodyData.hair).Id);
				if (component.bodyData.hat != HashedString.Invalid)
				{
					Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hat.Id, Db.Get().Accessories.Get(component.bodyData.hat).Id);
				}
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.HeadShape.Id, Db.Get().Accessories.Get(component.bodyData.headShape).Id);
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Mouth.Id, Db.Get().Accessories.Get(component.bodyData.mouth).Id);
			}
		}
		buildings = new List<Tuple<string, int>>();
		if (buildingCompletes != null)
		{
			foreach (BuildingComplete b in buildingCompletes)
			{
				int num = buildings.FindIndex((Tuple<string, int> match) => (Tag)match.first == b.PrefabID());
				if (num == -1)
				{
					buildings.Add(new Tuple<string, int>(b.PrefabID().ToString(), 0));
					num = buildings.Count - 1;
				}
				buildings[num].second++;
			}
		}
		Stats = null;
		Tuple<float, float>[] array = null;
		Tuple<float, float>[] array2 = null;
		Tuple<float, float>[] array3 = null;
		Tuple<float, float>[] array4 = null;
		Tuple<float, float>[] array5 = null;
		Tuple<float, float>[] array6 = null;
		Tuple<float, float>[] array7 = null;
		Tuple<float, float>[] array8 = null;
		Tuple<float, float>[] array9 = null;
		Tuple<float, float>[] array10 = null;
		Tuple<float, float>[] array11 = null;
		if ((Object)ReportManager.Instance != (Object)null)
		{
			array = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = new Tuple<float, float>((float)ReportManager.Instance.reports[k].day, ReportManager.Instance.reports[k].GetEntry(ReportManager.ReportType.OxygenCreated).accPositive);
			}
			array2 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l] = new Tuple<float, float>((float)ReportManager.Instance.reports[l].day, ReportManager.Instance.reports[l].GetEntry(ReportManager.ReportType.CaloriesCreated).accPositive * 0.001f);
			}
			array3 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int m = 0; m < array3.Length; m++)
			{
				array3[m] = new Tuple<float, float>((float)ReportManager.Instance.reports[m].day, ReportManager.Instance.reports[m].GetEntry(ReportManager.ReportType.EnergyCreated).accPositive * 0.001f);
			}
			array4 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int n = 0; n < array4.Length; n++)
			{
				array4[n] = new Tuple<float, float>((float)ReportManager.Instance.reports[n].day, ReportManager.Instance.reports[n].GetEntry(ReportManager.ReportType.EnergyWasted).accNegative * -1f * 0.001f);
			}
			array5 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num2 = 0; num2 < array5.Length; num2++)
			{
				array5[num2] = new Tuple<float, float>((float)ReportManager.Instance.reports[num2].day, ReportManager.Instance.reports[num2].GetEntry(ReportManager.ReportType.WorkTime).accPositive);
			}
			array7 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num3 = 0; num3 < array5.Length; num3++)
			{
				int num4 = 0;
				float num5 = 0f;
				ArrayRef<ReportManager.ReportEntry> contextEntries = ReportManager.Instance.reports[num3].GetEntry(ReportManager.ReportType.WorkTime).contextEntries;
				for (int num6 = 0; num6 < contextEntries.Count; num6++)
				{
					num4++;
					num5 += contextEntries[num6].accPositive;
				}
				num5 /= (float)num4;
				num5 /= 600f;
				array7[num3] = new Tuple<float, float>((float)ReportManager.Instance.reports[num3].day, num5);
			}
			array6 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num7 = 0; num7 < array6.Length; num7++)
			{
				array6[num7] = new Tuple<float, float>((float)ReportManager.Instance.reports[num7].day, ReportManager.Instance.reports[num7].GetEntry(ReportManager.ReportType.TravelTime).accPositive);
			}
			array8 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num8 = 0; num8 < array6.Length; num8++)
			{
				int num9 = 0;
				float num10 = 0f;
				ArrayRef<ReportManager.ReportEntry> contextEntries2 = ReportManager.Instance.reports[num8].GetEntry(ReportManager.ReportType.TravelTime).contextEntries;
				for (int num11 = 0; num11 < contextEntries2.Count; num11++)
				{
					num9++;
					num10 += contextEntries2[num11].accPositive;
				}
				num10 /= (float)num9;
				array8[num8] = new Tuple<float, float>((float)ReportManager.Instance.reports[num8].day, num10);
			}
			array9 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num12 = 0; num12 < array5.Length; num12++)
			{
				array9[num12] = new Tuple<float, float>((float)ReportManager.Instance.reports[num12].day, (float)ReportManager.Instance.reports[num12].GetEntry(ReportManager.ReportType.WorkTime).contextEntries.Count);
			}
			array10 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num13 = 0; num13 < array10.Length; num13++)
			{
				int num14 = 0;
				float num15 = 0f;
				ArrayRef<ReportManager.ReportEntry> contextEntries3 = ReportManager.Instance.reports[num13].GetEntry(ReportManager.ReportType.StressDelta).contextEntries;
				for (int num16 = 0; num16 < contextEntries3.Count; num16++)
				{
					num14++;
					num15 += contextEntries3[num16].accPositive;
				}
				array10[num13] = new Tuple<float, float>((float)ReportManager.Instance.reports[num13].day, num15 / (float)num14);
			}
			array11 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num17 = 0; num17 < array11.Length; num17++)
			{
				int num18 = 0;
				float num19 = 0f;
				ArrayRef<ReportManager.ReportEntry> contextEntries4 = ReportManager.Instance.reports[num17].GetEntry(ReportManager.ReportType.StressDelta).contextEntries;
				for (int num20 = 0; num20 < contextEntries4.Count; num20++)
				{
					num18++;
					num19 += contextEntries4[num20].accNegative;
				}
				num19 *= -1f;
				array11[num17] = new Tuple<float, float>((float)ReportManager.Instance.reports[num17].day, num19 / (float)num18);
			}
			Stats = new RetiredColonyStatistic[11]
			{
				new RetiredColonyStatistic(DataIDs.OxygenProduced, array, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
				new RetiredColonyStatistic(DataIDs.CaloriesProduced, array2, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
				new RetiredColonyStatistic(DataIDs.PowerProduced, array3, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyStatistic(DataIDs.PowerWasted, array4, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_WASTED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyStatistic(DataIDs.WorkTime, array5, UI.RETIRED_COLONY_INFO_SCREEN.STATS.WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyStatistic(DataIDs.AverageWorkTime, array7, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.TravelTime, array6, UI.RETIRED_COLONY_INFO_SCREEN.STATS.TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyStatistic(DataIDs.AverageTravelTime, array8, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.LiveDuplicants, array9, UI.RETIRED_COLONY_INFO_SCREEN.STATS.LIVE_DUPLICANTS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.DUPLICANTS),
				new RetiredColonyStatistic(DataIDs.AverageStressCreated, array10, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.AverageStressRemoved, array11, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_REMOVED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT)
			};
		}
	}
}
