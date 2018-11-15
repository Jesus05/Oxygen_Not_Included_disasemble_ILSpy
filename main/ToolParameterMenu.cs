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
				break;
			}
		}
		if (this.onParametersChanged != null)
		{
			this.onParametersChanged();
		}
	}
}
