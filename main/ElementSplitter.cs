using System;
using UnityEngine;

public struct ElementSplitter
{
	public PrimaryElement primaryElement;

	public Func<float, Pickupable> onTakeCB;

	public Func<Pickupable, bool> canAbsorbCB;

	public ElementSplitter(GameObject go)
	{
		primaryElement = go.GetComponent<PrimaryElement>();
		onTakeCB = null;
		canAbsorbCB = null;
	}
}
