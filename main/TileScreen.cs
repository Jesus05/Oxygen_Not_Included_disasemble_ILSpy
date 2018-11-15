using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScreen : KScreen
{
	public Text nameLabel;

	public Text symbolLabel;

	public Text massTitleLabel;

	public Text massAmtLabel;

	public Image massIcon;

	public MinMaxSlider temperatureSlider;

	public Text temperatureSliderText;

	public Image temperatureSliderIcon;

	public Image solidIcon;

	public Image liquidIcon;

	public Image gasIcon;

	public Text solidText;

	public Text gasText;

	[SerializeField]
	private Color temperatureDefaultColour;

	[SerializeField]
	private Color temperatureTransitionColour;

	private bool SetSliderColour(float temperature, float transition_temperature)
	{
		if (Mathf.Abs(temperature - transition_temperature) < 5f)
		{
			temperatureSliderText.color = temperatureTransitionColour;
			temperatureSliderIcon.color = temperatureTransitionColour;
			return true;
		}
		temperatureSliderText.color = temperatureDefaultColour;
		temperatureSliderIcon.color = temperatureDefaultColour;
		return false;
	}

	private void DisplayTileInfo()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		Vector3 position = Camera.main.transform.GetPosition();
		mousePos.z = 0f - position.z - Grid.CellSizeInMeters;
		Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
		int num = Grid.PosToCell(pos);
		if (Grid.IsValidCell(num) && Grid.IsVisible(num))
		{
			Element element = Grid.Element[num];
			nameLabel.text = element.name;
			float num2 = Grid.Mass[num];
			string arg = "kg";
			if (num2 < 5f)
			{
				num2 *= 1000f;
				arg = "g";
			}
			if (num2 < 5f)
			{
				num2 *= 1000f;
				arg = "mg";
			}
			if (num2 < 5f)
			{
				num2 *= 1000f;
				arg = "mcg";
				num2 = Mathf.Floor(num2);
			}
			massAmtLabel.text = $"{num2:0.0} {arg}";
			massTitleLabel.text = "mass";
			float num3 = Grid.Temperature[num];
			if (element.IsSolid)
			{
				solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				gasIcon.gameObject.transform.parent.gameObject.SetActive(false);
				massIcon.sprite = solidIcon.sprite;
				solidText.text = ((int)element.highTemp).ToString();
				gasText.text = string.Empty;
				liquidIcon.rectTransform.SetParent(solidIcon.transform.parent, true);
				liquidIcon.rectTransform.SetLocalPosition(new Vector3(0f, 64f));
				SetSliderColour(num3, element.highTemp);
				temperatureSlider.SetMinMaxValue(element.highTemp, Mathf.Min(element.highTemp + 100f, 4000f), Mathf.Max(element.highTemp - 100f, 0f), Mathf.Min(element.highTemp + 100f, 4000f));
			}
			else if (element.IsLiquid)
			{
				solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				massIcon.sprite = liquidIcon.sprite;
				solidText.text = ((int)element.lowTemp).ToString();
				gasText.text = ((int)element.highTemp).ToString();
				liquidIcon.rectTransform.SetParent(temperatureSlider.transform.parent, true);
				liquidIcon.rectTransform.SetLocalPosition(new Vector3(-80f, 0f));
				if (!SetSliderColour(num3, element.lowTemp))
				{
					SetSliderColour(num3, element.highTemp);
				}
				temperatureSlider.SetMinMaxValue(element.lowTemp, element.highTemp, Mathf.Max(element.lowTemp - 100f, 0f), Mathf.Min(element.highTemp + 100f, 5200f));
			}
			else if (element.IsGas)
			{
				solidText.text = string.Empty;
				gasText.text = ((int)element.lowTemp).ToString();
				solidIcon.gameObject.transform.parent.gameObject.SetActive(false);
				gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				massIcon.sprite = gasIcon.sprite;
				SetSliderColour(num3, element.lowTemp);
				liquidIcon.rectTransform.SetParent(gasIcon.transform.parent, true);
				liquidIcon.rectTransform.SetLocalPosition(new Vector3(0f, -64f));
				temperatureSlider.SetMinMaxValue(0f, Mathf.Max(element.lowTemp - 100f, 0f), 0f, element.lowTemp + 100f);
			}
			temperatureSlider.SetExtraValue(num3);
			temperatureSliderText.text = GameUtil.GetFormattedTemperature((float)(int)num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
			Dictionary<int, float> info = FallingWater.instance.GetInfo(num);
			if (info.Count > 0)
			{
				List<Element> elements = ElementLoader.elements;
				foreach (KeyValuePair<int, float> item in info)
				{
					Element element2 = elements[item.Key];
					Text text = nameLabel;
					text.text = text.text + "\n" + element2.name + $" {item.Value:0.00} kg";
				}
			}
		}
		else
		{
			nameLabel.text = "Unknown";
		}
	}

	private void DisplayConduitFlowInfo()
	{
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		UtilityNetworkManager<FlowUtilityNetwork, Vent> utilityNetworkManager = (mode != SimViewMode.GasVentMap) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
		ConduitFlow conduitFlow = (mode != SimViewMode.GasVentMap) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
		Vector3 mousePos = KInputManager.GetMousePos();
		Vector3 position = Camera.main.transform.GetPosition();
		mousePos.z = 0f - position.z - Grid.CellSizeInMeters;
		Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
		int cell = Grid.PosToCell(pos);
		if (Grid.IsValidCell(cell) && utilityNetworkManager.GetConnections(cell, true) != 0)
		{
			ConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
			SimHashes element = contents.element;
			Element element2 = ElementLoader.FindElementByHash(element);
			float num = contents.mass;
			float temperature = contents.temperature;
			nameLabel.text = element2.name;
			string arg = "kg";
			if (num < 5f)
			{
				num *= 1000f;
				arg = "g";
			}
			massAmtLabel.text = $"{num:0.0} {arg}";
			massTitleLabel.text = "mass";
			if (element2.IsLiquid)
			{
				solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				massIcon.sprite = liquidIcon.sprite;
				solidText.text = ((int)element2.lowTemp).ToString();
				gasText.text = ((int)element2.highTemp).ToString();
				liquidIcon.rectTransform.SetParent(temperatureSlider.transform.parent, true);
				liquidIcon.rectTransform.SetLocalPosition(new Vector3(-80f, 0f));
				if (!SetSliderColour(temperature, element2.lowTemp))
				{
					SetSliderColour(temperature, element2.highTemp);
				}
				temperatureSlider.SetMinMaxValue(element2.lowTemp, element2.highTemp, Mathf.Max(element2.lowTemp - 100f, 0f), Mathf.Min(element2.highTemp + 100f, 5200f));
			}
			else if (element2.IsGas)
			{
				solidText.text = string.Empty;
				gasText.text = ((int)element2.lowTemp).ToString();
				solidIcon.gameObject.transform.parent.gameObject.SetActive(false);
				gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				massIcon.sprite = gasIcon.sprite;
				SetSliderColour(temperature, element2.lowTemp);
				liquidIcon.rectTransform.SetParent(gasIcon.transform.parent, true);
				liquidIcon.rectTransform.SetLocalPosition(new Vector3(0f, -64f));
				temperatureSlider.SetMinMaxValue(0f, Mathf.Max(element2.lowTemp - 100f, 0f), 0f, element2.lowTemp + 100f);
			}
			temperatureSlider.SetExtraValue(temperature);
			temperatureSliderText.text = GameUtil.GetFormattedTemperature((float)(int)temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
		}
		else
		{
			nameLabel.text = "No Conduit";
			symbolLabel.text = string.Empty;
			massAmtLabel.text = string.Empty;
			massTitleLabel.text = string.Empty;
		}
	}

	private void Update()
	{
		base.transform.SetPosition(KInputManager.GetMousePos());
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		if (mode == SimViewMode.GasVentMap || mode == SimViewMode.LiquidVentMap)
		{
			DisplayConduitFlowInfo();
		}
		else
		{
			DisplayTileInfo();
		}
	}
}
