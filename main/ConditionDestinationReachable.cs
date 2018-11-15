using STRINGS;

public class ConditionDestinationReachable : RocketLaunchCondition
{
	private CommandModule commandModule;

	public ConditionDestinationReachable(CommandModule module)
	{
		commandModule = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override bool EvaluateLaunchCondition()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination activeMission = SpacecraftManager.instance.GetActiveMission(id);
		return activeMission != null && CanReachDestination(activeMission);
	}

	public bool CanReachDestination(SpaceDestination destination)
	{
		float rocketMaxDistance = commandModule.rocketStats.GetRocketMaxDistance();
		return (float)destination.OneBasedDistance * 10000f <= rocketMaxDistance;
	}

	public SpaceDestination GetDestination()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		return SpacecraftManager.instance.GetActiveMission(id);
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready && GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
		}
		if (GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE;
		}
		return UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready && GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
		}
		if (GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE;
		}
		return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED;
	}
}
