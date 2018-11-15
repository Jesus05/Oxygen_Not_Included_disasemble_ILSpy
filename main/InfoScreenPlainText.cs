using UnityEngine;

public class InfoScreenPlainText : KMonoBehaviour
{
	[SerializeField]
	private LocText locText;

	public void SetText(string text)
	{
		locText.text = text;
	}
}
