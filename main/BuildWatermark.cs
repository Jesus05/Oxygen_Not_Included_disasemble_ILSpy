using STRINGS;
using UnityEngine;

public class BuildWatermark : KScreen
{
	public LocText textDisplay;

	public static BuildWatermark Instance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		string arg = "LU-" + ((!Application.isEditor) ? 352488.ToString() : "<EDITOR>");
		textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.TESTING_WATERMARK, arg));
	}

	private void Update()
	{
		if (base.transform.GetSiblingIndex() != base.transform.parent.childCount - 1)
		{
			base.transform.SetAsLastSibling();
		}
	}
}
