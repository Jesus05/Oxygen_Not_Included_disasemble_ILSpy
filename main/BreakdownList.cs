using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownList : KMonoBehaviour
{
	public Image headerIcon;

	public Sprite headerIconSprite;

	public Image headerBar;

	public LocText headerTitle;

	public LocText headerValue;

	public LocText infoTextLabel;

	public BreakdownListRow listRowTemplate;

	private List<BreakdownListRow> listRows = new List<BreakdownListRow>();

	private List<BreakdownListRow> unusedListRows = new List<BreakdownListRow>();

	private List<GameObject> customRows = new List<GameObject>();

	public BreakdownListRow AddRow()
	{
		BreakdownListRow breakdownListRow;
		if (unusedListRows.Count > 0)
		{
			breakdownListRow = unusedListRows[0];
			unusedListRows.RemoveAt(0);
		}
		else
		{
			breakdownListRow = Object.Instantiate(listRowTemplate);
		}
		breakdownListRow.gameObject.transform.SetParent(base.transform);
		breakdownListRow.gameObject.transform.SetAsLastSibling();
		listRows.Add(breakdownListRow);
		breakdownListRow.gameObject.SetActive(true);
		return breakdownListRow;
	}

	public GameObject AddCustomRow(GameObject newRow)
	{
		newRow.transform.SetParent(base.transform);
		newRow.gameObject.transform.SetAsLastSibling();
		customRows.Add(newRow);
		newRow.SetActive(true);
		return newRow;
	}

	public void ClearRows()
	{
		foreach (BreakdownListRow listRow in listRows)
		{
			unusedListRows.Add(listRow);
			listRow.gameObject.SetActive(false);
			listRow.ClearTooltip();
		}
		listRows.Clear();
		foreach (GameObject customRow in customRows)
		{
			customRow.SetActive(false);
		}
	}

	public void SetTitle(string title)
	{
		headerTitle.text = title;
	}

	public void SetDescription(string description)
	{
		if (description != null && description.Length >= 0)
		{
			infoTextLabel.gameObject.SetActive(true);
			infoTextLabel.text = description;
		}
		else
		{
			infoTextLabel.gameObject.SetActive(false);
		}
	}

	public void SetIcon(Sprite icon)
	{
		headerIcon.sprite = icon;
	}
}
