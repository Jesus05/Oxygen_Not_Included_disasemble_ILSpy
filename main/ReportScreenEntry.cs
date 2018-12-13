using System;
using System.Collections.Generic;
using UnityEngine;

public class ReportScreenEntry : KMonoBehaviour
{
	[SerializeField]
	private ReportScreenEntryRow rowTemplate;

	private ReportScreenEntryRow mainRow;

	private List<ReportScreenEntryRow> contextRows = new List<ReportScreenEntryRow>();

	private int currentContextCount;

	public void SetMainEntry(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		if ((UnityEngine.Object)mainRow == (UnityEngine.Object)null)
		{
			mainRow = Util.KInstantiateUI(rowTemplate.gameObject, base.gameObject, true).GetComponent<ReportScreenEntryRow>();
			MultiToggle toggle = mainRow.toggle;
			toggle.onClick = (System.Action)Delegate.Combine(toggle.onClick, new System.Action(ToggleContext));
			MultiToggle componentInChildren = mainRow.name.GetComponentInChildren<MultiToggle>();
			componentInChildren.onClick = (System.Action)Delegate.Combine(componentInChildren.onClick, new System.Action(ToggleContext));
			MultiToggle componentInChildren2 = mainRow.added.GetComponentInChildren<MultiToggle>();
			componentInChildren2.onClick = (System.Action)Delegate.Combine(componentInChildren2.onClick, new System.Action(ToggleContext));
			MultiToggle componentInChildren3 = mainRow.removed.GetComponentInChildren<MultiToggle>();
			componentInChildren3.onClick = (System.Action)Delegate.Combine(componentInChildren3.onClick, new System.Action(ToggleContext));
			MultiToggle componentInChildren4 = mainRow.net.GetComponentInChildren<MultiToggle>();
			componentInChildren4.onClick = (System.Action)Delegate.Combine(componentInChildren4.onClick, new System.Action(ToggleContext));
		}
		mainRow.SetLine(entry, reportGroup);
		currentContextCount = entry.contextEntries.Count;
		for (int i = 0; i < entry.contextEntries.Count; i++)
		{
			if (i >= contextRows.Count)
			{
				ReportScreenEntryRow component = Util.KInstantiateUI(rowTemplate.gameObject, base.gameObject, false).GetComponent<ReportScreenEntryRow>();
				contextRows.Add(component);
			}
			contextRows[i].SetLine(entry.contextEntries[i], reportGroup);
		}
		UpdateVisibility();
	}

	private void ToggleContext()
	{
		mainRow.toggle.NextState();
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		int i;
		for (i = 0; i < currentContextCount; i++)
		{
			contextRows[i].gameObject.SetActive(mainRow.toggle.CurrentState == 1);
		}
		for (; i < contextRows.Count; i++)
		{
			contextRows[i].gameObject.SetActive(false);
		}
	}
}
