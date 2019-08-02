using System;
using UnityEngine;

public class SimpleUIShowHide : KMonoBehaviour
{
	[MyCmpReq]
	private MultiToggle toggle;

	[SerializeField]
	public GameObject content;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = toggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(OnClick));
	}

	private void OnClick()
	{
		toggle.NextState();
		content.SetActive(toggle.CurrentState == 0);
	}
}
