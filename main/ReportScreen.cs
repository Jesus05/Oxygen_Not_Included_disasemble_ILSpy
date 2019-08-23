using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ReportScreen : KScreen
{
	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton prevButton;

	[SerializeField]
	private KButton nextButton;

	[SerializeField]
	private KButton summaryButton;

	[SerializeField]
	private GameObject lineItem;

	[SerializeField]
	private GameObject lineItemSpacer;

	[SerializeField]
	private GameObject lineItemHeader;

	[SerializeField]
	private GameObject contentFolder;

	private Dictionary<string, GameObject> lineItems = new Dictionary<string, GameObject>();

	private ReportManager.DailyReport currentReport;

	public static ReportScreen Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		prevButton.onClick += delegate
		{
			ShowReport(currentReport.day - 1);
		};
		nextButton.onClick += delegate
		{
			ShowReport(currentReport.day + 1);
		};
		summaryButton.onClick += delegate
		{
			RetiredColonyData currentColonyRetiredColonyData = RetireColonyUtility.GetCurrentColonyRetiredColonyData();
			MainMenu.ActivateRetiredColoniesScreenFromData(PauseScreen.Instance.transform.parent.gameObject, currentColonyRetiredColonyData);
		};
		ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnShow(bool bShow)
	{
		base.OnShow(bShow);
		if ((Object)ReportManager.Instance != (Object)null)
		{
			currentReport = ReportManager.Instance.TodaysReport;
		}
	}

	public void SetTitle(string title)
	{
		this.title.text = title;
	}

	public override void ScreenUpdate(bool b)
	{
		base.ScreenUpdate(b);
		Refresh();
	}

	private void Refresh()
	{
		Debug.Assert(currentReport != null);
		if (currentReport.day == ReportManager.Instance.TodaysReport.day)
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_TODAY, currentReport.day));
		}
		else if (currentReport.day == ReportManager.Instance.TodaysReport.day - 1)
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_YESTERDAY, currentReport.day));
		}
		else
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day));
		}
		bool flag = currentReport.day < ReportManager.Instance.TodaysReport.day;
		nextButton.isInteractable = flag;
		if (flag)
		{
			nextButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day + 1);
			nextButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			nextButton.GetComponent<ToolTip>().enabled = false;
		}
		flag = (currentReport.day > 1);
		prevButton.isInteractable = flag;
		if (flag)
		{
			prevButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day - 1);
			prevButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			prevButton.GetComponent<ToolTip>().enabled = false;
		}
		AddSpacer(0);
		int num = 1;
		foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> reportGroup in ReportManager.Instance.ReportGroups)
		{
			ReportManager.ReportEntry entry = currentReport.GetEntry(reportGroup.Key);
			int num2 = num;
			ReportManager.ReportGroup value = reportGroup.Value;
			if (num2 != value.group)
			{
				ReportManager.ReportGroup value2 = reportGroup.Value;
				num = value2.group;
				AddSpacer(num);
			}
			int num3;
			if (entry.accumulate == 0f)
			{
				ReportManager.ReportGroup value3 = reportGroup.Value;
				num3 = (value3.reportIfZero ? 1 : 0);
			}
			else
			{
				num3 = 1;
			}
			bool flag2 = (byte)num3 != 0;
			ReportManager.ReportGroup value4 = reportGroup.Value;
			if (value4.isHeader)
			{
				CreateHeader(reportGroup.Value);
			}
			else if (flag2)
			{
				CreateOrUpdateLine(entry, reportGroup.Value, flag2);
			}
		}
	}

	public void ShowReport(int day)
	{
		currentReport = ReportManager.Instance.FindReport(day);
		Debug.Assert(currentReport != null, "Can't find report for day: " + day.ToString());
		Refresh();
	}

	private GameObject AddSpacer(int group)
	{
		GameObject gameObject = null;
		if (lineItems.ContainsKey(group.ToString()))
		{
			gameObject = lineItems[group.ToString()];
		}
		else
		{
			gameObject = Util.KInstantiateUI(lineItemSpacer, contentFolder, false);
			gameObject.name = "Spacer" + group.ToString();
			lineItems[group.ToString()] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private GameObject CreateHeader(ReportManager.ReportGroup reportGroup)
	{
		GameObject value = null;
		lineItems.TryGetValue(reportGroup.stringKey, out value);
		if ((Object)value == (Object)null)
		{
			value = Util.KInstantiateUI(lineItemHeader, contentFolder, true);
			value.name = "LineItemHeader" + lineItems.Count;
			lineItems[reportGroup.stringKey] = value;
		}
		value.SetActive(true);
		ReportScreenHeader component = value.GetComponent<ReportScreenHeader>();
		component.SetMainEntry(reportGroup);
		return value;
	}

	private GameObject CreateOrUpdateLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup, bool is_line_active)
	{
		GameObject value = null;
		lineItems.TryGetValue(reportGroup.stringKey, out value);
		if (!is_line_active)
		{
			if ((Object)value != (Object)null && value.activeSelf)
			{
				value.SetActive(false);
			}
		}
		else
		{
			if ((Object)value == (Object)null)
			{
				value = Util.KInstantiateUI(lineItem, contentFolder, true);
				value.name = "LineItem" + lineItems.Count;
				lineItems[reportGroup.stringKey] = value;
			}
			value.SetActive(true);
			ReportScreenEntry component = value.GetComponent<ReportScreenEntry>();
			component.SetMainEntry(entry, reportGroup);
		}
		return value;
	}

	private void OnClickClose()
	{
		PlaySound3D(GlobalAssets.GetSound("HUD_Click_Close", false));
		Show(false);
	}
}
