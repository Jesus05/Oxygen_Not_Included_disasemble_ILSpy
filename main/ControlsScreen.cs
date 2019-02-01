using UnityEngine.UI;

public class ControlsScreen : KScreen
{
	public Text controlLabel;

	protected override void OnPrefabInit()
	{
		BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
		string text = "";
		BindingEntry[] array = bindingEntries;
		for (int i = 0; i < array.Length; i++)
		{
			BindingEntry bindingEntry = array[i];
			text += bindingEntry.mAction.ToString();
			text += ": ";
			text += bindingEntry.mKeyCode.ToString();
			text += "\n";
		}
		controlLabel.text = text;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Help) || e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
	}
}
