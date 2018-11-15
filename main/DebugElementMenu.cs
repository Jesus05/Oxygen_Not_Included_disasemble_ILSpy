using UnityEngine;

public class DebugElementMenu : KButtonMenu
{
	public static DebugElementMenu Instance;

	public GameObject root;

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
	}

	public void Turnoff()
	{
		root.gameObject.SetActive(false);
	}
}
