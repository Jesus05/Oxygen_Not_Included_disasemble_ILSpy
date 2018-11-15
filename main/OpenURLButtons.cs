using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLButtons : KMonoBehaviour
{
	public enum URLButtonType
	{
		url,
		patchNotes
	}

	[Serializable]
	public class URLButtonData
	{
		public string stringKey;

		public URLButtonType urlType;

		public string url;
	}

	public GameObject buttonPrefab;

	public List<URLButtonData> buttonData;

	[SerializeField]
	private GameObject patchNotesScreen;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < buttonData.Count; i++)
		{
			URLButtonData data = buttonData[i];
			GameObject gameObject = Util.KInstantiateUI(buttonPrefab, base.gameObject, true);
			string text = Strings.Get(data.stringKey);
			gameObject.GetComponentInChildren<LocText>().SetText(text);
			switch (data.urlType)
			{
			case URLButtonType.patchNotes:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenPatchNotes();
				};
				break;
			case URLButtonType.url:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenURL(data.url);
				};
				break;
			}
		}
	}

	public void OpenPatchNotes()
	{
		patchNotesScreen.SetActive(true);
	}

	public void OpenURL(string URL)
	{
		Application.OpenURL(URL);
	}
}
