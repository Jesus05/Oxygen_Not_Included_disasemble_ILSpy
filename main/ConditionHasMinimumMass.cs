using STRINGS;
using UnityEngine;

public class ConditionHasMinimumMass : RocketLaunchCondition
{
	private CommandModule commandModule;

	public ConditionHasMinimumMass(CommandModule command)
	{
		commandModule = command;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override LaunchStatus EvaluateLaunchCondition()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
		if (spacecraftDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete && spacecraftDestination.AvailableMass >= CargoCapacity(spacecraftDestination, commandModule))
		{
			return LaunchStatus.Ready;
		}
		return LaunchStatus.Warning;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
		if (spacecraftDestination != null)
		{
			if (SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				return string.Format(UI.STARMAP.LAUNCHCHECKLIST.MINIMUM_MASS, GameUtil.GetFormattedMass(spacecraftDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"));
			}
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.MINIMUM_MASS, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
		}
		return UI.STARMAP.LAUNCHCHECKLIST.NO_DESTINATION;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
		bool flag = spacecraftDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete;
		string text = string.Empty;
		if (flag)
		{
			if (spacecraftDestination.AvailableMass <= CargoCapacity(spacecraftDestination, commandModule))
			{
				text = text + UI.STARMAP.LAUNCHCHECKLIST.INSUFFICENT_MASS_TOOLTIP + UI.HORIZONTAL_BR_RULE;
			}
			text = text + string.Format(UI.STARMAP.LAUNCHCHECKLIST.RESOURCE_MASS_TOOLTIP, spacecraftDestination.GetDestinationType().Name, GameUtil.GetFormattedMass(spacecraftDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(CargoCapacity(spacecraftDestination, commandModule), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")) + "\n\n";
		}
		float num = spacecraftDestination?.AvailableMass ?? 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if ((Object)component != (Object)null)
			{
				if (flag)
				{
					float availableResourcesPercentage = spacecraftDestination.GetAvailableResourcesPercentage(component.storageType);
					float num2 = Mathf.Min(component.storage.Capacity(), availableResourcesPercentage * num);
					num -= num2;
					string text2 = text;
					text = text2 + component.gameObject.GetProperName() + " " + string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")) + "\n";
				}
				else
				{
					string text2 = text;
					text = text2 + component.gameObject.GetProperName() + " " + string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")) + "\n";
				}
			}
		}
		return text;
	}

	public static float CargoCapacity(SpaceDestination destination, CommandModule module)
	{
		if ((Object)module == (Object)null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(module.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if ((Object)component != (Object)null && destination.HasElementType(component.storageType))
			{
				Storage component2 = component.GetComponent<Storage>();
				num += component2.capacityKg;
			}
		}
		return num;
	}
}
