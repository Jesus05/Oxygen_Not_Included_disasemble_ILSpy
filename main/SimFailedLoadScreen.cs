using UnityEngine;
using UnityEngine.UI;

internal class SimFailedLoadScreen : KScreen
{
	[SerializeField]
	private Button okButton;

	[SerializeField]
	private LocText bodyText;

	private bool IsRuntimeInstalled()
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		bodyText.key = "STRINGS.UI.FRONTEND.MINSPECSCREEN.SIMFAILEDTOLOAD";
		okButton.onClick.AddListener(OnClickQuit);
		if (IsRuntimeInstalled())
		{
			Deactivate();
		}
	}

	private void OnClickQuit()
	{
		Deactivate();
	}

	protected override void OnActivate()
	{
		if (IsRuntimeInstalled())
		{
			Deactivate();
		}
	}
}
