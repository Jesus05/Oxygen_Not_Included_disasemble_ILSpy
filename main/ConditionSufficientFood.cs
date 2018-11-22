using STRINGS;

public class ConditionSufficientFood : RocketLaunchCondition
{
	private CommandModule module;

	public ConditionSufficientFood(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override bool EvaluateLaunchCondition()
	{
		return module.storage.GetAmountAvailable(GameTags.Edible) > 1f;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.NOFOOD.NAME;
		}
		return UI.STARMAP.HASFOOD.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.NOFOOD.TOOLTIP;
		}
		return UI.STARMAP.HASFOOD.TOOLTIP;
	}
}
