using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
[DebuggerDisplay("{Id}")]
public class AssignableSlot : Resource
{
	public bool showInUI = true;

	public AssignableSlot(string id, string name, bool showInUI = true)
		: base(id, name)
	{
		this.showInUI = showInUI;
	}

	public AssignableSlotInstance Lookup(GameObject go)
	{
		Assignables component = go.GetComponent<Assignables>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.GetSlot(this);
		}
		return null;
	}
}
