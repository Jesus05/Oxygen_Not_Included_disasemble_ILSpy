using STRINGS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class EnergyInfoScreen : TargetScreen
{
	public GameObject labelTemplate;

	private GameObject overviewPanel;

	private GameObject generatorsPanel;

	private GameObject consumersPanel;

	private GameObject batteriesPanel;

	private Dictionary<string, GameObject> overviewLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> generatorsLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> consumersLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> batteriesLabels = new Dictionary<string, GameObject>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overviewPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		overviewPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.CIRCUITOVERVIEW;
		generatorsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		generatorsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.GENERATORS;
		consumersPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		consumersPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.CONSUMERS;
		batteriesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		batteriesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.BATTERIES;
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(labelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (!((Object)selectedTarget == (Object)null))
		{
			foreach (KeyValuePair<string, GameObject> overviewLabel in overviewLabels)
			{
				overviewLabel.Value.SetActive(false);
			}
			foreach (KeyValuePair<string, GameObject> generatorsLabel in generatorsLabels)
			{
				generatorsLabel.Value.SetActive(false);
			}
			foreach (KeyValuePair<string, GameObject> consumersLabel in consumersLabels)
			{
				consumersLabel.Value.SetActive(false);
			}
			foreach (KeyValuePair<string, GameObject> batteriesLabel in batteriesLabels)
			{
				batteriesLabel.Value.SetActive(false);
			}
			CircuitManager circuitManager = Game.Instance.circuitManager;
			ushort num = ushort.MaxValue;
			EnergyConsumer component = selectedTarget.GetComponent<EnergyConsumer>();
			if ((Object)component != (Object)null)
			{
				num = component.CircuitID;
			}
			else
			{
				Generator component2 = selectedTarget.GetComponent<Generator>();
				if ((Object)component2 != (Object)null)
				{
					num = component2.CircuitID;
				}
			}
			if (num == 65535)
			{
				int cell = Grid.PosToCell(selectedTarget.transform.GetPosition());
				num = circuitManager.GetCircuitID(cell);
			}
			if (num != 65535)
			{
				overviewPanel.SetActive(true);
				generatorsPanel.SetActive(true);
				consumersPanel.SetActive(true);
				batteriesPanel.SetActive(true);
				float joulesAvailableOnCircuit = circuitManager.GetJoulesAvailableOnCircuit(num);
				GameObject gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "joulesAvailable");
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES, GameUtil.GetFormattedJoules(joulesAvailableOnCircuit, "F1", GameUtil.TimeSlice.None));
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES_TOOLTIP;
				gameObject.SetActive(true);
				float wattsGeneratedByCircuit = circuitManager.GetWattsGeneratedByCircuit(num);
				float potentialWattsGeneratedByCircuit = circuitManager.GetPotentialWattsGeneratedByCircuit(num);
				gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "wattageGenerated");
				string text = null;
				text = ((wattsGeneratedByCircuit != potentialWattsGeneratedByCircuit) ? $"{GameUtil.GetFormattedWattage(wattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic)} / {GameUtil.GetFormattedWattage(potentialWattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic)}" : GameUtil.GetFormattedWattage(wattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic));
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED, text);
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED_TOOLTIP;
				gameObject.SetActive(true);
				gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "wattageConsumed");
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(circuitManager.GetWattsUsedByCircuit(num), GameUtil.WattageFormatterUnit.Automatic));
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED_TOOLTIP;
				gameObject.SetActive(true);
				gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "potentialWattageConsumed");
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(circuitManager.GetWattsNeededWhenActive(num), GameUtil.WattageFormatterUnit.Automatic));
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED_TOOLTIP;
				gameObject.SetActive(true);
				gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "maxSafeWattage");
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE, GameUtil.GetFormattedWattage(circuitManager.GetMaxSafeWattageForCircuit(num), GameUtil.WattageFormatterUnit.Automatic));
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE_TOOLTIP;
				gameObject.SetActive(true);
				ReadOnlyCollection<Generator> generatorsOnCircuit = circuitManager.GetGeneratorsOnCircuit(num);
				ReadOnlyCollection<IEnergyConsumer> consumersOnCircuit = circuitManager.GetConsumersOnCircuit(num);
				List<Battery> batteriesOnCircuit = circuitManager.GetBatteriesOnCircuit(num);
				ReadOnlyCollection<Battery> transformersOnCircuit = circuitManager.GetTransformersOnCircuit(num);
				if (generatorsOnCircuit.Count > 0)
				{
					foreach (Generator item in generatorsOnCircuit)
					{
						if ((Object)item != (Object)null && (Object)item.GetComponent<Battery>() == (Object)null)
						{
							gameObject = AddOrGetLabel(generatorsLabels, generatorsPanel, item.gameObject.GetInstanceID().ToString());
							Operational component3 = item.GetComponent<Operational>();
							if (component3.IsActive)
							{
								gameObject.GetComponent<LocText>().text = $"{item.GetComponent<KSelectable>().entityName}: {GameUtil.GetFormattedWattage(item.WattageRating, GameUtil.WattageFormatterUnit.Automatic)}";
							}
							else
							{
								gameObject.GetComponent<LocText>().text = $"{item.GetComponent<KSelectable>().entityName}: {GameUtil.GetFormattedWattage(0f, GameUtil.WattageFormatterUnit.Automatic)} / {GameUtil.GetFormattedWattage(item.WattageRating, GameUtil.WattageFormatterUnit.Automatic)}";
							}
							gameObject.SetActive(true);
							gameObject.GetComponent<LocText>().fontStyle = (((Object)item.gameObject == (Object)selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
						}
					}
				}
				else
				{
					gameObject = AddOrGetLabel(generatorsLabels, generatorsPanel, "nogenerators");
					gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOGENERATORS;
					gameObject.SetActive(true);
				}
				if (consumersOnCircuit.Count > 0 || transformersOnCircuit.Count > 0)
				{
					foreach (IEnergyConsumer item2 in consumersOnCircuit)
					{
						AddConsumerInfo(item2, gameObject);
					}
					foreach (Battery item3 in transformersOnCircuit)
					{
						AddConsumerInfo(item3, gameObject);
					}
				}
				else
				{
					gameObject = AddOrGetLabel(consumersLabels, consumersPanel, "noconsumers");
					gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOCONSUMERS;
					gameObject.SetActive(true);
				}
				if (batteriesOnCircuit.Count > 0)
				{
					foreach (Battery item4 in batteriesOnCircuit)
					{
						if ((Object)item4 != (Object)null)
						{
							gameObject = AddOrGetLabel(batteriesLabels, batteriesPanel, item4.gameObject.GetInstanceID().ToString());
							gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", item4.GetComponent<KSelectable>().entityName, GameUtil.GetFormattedJoules(item4.JoulesAvailable, "F1", GameUtil.TimeSlice.None));
							gameObject.SetActive(true);
							gameObject.GetComponent<LocText>().fontStyle = (((Object)item4.gameObject == (Object)selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
						}
					}
				}
				else
				{
					gameObject = AddOrGetLabel(batteriesLabels, batteriesPanel, "nobatteries");
					gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOBATTERIES;
					gameObject.SetActive(true);
				}
			}
			else
			{
				overviewPanel.SetActive(true);
				generatorsPanel.SetActive(false);
				consumersPanel.SetActive(false);
				batteriesPanel.SetActive(false);
				GameObject gameObject2 = AddOrGetLabel(overviewLabels, overviewPanel, "nocircuit");
				gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.DISCONNECTED;
				gameObject2.SetActive(true);
			}
		}
	}

	private void AddConsumerInfo(IEnergyConsumer consumer, GameObject label)
	{
		KMonoBehaviour kMonoBehaviour = consumer as KMonoBehaviour;
		if ((Object)kMonoBehaviour != (Object)null)
		{
			label = AddOrGetLabel(consumersLabels, consumersPanel, kMonoBehaviour.gameObject.GetInstanceID().ToString());
			float wattsUsed = consumer.WattsUsed;
			float wattsNeededWhenActive = consumer.WattsNeededWhenActive;
			string text = null;
			text = ((wattsUsed != wattsNeededWhenActive) ? $"{GameUtil.GetFormattedWattage(wattsUsed, GameUtil.WattageFormatterUnit.Automatic)} / {GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic)}" : GameUtil.GetFormattedWattage(wattsUsed, GameUtil.WattageFormatterUnit.Automatic));
			label.GetComponent<LocText>().text = $"{consumer.Name}: {text}";
			label.SetActive(true);
			label.GetComponent<LocText>().fontStyle = (((Object)kMonoBehaviour.gameObject == (Object)selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
		}
	}
}
