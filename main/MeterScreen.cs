using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeterScreen : KScreen, IRender1000ms
{
	private struct DisplayInfo
	{
		public int selectedIndex;
	}

	[SerializeField]
	private LocText currentMinions;

	public ToolTip MinionsTooltip;

	public LocText StressText;

	public ToolTip StressTooltip;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public LocText ImmunityText;

	public ToolTip ImmunityTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet = false;

	[SerializeField]
	private KToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private DisplayInfo stressDisplayInfo = new DisplayInfo
	{
		selectedIndex = -1
	};

	private DisplayInfo immunityDisplayInfo = new DisplayInfo
	{
		selectedIndex = -1
	};

	private int cachedMinionCount = -1;

	private long cachedCalories = -1L;

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

	public static MeterScreen Instance
	{
		get;
		private set;
	}

	public bool StartValuesSet => startValuesSet;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnSpawn()
	{
		StressTooltip.OnToolTip = OnStressTooltip;
		ImmunityTooltip.OnToolTip = OnImmunityTooltip;
		RationsTooltip.OnToolTip = OnRationsTooltip;
		RedAlertTooltip.OnToolTip = OnRedAlertTooltip;
		RedAlertButton.onClick += delegate
		{
			OnRedAlertClick();
		};
	}

	private void OnRedAlertClick()
	{
		bool flag = !RedAlertManager.Instance.Get().IsToggledOn();
		RedAlertManager.Instance.Get().Toggle(flag);
		if (flag)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
	}

	public void Render1000ms(float dt)
	{
		Refresh();
	}

	public void InitializeValues()
	{
		if (!startValuesSet)
		{
			startValuesSet = true;
			Refresh();
		}
	}

	private void Refresh()
	{
		RefreshMinions();
		RefreshRations();
		RefreshStress();
		RefreshImmunity();
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		if (count != cachedMinionCount)
		{
			cachedMinionCount = count;
			currentMinions.text = count.ToString("0");
			MinionsTooltip.ClearMultiStringTooltip();
			MinionsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0")), ToolTipStyle_Header);
		}
	}

	private void RefreshImmunity()
	{
		float worstImmunity = GetWorstImmunity();
		ImmunityText.text = Mathf.Round(worstImmunity).ToString();
	}

	private float GetWorstImmunity()
	{
		if (Components.LiveMinionIdentities.Count > 0)
		{
			Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
			float num = Db.Get().Amounts.ImmuneLevel.Lookup(liveMinionIdentities[0]).value;
			for (int i = 1; i < liveMinionIdentities.Count; i++)
			{
				num = Mathf.Min(Db.Get().Amounts.ImmuneLevel.Lookup(liveMinionIdentities[i]).value, num);
			}
			return num;
		}
		return 100f;
	}

	private void RefreshRations()
	{
		if ((Object)RationsText != (Object)null && (Object)RationTracker.Get() != (Object)null)
		{
			long num = (long)RationTracker.Get().CountRations(null, true);
			if (cachedCalories != num)
			{
				RationsText.text = GameUtil.GetFormattedCalories((float)num, GameUtil.TimeSlice.None, true);
				cachedCalories = num;
			}
		}
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		List<MinionIdentity> source = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		return new List<MinionIdentity>(from x in source
		orderby stress_amount.Lookup(x).value descending
		select x);
	}

	private string OnStressTooltip()
	{
		float maxStress = GameUtil.GetMaxStress();
		StressTooltip.ClearMultiStringTooltip();
		StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStress).ToString() + "%"), ToolTipStyle_Header);
		Amount stress = Db.Get().Amounts.Stress;
		IList<MinionIdentity> stressedMinions = GetStressedMinions();
		for (int i = 0; i < stressedMinions.Count; i++)
		{
			MinionIdentity minionIdentity = stressedMinions[i];
			AmountInstance amount = stress.Lookup(minionIdentity);
			AddToolTipAmountLine(StressTooltip, amount, minionIdentity, i == stressDisplayInfo.selectedIndex);
		}
		return "";
	}

	private IList<MinionIdentity> GetImmunityLevels()
	{
		Amount amounts = Db.Get().Amounts.ImmuneLevel;
		List<MinionIdentity> source = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		return new List<MinionIdentity>(from x in source
		orderby amounts.Lookup(x).value
		select x);
	}

	private string OnImmunityTooltip()
	{
		float worstImmunity = GetWorstImmunity();
		ImmunityTooltip.ClearMultiStringTooltip();
		ImmunityTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_IMMUNITY_LEVELS, Mathf.Round(worstImmunity).ToString() + "%"), ToolTipStyle_Header);
		Amount immuneLevel = Db.Get().Amounts.ImmuneLevel;
		IList<MinionIdentity> immunityLevels = GetImmunityLevels();
		for (int i = 0; i < immunityLevels.Count; i++)
		{
			MinionIdentity minionIdentity = immunityLevels[i];
			AmountInstance amount = immuneLevel.Lookup(minionIdentity);
			AddToolTipAmountLine(ImmunityTooltip, amount, minionIdentity, i == immunityDisplayInfo.selectedIndex);
		}
		return "";
	}

	private void AddToolTipAmountLine(ToolTip tooltip, AmountInstance amount, MinionIdentity id, bool selected)
	{
		string name = id.GetComponent<KSelectable>().GetName();
		string text = name + ":  " + Mathf.Round(amount.value).ToString() + "%";
		if (selected)
		{
			tooltip.AddMultiStringTooltip("<color=#F0B310FF>" + text + "</color>", ToolTipStyle_Property);
		}
		else
		{
			tooltip.AddMultiStringTooltip(text, ToolTipStyle_Property);
		}
	}

	private string OnRationsTooltip()
	{
		rationsDict.Clear();
		float calories = RationTracker.Get().CountRations(rationsDict, true);
		RationsText.text = GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true);
		RationsTooltip.ClearMultiStringTooltip();
		RationsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_MEALHISTORY, GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true)), ToolTipStyle_Header);
		RationsTooltip.AddMultiStringTooltip("", ToolTipStyle_Property);
		IOrderedEnumerable<KeyValuePair<string, float>> source = from x in rationsDict
		orderby x.Value * Game.Instance.ediblesManager.GetFoodInfo(x.Key).CaloriesPerUnit descending
		select x;
		Dictionary<string, float> dictionary = source.ToDictionary((KeyValuePair<string, float> t) => t.Key, (KeyValuePair<string, float> t) => t.Value);
		foreach (KeyValuePair<string, float> item in dictionary)
		{
			EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(item.Key);
			RationsTooltip.AddMultiStringTooltip($"{foodInfo.Name}: {GameUtil.GetFormattedCalories(item.Value * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)}", ToolTipStyle_Property);
		}
		return "";
	}

	private string OnRedAlertTooltip()
	{
		RedAlertTooltip.ClearMultiStringTooltip();
		RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_TITLE, ToolTipStyle_Header);
		RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_CONTENT, ToolTipStyle_Property);
		return "";
	}

	private void RefreshStress()
	{
		float maxStress = GameUtil.GetMaxStress();
		StressText.text = Mathf.Round(maxStress).ToString();
	}

	public void OnClickStress(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> stressedMinions = GetStressedMinions();
		UpdateDisplayInfo(base_ev_data, ref stressDisplayInfo, stressedMinions);
		OnStressTooltip();
		StressTooltip.forceRefresh = true;
	}

	public void OnClickImmunity(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> immunityLevels = GetImmunityLevels();
		UpdateDisplayInfo(base_ev_data, ref immunityDisplayInfo, immunityLevels);
		OnImmunityTooltip();
		ImmunityTooltip.forceRefresh = true;
	}

	private void UpdateDisplayInfo(BaseEventData base_ev_data, ref DisplayInfo display_info, IList<MinionIdentity> minions)
	{
		PointerEventData pointerEventData = base_ev_data as PointerEventData;
		if (pointerEventData != null)
		{
			switch (pointerEventData.button)
			{
			case PointerEventData.InputButton.Left:
				if (Components.LiveMinionIdentities.Count < display_info.selectedIndex)
				{
					display_info.selectedIndex = -1;
				}
				if (Components.LiveMinionIdentities.Count > 0)
				{
					display_info.selectedIndex = (display_info.selectedIndex + 1) % Components.LiveMinionIdentities.Count;
					MinionIdentity minionIdentity = minions[display_info.selectedIndex];
					SelectTool.Instance.SelectAndFocus(minionIdentity.transform.GetPosition(), minionIdentity.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
				}
				break;
			case PointerEventData.InputButton.Right:
				display_info.selectedIndex = -1;
				break;
			}
		}
	}
}
