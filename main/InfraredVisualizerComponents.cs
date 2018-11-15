using UnityEngine;

public class InfraredVisualizerComponents : KGameObjectComponentManager<InfraredVisualizerData>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return Add(go, new InfraredVisualizerData(go));
	}

	public void UpdateTemperature()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		for (int i = 0; i < data.Count; i++)
		{
			InfraredVisualizerData infraredVisualizerData = data[i];
			KAnimControllerBase controller = infraredVisualizerData.controller;
			if ((Object)controller != (Object)null)
			{
				Vector3 position = controller.transform.GetPosition();
				if (visibleArea.Min <= (Vector2)position && (Vector2)position <= visibleArea.Max)
				{
					data[i].Update();
				}
			}
		}
	}

	public void ClearOverlayColour()
	{
		Color32 c = Color.black;
		for (int i = 0; i < data.Count; i++)
		{
			InfraredVisualizerData infraredVisualizerData = data[i];
			KAnimControllerBase controller = infraredVisualizerData.controller;
			if ((Object)controller != (Object)null)
			{
				controller.OverlayColour = c;
			}
		}
	}

	public static void ClearOverlayColour(KBatchedAnimController controller)
	{
		if ((Object)controller != (Object)null)
		{
			controller.OverlayColour = Color.black;
		}
	}
}
