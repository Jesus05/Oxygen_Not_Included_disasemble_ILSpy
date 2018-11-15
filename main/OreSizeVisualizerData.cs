using System;
using UnityEngine;

public struct OreSizeVisualizerData
{
	public PrimaryElement primaryElement;

	public Action<object> onMassChangedCB;

	public OreSizeVisualizerData(GameObject go)
	{
		primaryElement = go.GetComponent<PrimaryElement>();
		onMassChangedCB = null;
	}
}
