using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolParameterMenu : KMonoBehaviour
{
	public class FILTERLAYERS
	{
		public static string BUILDINGS = "BUILDINGS";

		public static string TILES = "TILES";

		public static string WIRES = "WIRES";

		public static string LIQUIDCONDUIT = "LIQUIDPIPES";

		public static string GASCONDUIT = "GASPIPES";

		public static string SOLIDCONDUIT = "SOLIDCONDUITS";

		public static string CLEANANDCLEAR = "CLEANANDCLEAR";

		public static string DIGPLACER = "DIGPLACER";

		public static string LOGIC = "LOGIC";

		public static string BACKWALL = "BACKWALL";

		public static string CONSTRUCTION = "CONSTRUCTION";

		public static string DIG = "DIG";

		public static string CLEAN = "CLEAN";

		public static string OPERATE = "OPERATE";

		public static string METAL = "METAL";

		public static string BUILDABLE = "BUILDABLE";

		public static string FILTER = "FILTER";

		public static string LIQUIFIABLE = "LIQUIFIABLE";

		public static string LIQUID = "LIQUID";

		public static string CONSUMABLEORE = "CONSUMABLEORE";

		public static string ORGANICS = "ORGANICS";

		public static string FARMABLE = "FARMABLE";

		public static string GAS = "GAS";

		public static string HEATFLOW = "HEATFLOW";

		public static string ABSOLUTETEMPERATURE = "ABSOLUTETEMPERATURE";

		public static string ADAPTIVETEMPERATURE = "ADAPTIVETEMPERATURE";

		public static string STATECHANGE = "STATECHANGE";

		public static string ALL = "ALL";
	}

	public enum ToggleState
	{
		On,
		Off,
		Disabled
	}

	public GameObject content;

	public GameObject widgetContainer;

	public GameObject widgetPrefab;

	private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

	private Dictionary<string, ToggleState> currentParameters;

	private string lastEnabledFilter;

	public event System.Action onParametersChanged;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ClearMenu();
	}

	public void PopulateMenu(Dictionary<string, ToggleState> parameters)
	{
		ClearMenu();
		currentParameters = parameters;
		foreach (KeyValuePair<string, ToggleState> parameter in parameters)
		{
			GameObject gameObject = Util.KInstantiateUI(widgetPrefab, widgetContainer, true);
			gameObject.GetComponentInChildren<LocText>().text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + parameter.Key);
			widgets.Add(parameter.Key, gameObject);
			MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
			switch (parameter.Value)
			{
			case ToggleState.Disabled:
				toggle.ChangeState(2);
				break;
			case ToggleState.On:
				toggle.ChangeState(1);
				lastEnabledFilter = parameter.Key;
				break;
			default:
				toggle.ChangeState(0);
				break;
			}
			MultiToggle multiToggle = toggle;
			multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
			{
				foreach (KeyValuePair<string, GameObject> widget in widgets)
				{
					if ((UnityEngine.Object)widget.Value == (UnityEngine.Object)toggle.transform.parent.gameObject)
					{
						if (currentParameters[widget.Key] != ToggleState.Disabled)
						{
							ChangeToSetting(widget.Key);
							OnChange();
						}
						break;
					}
				}
			});
		}
		content.SetActive(true);
	}

	public void ClearMenu()
	{
		content.SetActive(false);
		foreach (KeyValuePair<string, GameObject> widget in widgets)
		{
			Util.KDestroyGameObject(widget.Value);
		}
		widgets.Clear();
	}

	private void ChangeToSetting(string key)
	{
		foreach (KeyValuePair<string, GameObject> widget in widgets)
		{
			if (currentParameters[widget.Key] != ToggleState.Disabled)
			{
				currentParameters[widget.Key] = ToggleState.Off;
			}
		}
		currentParameters[key] = ToggleState.On;
	}

	private void OnChange()
	{
		foreach (KeyValuePair<string, GameObject> widget in widgets)
		{
			switch (currentParameters[widget.Key])
			{
			case ToggleState.Disabled:
				widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
				break;
			case ToggleState.Off:
				widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
				break;
			case ToggleState.On:
				widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
				lastEnabledFilter = widget.Key;
				break;
			}
		}
		if (this.onParametersChanged != null)
		{
			this.onParametersChanged();
		}
	}

	public string GetLastEnabledFilter()
	{
		return lastEnabledFilter;
	}
}
