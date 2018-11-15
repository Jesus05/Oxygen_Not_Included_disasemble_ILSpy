using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RequireAttachedComponent : RocketLaunchCondition
{
	private string typeNameString;

	private Type requiredType;

	private AttachableBuilding myAttachable;

	public Type RequiredType
	{
		get
		{
			return requiredType;
		}
		set
		{
			requiredType = value;
			typeNameString = requiredType.Name;
		}
	}

	public RequireAttachedComponent(AttachableBuilding myAttachable, Type required_type, string type_name_string)
	{
		this.myAttachable = myAttachable;
		requiredType = required_type;
		typeNameString = type_name_string;
	}

	public override bool EvaluateLaunchCondition()
	{
		if ((UnityEngine.Object)myAttachable != (UnityEngine.Object)null)
		{
			foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(myAttachable))
			{
				if ((bool)item.GetComponent(requiredType))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return typeNameString + " " + UI.STARMAP.LAUNCHCHECKLIST.REQUIRED;
		}
		return typeNameString + " " + UI.STARMAP.LAUNCHCHECKLIST.INSTALLED;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.INSTALLED_TOOLTIP, typeNameString);
		}
		return string.Format(UI.STARMAP.LAUNCHCHECKLIST.REQUIRED_TOOLTIP, typeNameString);
	}
}
