using STRINGS;
using UnityEngine;

public class CargoBayIsEmpty : RocketLaunchCondition
{
	private CommandModule commandModule;

	public CargoBayIsEmpty(CommandModule module)
	{
		commandModule = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override bool EvaluateLaunchCondition()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if ((Object)component != (Object)null && component.storage.MassStored() != 0f)
			{
				return false;
			}
		}
		return true;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.CARGOEMPTY.NAME;
		}
		return UI.STARMAP.CARGOEMPTY.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (!ready)
		{
			return UI.STARMAP.CARGOEMPTY.TOOLTIP;
		}
		return UI.STARMAP.CARGOEMPTY.TOOLTIP;
	}
}
