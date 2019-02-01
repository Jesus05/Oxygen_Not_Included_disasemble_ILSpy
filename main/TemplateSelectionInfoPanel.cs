using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TemplateSelectionInfoPanel : KMonoBehaviour, IRender1000ms
{
	[SerializeField]
	private GameObject prefab_detail_label;

	[SerializeField]
	private GameObject current_detail_container;

	[SerializeField]
	private LocText saved_detail_label;

	[SerializeField]
	private KButton save_button;

	private Func<List<int>, string>[] details = new Func<List<int>, string>[6]
	{
		TotalMass,
		AverageMass,
		AverageTemperature,
		TotalJoules,
		JoulesPerKilogram,
		MassPerElement
	};

	private static List<Tuple<Element, float>> mass_per_element = new List<Tuple<Element, float>>();

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<List<int>, string> _003C_003Ef__mg_0024cache5;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < details.Length; i++)
		{
			Util.KInstantiateUI(prefab_detail_label, current_detail_container, true);
		}
		RefreshDetails();
		save_button.onClick += SaveCurrentDetails;
	}

	public void SaveCurrentDetails()
	{
		string str = "";
		for (int i = 0; i < details.Length; i++)
		{
			str = str + details[i](DebugBaseTemplateButton.Instance.SelectedCells) + "\n";
		}
		str += UI.HORIZONTAL_BR_RULE;
		str += saved_detail_label.text;
		saved_detail_label.text = str;
	}

	public void Render1000ms(float dt)
	{
		RefreshDetails();
	}

	public void RefreshDetails()
	{
		for (int i = 0; i < details.Length; i++)
		{
			current_detail_container.transform.GetChild(i).GetComponent<LocText>().text = details[i](DebugBaseTemplateButton.Instance.SelectedCells);
		}
	}

	private static string TotalMass(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Mass[cell];
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_MASS, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private static string AverageMass(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Mass[cell];
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_MASS, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private static string AverageTemperature(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Temperature[cell];
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_TEMPERATURE, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
	}

	private static string TotalJoules(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_JOULES, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None));
	}

	private static string JoulesPerKilogram(List<int> cells)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
			num2 += Grid.Mass[cell];
		}
		num /= num2;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.JOULES_PER_KILOGRAM, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None));
	}

	private static string MassPerElement(List<int> cells)
	{
		mass_per_element.Clear();
		foreach (int cell in cells)
		{
			bool flag = false;
			for (int i = 0; i < mass_per_element.Count; i++)
			{
				if (mass_per_element[i].first == Grid.Element[cell])
				{
					mass_per_element[i].second += Grid.Mass[cell];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				mass_per_element.Add(new Tuple<Element, float>(Grid.Element[cell], Grid.Mass[cell]));
			}
		}
		mass_per_element.Sort(delegate(Tuple<Element, float> a, Tuple<Element, float> b)
		{
			if (!(a.second > b.second))
			{
				if (!(b.second > a.second))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		});
		string text = "";
		foreach (Tuple<Element, float> item in mass_per_element)
		{
			string text2 = text;
			text = text2 + item.first.name + ": " + GameUtil.GetFormattedMass(item.second, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") + "\n";
		}
		return text;
	}
}
