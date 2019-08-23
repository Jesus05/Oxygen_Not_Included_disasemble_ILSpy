using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RetiredColonyData
{
	public static class DataIDs
	{
		public static string OxygenProduced = "oxygenProduced";

		public static string OxygenConsumed = "oxygenConsumed";

		public static string CaloriesProduced = "caloriesProduced";

		public static string CaloriesRemoved = "caloriesRemoved";

		public static string PowerProduced = "powerProduced";

		public static string PowerWasted = "powerWasted";

		public static string WorkTime = "workTime";

		public static string TravelTime = "travelTime";

		public static string AverageWorkTime = "averageWorkTime";

		public static string AverageTravelTime = "averageTravelTime";

		public static string LiveDuplicants = "liveDuplicants";

		public static string AverageStressCreated = "averageStressCreated";

		public static string AverageStressRemoved = "averageStressRemoved";

		public static string DomesticatedCritters = "domesticatedCritters";

		public static string WildCritters = "wildCritters";

		public static string AverageGerms = "averageGerms";

		public static string RocketsInFlight = "rocketsInFlight";
	}

	public class RetiredColonyStatistic
	{
		public string id;

		public Tuple<float, float>[] value;

		public string name;

		public string nameX;

		public string nameY;

		public RetiredColonyStatistic()
		{
		}

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
			if (value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
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

		public Tuple<float, float> GetByMaxKey()
		{
			if (value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
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

	public RetiredColonyData()
	{
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
		Tuple<float, float>[] array12 = null;
		Tuple<float, float>[] array13 = null;
		Tuple<float, float>[] array14 = null;
		Tuple<float, float>[] array15 = null;
		Tuple<float, float>[] array16 = null;
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
				array2[l] = new Tuple<float, float>((float)ReportManager.Instance.reports[l].day, ReportManager.Instance.reports[l].GetEntry(ReportManager.ReportType.OxygenCreated).accNegative * -1f);
			}
			array3 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int m = 0; m < array3.Length; m++)
			{
				array3[m] = new Tuple<float, float>((float)ReportManager.Instance.reports[m].day, ReportManager.Instance.reports[m].GetEntry(ReportManager.ReportType.CaloriesCreated).accPositive * 0.001f);
			}
			array4 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int n = 0; n < array4.Length; n++)
			{
				array4[n] = new Tuple<float, float>((float)ReportManager.Instance.reports[n].day, ReportManager.Instance.reports[n].GetEntry(ReportManager.ReportType.CaloriesCreated).accNegative * 0.001f * -1f);
			}
			array5 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num2 = 0; num2 < array5.Length; num2++)
			{
				array5[num2] = new Tuple<float, float>((float)ReportManager.Instance.reports[num2].day, ReportManager.Instance.reports[num2].GetEntry(ReportManager.ReportType.EnergyCreated).accPositive * 0.001f);
			}
			array6 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num3 = 0; num3 < array6.Length; num3++)
			{
				array6[num3] = new Tuple<float, float>((float)ReportManager.Instance.reports[num3].day, ReportManager.Instance.reports[num3].GetEntry(ReportManager.ReportType.EnergyWasted).accNegative * -1f * 0.001f);
			}
			array7 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num4 = 0; num4 < array7.Length; num4++)
			{
				array7[num4] = new Tuple<float, float>((float)ReportManager.Instance.reports[num4].day, ReportManager.Instance.reports[num4].GetEntry(ReportManager.ReportType.WorkTime).accPositive);
			}
			array9 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num5 = 0; num5 < array7.Length; num5++)
			{
				int num6 = 0;
				float num7 = 0f;
				ReportManager.ReportEntry entry = ReportManager.Instance.reports[num5].GetEntry(ReportManager.ReportType.WorkTime);
				for (int num8 = 0; num8 < entry.contextEntries.Count; num8++)
				{
					num6++;
					num7 += entry.contextEntries[num8].accPositive;
				}
				num7 /= (float)num6;
				num7 /= 600f;
				num7 *= 100f;
				array9[num5] = new Tuple<float, float>((float)ReportManager.Instance.reports[num5].day, num7);
			}
			array8 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num9 = 0; num9 < array8.Length; num9++)
			{
				array8[num9] = new Tuple<float, float>((float)ReportManager.Instance.reports[num9].day, ReportManager.Instance.reports[num9].GetEntry(ReportManager.ReportType.TravelTime).accPositive);
			}
			array10 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num10 = 0; num10 < array8.Length; num10++)
			{
				int num11 = 0;
				float num12 = 0f;
				ReportManager.ReportEntry entry2 = ReportManager.Instance.reports[num10].GetEntry(ReportManager.ReportType.TravelTime);
				for (int num13 = 0; num13 < entry2.contextEntries.Count; num13++)
				{
					num11++;
					num12 += entry2.contextEntries[num13].accPositive;
				}
				num12 /= (float)num11;
				num12 /= 600f;
				num12 *= 100f;
				array10[num10] = new Tuple<float, float>((float)ReportManager.Instance.reports[num10].day, num12);
			}
			array11 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num14 = 0; num14 < array7.Length; num14++)
			{
				array11[num14] = new Tuple<float, float>((float)ReportManager.Instance.reports[num14].day, (float)ReportManager.Instance.reports[num14].GetEntry(ReportManager.ReportType.WorkTime).contextEntries.Count);
			}
			array12 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num15 = 0; num15 < array12.Length; num15++)
			{
				int num16 = 0;
				float num17 = 0f;
				ReportManager.ReportEntry entry3 = ReportManager.Instance.reports[num15].GetEntry(ReportManager.ReportType.StressDelta);
				for (int num18 = 0; num18 < entry3.contextEntries.Count; num18++)
				{
					num16++;
					num17 += entry3.contextEntries[num18].accPositive;
				}
				array12[num15] = new Tuple<float, float>((float)ReportManager.Instance.reports[num15].day, num17 / (float)num16);
			}
			array13 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num19 = 0; num19 < array13.Length; num19++)
			{
				int num20 = 0;
				float num21 = 0f;
				ReportManager.ReportEntry entry4 = ReportManager.Instance.reports[num19].GetEntry(ReportManager.ReportType.StressDelta);
				for (int num22 = 0; num22 < entry4.contextEntries.Count; num22++)
				{
					num20++;
					num21 += entry4.contextEntries[num22].accNegative;
				}
				num21 *= -1f;
				array13[num19] = new Tuple<float, float>((float)ReportManager.Instance.reports[num19].day, num21 / (float)num20);
			}
			array14 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num23 = 0; num23 < array14.Length; num23++)
			{
				array14[num23] = new Tuple<float, float>((float)ReportManager.Instance.reports[num23].day, ReportManager.Instance.reports[num23].GetEntry(ReportManager.ReportType.DomesticatedCritters).accPositive);
			}
			array15 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num24 = 0; num24 < array15.Length; num24++)
			{
				array15[num24] = new Tuple<float, float>((float)ReportManager.Instance.reports[num24].day, ReportManager.Instance.reports[num24].GetEntry(ReportManager.ReportType.WildCritters).accPositive);
			}
			array16 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num25 = 0; num25 < array16.Length; num25++)
			{
				array16[num25] = new Tuple<float, float>((float)ReportManager.Instance.reports[num25].day, ReportManager.Instance.reports[num25].GetEntry(ReportManager.ReportType.RocketsInFlight).accPositive);
			}
			Stats = new RetiredColonyStatistic[16]
			{
				new RetiredColonyStatistic(DataIDs.OxygenProduced, array, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
				new RetiredColonyStatistic(DataIDs.OxygenConsumed, array2, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CONSUMED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
				new RetiredColonyStatistic(DataIDs.CaloriesProduced, array3, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
				new RetiredColonyStatistic(DataIDs.CaloriesRemoved, array4, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CONSUMED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
				new RetiredColonyStatistic(DataIDs.PowerProduced, array5, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyStatistic(DataIDs.PowerWasted, array6, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_WASTED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyStatistic(DataIDs.WorkTime, array7, UI.RETIRED_COLONY_INFO_SCREEN.STATS.WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyStatistic(DataIDs.AverageWorkTime, array9, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.TravelTime, array8, UI.RETIRED_COLONY_INFO_SCREEN.STATS.TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyStatistic(DataIDs.AverageTravelTime, array10, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.LiveDuplicants, array11, UI.RETIRED_COLONY_INFO_SCREEN.STATS.LIVE_DUPLICANTS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.DUPLICANTS),
				new RetiredColonyStatistic(DataIDs.RocketsInFlight, array16, UI.RETIRED_COLONY_INFO_SCREEN.STATS.ROCKET_MISSIONS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ROCKET_MISSIONS),
				new RetiredColonyStatistic(DataIDs.AverageStressCreated, array12, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.AverageStressRemoved, array13, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_REMOVED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyStatistic(DataIDs.DomesticatedCritters, array14, UI.RETIRED_COLONY_INFO_SCREEN.STATS.NUMBER_DOMESTICATED_CRITTERS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CRITTERS),
				new RetiredColonyStatistic(DataIDs.WildCritters, array15, UI.RETIRED_COLONY_INFO_SCREEN.STATS.NUMBER_WILD_CRITTERS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CRITTERS)
			};
		}
	}
}
