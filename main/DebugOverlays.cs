using System;

public class DebugOverlays : KScreen
{
	public static DebugOverlays instance
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		KPopupMenu componentInChildren = GetComponentInChildren<KPopupMenu>();
		componentInChildren.SetOptions(new string[5]
		{
			"None",
			"Rooms",
			"Lighting",
			"Style",
			"Flow"
		});
		KPopupMenu kPopupMenu = componentInChildren;
		kPopupMenu.OnSelect = (Action<string, int>)Delegate.Combine(kPopupMenu.OnSelect, new Action<string, int>(OnSelect));
		base.gameObject.SetActive(false);
	}

	private void OnSelect(string str, int index)
	{
		if (str != null)
		{
			if (str == "None")
			{
				SimDebugView.Instance.SetMode(OverlayModes.None.ID);
				return;
			}
			if (str == "Flow")
			{
				SimDebugView.Instance.SetMode(SimDebugView.OverlayModes.Flow);
				return;
			}
			if (str == "Lighting")
			{
				SimDebugView.Instance.SetMode(OverlayModes.Light.ID);
				return;
			}
			if (str == "Rooms")
			{
				SimDebugView.Instance.SetMode(OverlayModes.Rooms.ID);
				return;
			}
		}
		Debug.LogError("Unknown debug view: " + str, null);
	}
}
