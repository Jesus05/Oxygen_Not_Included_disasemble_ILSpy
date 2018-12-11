using STRINGS;
using System.Collections.Generic;

public class ConditionHasAstronaut : RocketLaunchCondition
{
	private CommandModule module;

	public ConditionHasAstronaut(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override bool EvaluateLaunchCondition()
	{
		MinionStorage component = module.GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		int result;
		if (storedMinionInfo.Count > 0)
		{
			MinionStorage.Info info = storedMinionInfo[0];
			result = ((info.serializedMinion != null) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUT_TITLE;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.HASASTRONAUT;
	}
}
