using System;
using System.Collections.Generic;
using UnityEngine;

public class KPopupMenu : KScreen
{
	[SerializeField]
	private KButtonMenu buttonMenu;

	private KButtonMenu.ButtonInfo[] Buttons;

	public Action<string, int> OnSelect;

	public void SetOptions(IList<string> options)
	{
		List<KButtonMenu.ButtonInfo> list = new List<KButtonMenu.ButtonInfo>();
		for (int i = 0; i < options.Count; i++)
		{
			int index = i;
			string option = options[i];
			list.Add(new KButtonMenu.ButtonInfo(option, Action.NumActions, delegate
			{
				SelectOption(option, index);
			}, null, null));
		}
		Buttons = list.ToArray();
	}

	public void OnClick()
	{
		if (Buttons != null)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				buttonMenu.SetButtons(Buttons);
				buttonMenu.RefreshButtons();
				base.gameObject.SetActive(true);
			}
		}
	}

	public void SelectOption(string option, int index)
	{
		if (OnSelect != null)
		{
			OnSelect(option, index);
		}
		base.gameObject.SetActive(false);
	}

	public IList<KButtonMenu.ButtonInfo> GetButtons()
	{
		return Buttons;
	}
}
