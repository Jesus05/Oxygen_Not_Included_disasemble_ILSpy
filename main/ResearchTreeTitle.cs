using UnityEngine;
using UnityEngine.UI;

public class ResearchTreeTitle : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private LocText treeLabel;

	[SerializeField]
	private Image BG;

	public void SetLabel(string txt)
	{
		treeLabel.text = txt;
	}

	public void SetColor(int id)
	{
		BG.enabled = ((id % 2 != 0) ? true : false);
	}
}
