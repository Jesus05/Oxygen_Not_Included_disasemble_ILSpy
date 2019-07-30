using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RocketStats
{
	private CommandModule commandModule;

	private static Dictionary<Tag, float> oxidizerEfficiencies;

	public RocketStats(CommandModule commandModule)
	{
		this.commandModule = commandModule;
		if (oxidizerEfficiencies == null)
		{
			oxidizerEfficiencies = new Dictionary<Tag, float>
			{
				{
					SimHashes.OxyRock.CreateTag(),
					ROCKETRY.OXIDIZER_EFFICIENCY.LOW
				},
				{
					SimHashes.LiquidOxygen.CreateTag(),
					ROCKETRY.OXIDIZER_EFFICIENCY.HIGH
				}
			};
		}
	}

	public float GetRocketMaxDistance()
	{
		float totalMass = GetTotalMass();
		float totalThrust = GetTotalThrust();
		float num = ROCKETRY.CalculateMassWithPenalty(totalMass);
		return Mathf.Max(0f, totalThrust - num);
	}

	public float GetTotalMass()
	{
		return GetDryMass() + GetWetMass();
	}

	public float GetDryMass()
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if ((Object)component != (Object)null)
			{
				num += component.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public float GetWetMass()
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if ((Object)component != (Object)null)
			{
				FuelTank component2 = component.GetComponent<FuelTank>();
				OxidizerTank component3 = component.GetComponent<OxidizerTank>();
				SolidBooster component4 = component.GetComponent<SolidBooster>();
				if ((Object)component2 != (Object)null)
				{
					num += component2.MassStored();
				}
				if ((Object)component3 != (Object)null)
				{
					num += component3.MassStored();
				}
				if ((Object)component4 != (Object)null)
				{
					num += component4.fuelStorage.MassStored();
				}
			}
		}
		return num;
	}

	public Tag GetEngineFuelTag()
	{
		RocketEngine mainEngine = GetMainEngine();
		if ((Object)mainEngine != (Object)null)
		{
			return mainEngine.fuelTag;
		}
		return null;
	}

	public float GetTotalFuel(bool includeBoosters = false)
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			FuelTank component = item.GetComponent<FuelTank>();
			Tag engineFuelTag = GetEngineFuelTag();
			if ((Object)component != (Object)null)
			{
				num += component.GetAmountAvailable(engineFuelTag);
			}
			if (includeBoosters)
			{
				SolidBooster component2 = item.GetComponent<SolidBooster>();
				if ((Object)component2 != (Object)null)
				{
					num += component2.fuelStorage.GetAmountAvailable(component2.fuelTag);
				}
			}
		}
		return num;
	}

	public float GetTotalOxidizer(bool includeBoosters = false)
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = item.GetComponent<OxidizerTank>();
			if ((Object)component != (Object)null)
			{
				num += component.GetTotalOxidizerAvailable();
			}
			if (includeBoosters)
			{
				SolidBooster component2 = item.GetComponent<SolidBooster>();
				if ((Object)component2 != (Object)null)
				{
					num += component2.fuelStorage.GetAmountAvailable(GameTags.OxyRock);
				}
			}
		}
		return num;
	}

	public float GetAverageOxidizerEfficiency()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		dictionary[SimHashes.LiquidOxygen.CreateTag()] = 0f;
		dictionary[SimHashes.OxyRock.CreateTag()] = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = item.GetComponent<OxidizerTank>();
			if ((Object)component != (Object)null)
			{
				Dictionary<Tag, float> oxidizersAvailable = component.GetOxidizersAvailable();
				foreach (KeyValuePair<Tag, float> item2 in oxidizersAvailable)
				{
					if (dictionary.ContainsKey(item2.Key))
					{
						Dictionary<Tag, float> dictionary2;
						Tag key;
						(dictionary2 = dictionary)[key = item2.Key] = dictionary2[key] + item2.Value;
					}
				}
			}
		}
		float num = 0f;
		float num2 = 0f;
		foreach (KeyValuePair<Tag, float> item3 in dictionary)
		{
			num += item3.Value * oxidizerEfficiencies[item3.Key];
			num2 += item3.Value;
		}
		if (num2 == 0f)
		{
			return 0f;
		}
		float num3 = num / num2;
		return num3 * 100f;
	}

	public float GetTotalThrust()
	{
		float num = 0f;
		float totalFuel = GetTotalFuel(false);
		float totalOxidizer = GetTotalOxidizer(false);
		float averageOxidizerEfficiency = GetAverageOxidizerEfficiency();
		RocketEngine mainEngine = GetMainEngine();
		if ((Object)mainEngine == (Object)null)
		{
			return 0f;
		}
		num = ((!mainEngine.requireOxidizer) ? (totalFuel * mainEngine.efficiency) : (Mathf.Min(totalFuel, totalOxidizer) * (mainEngine.efficiency * (averageOxidizerEfficiency / 100f))));
		return num + GetBoosterThrust();
	}

	public float GetBoosterThrust()
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			SolidBooster component = item.GetComponent<SolidBooster>();
			if ((Object)component != (Object)null)
			{
				float amountAvailable = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag);
				float amountAvailable2 = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.Iron).tag);
				num += component.efficiency * Mathf.Min(amountAvailable, amountAvailable2);
			}
		}
		return num;
	}

	public float GetEngineEfficiency()
	{
		RocketEngine mainEngine = GetMainEngine();
		if ((Object)mainEngine != (Object)null)
		{
			return mainEngine.efficiency;
		}
		return 0f;
	}

	public RocketEngine GetMainEngine()
	{
		RocketEngine rocketEngine = null;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			rocketEngine = item.GetComponent<RocketEngine>();
			if ((Object)rocketEngine != (Object)null && rocketEngine.mainEngine)
			{
				return rocketEngine;
			}
		}
		return rocketEngine;
	}

	public float GetTotalOxidizableFuel()
	{
		float totalFuel = GetTotalFuel(false);
		float totalOxidizer = GetTotalOxidizer(false);
		return Mathf.Min(totalFuel, totalOxidizer);
	}
}
