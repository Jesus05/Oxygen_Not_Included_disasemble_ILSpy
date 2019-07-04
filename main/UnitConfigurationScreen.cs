using STRINGS;
using System;
using UnityEngine;

[Serializable]
public class UnitConfigurationScreen
{
	[SerializeField]
	private GameObject toggleUnitPrefab;

	[SerializeField]
	private GameObject toggleGroup;

	private GameObject celsiusToggle;

	private GameObject kelvinToggle;

	private GameObject fahrenheitToggle;

	public static readonly string TemperatureUnitKey = "TemperatureUnit";

	public static readonly string MassUnitKey = "MassUnit";

	public void Init()
	{
		celsiusToggle = Util.KInstantiateUI(toggleUnitPrefab, toggleGroup, true);
		celsiusToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.CELSIUS_TOOLTIP;
		celsiusToggle.GetComponentInChildren<KButton>().onClick += OnCelsiusClicked;
		celsiusToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.CELSIUS;
		kelvinToggle = Util.KInstantiateUI(toggleUnitPrefab, toggleGroup, true);
		kelvinToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.KELVIN_TOOLTIP;
		kelvinToggle.GetComponentInChildren<KButton>().onClick += OnKelvinClicked;
		kelvinToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.KELVIN;
		fahrenheitToggle = Util.KInstantiateUI(toggleUnitPrefab, toggleGroup, true);
		fahrenheitToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.FAHRENHEIT_TOOLTIP;
		fahrenheitToggle.GetComponentInChildren<KButton>().onClick += OnFahrenheitClicked;
		fahrenheitToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.FAHRENHEIT;
		DisplayCurrentUnit();
	}

	private void DisplayCurrentUnit()
	{
		switch (KPlayerPrefs.GetInt(TemperatureUnitKey, 0))
		{
		case 0:
			celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			break;
		case 2:
			celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			break;
		default:
			celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			break;
		}
	}

	private void OnCelsiusClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Celsius;
		KPlayerPrefs.SetInt(TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		DisplayCurrentUnit();
	}

	private void OnKelvinClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Kelvin;
		KPlayerPrefs.SetInt(TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		DisplayCurrentUnit();
	}

	private void OnFahrenheitClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Fahrenheit;
		KPlayerPrefs.SetInt(TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		DisplayCurrentUnit();
	}
}
