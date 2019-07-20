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

	public override LaunchStatus EvaluateLaunchCondition()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
		if (spacecraftDestination != null && CanReachDestination(spacecraftDestination) && spacecraftDestination.GetDestinationType().visitable)
		{
			return LaunchStatus.Ready;
		}
		return LaunchStatus.Failure;
	}

	public bool CanReachDestination(SpaceDestination destination)
	{
		float rocketMaxDistance = commandModule.rocketStats.GetRocketMaxDistance();
		return (float)destination.OneBasedDistance * 10000f <= rocketMaxDistance;
	}

	public SpaceDestination GetDestination()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).id;
		return SpacecraftManager.instance.GetSpacecraftDestination(id);
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready && GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
		}
		if (GetDestination() == null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED;
		}
		return UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready && GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
		}
		if (GetDestination() == null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED;
		}
		return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE;
	}
}
