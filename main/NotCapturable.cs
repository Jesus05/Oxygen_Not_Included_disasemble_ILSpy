using UnityEngine;

public class NotCapturable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if ((Object)GetComponent<Capturable>() != (Object)null)
		{
			Output.LogErrorWithObj(this, "Entity has both Capturable and NotCapturable!");
		}
		Components.NotCapturables.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.NotCapturables.Remove(this);
		base.OnCleanUp();
	}
}
