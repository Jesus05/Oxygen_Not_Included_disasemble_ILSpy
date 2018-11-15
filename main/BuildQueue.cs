using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildQueue : KButtonMenu
{
	private IHasBuildQueue fabricator;

	private int prevLength;

	private Dictionary<Tag, float> allocatedMaterials = new Dictionary<Tag, float>();

	public List<Storage> availableMaterialStorages = new List<Storage>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		keepMenuOpen = true;
		buttons = new ButtonInfo[6];
		for (int i = 0; i < 6; i++)
		{
			string text = (i + 1).ToString();
			int order_idx = i;
			buttons[i] = new ButtonInfo(text, Action.NumActions, delegate
			{
				CancelOrder(order_idx);
			}, null, null);
		}
	}

	public void CancelOrder(int order_idx)
	{
		if (fabricator != null && fabricator.NumOrders != 0 && order_idx < fabricator.NumOrders)
		{
			fabricator.CancelOrder(order_idx);
		}
	}

	private void Update()
	{
		allocatedMaterials.Clear();
		int i = 0;
		if (fabricator != null)
		{
			List<IBuildQueueOrder> orders = fabricator.Orders;
			foreach (IBuildQueueOrder item in orders)
			{
				BuildQueueButton componentInChildren = buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
				componentInChildren.SetOrder(item);
				bool currentAvailability = true;
				string text = string.Empty;
				foreach (KeyValuePair<Tag, float> materialRequirement in item.GetMaterialRequirements())
				{
					float num = materialRequirement.Value - WorldInventory.Instance.GetAmount(materialRequirement.Key);
					for (int j = 0; j < availableMaterialStorages.Count; j++)
					{
						num -= availableMaterialStorages[j].GetAmountAvailable(materialRequirement.Key);
					}
					if (allocatedMaterials.ContainsKey(materialRequirement.Key))
					{
						num += allocatedMaterials[materialRequirement.Key];
					}
					if (num > 0f)
					{
						currentAvailability = false;
						text += string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.QUEUED_MISSING_INGREDIENTS_TOOLTIP, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), materialRequirement.Key.ProperName());
					}
					if (allocatedMaterials.ContainsKey(materialRequirement.Key))
					{
						Dictionary<Tag, float> dictionary;
						Tag key;
						(dictionary = allocatedMaterials)[key = materialRequirement.Key] = dictionary[key] + materialRequirement.Value;
					}
					else
					{
						allocatedMaterials.Add(materialRequirement.Key, materialRequirement.Value);
					}
				}
				componentInChildren.SetAvailability(item.Result.ProperName(), currentAvailability, text);
				i++;
				if (i >= 6)
				{
					break;
				}
			}
			if (orders.Count > prevLength)
			{
				BuildQueueButton componentInChildren2 = buttonObjects[prevLength].GetComponentInChildren<BuildQueueButton>();
				SizePulse pulse = componentInChildren2.gameObject.AddComponent<SizePulse>();
				pulse.speed = 10f;
				pulse.updateWhenPaused = true;
				SizePulse sizePulse = pulse;
				sizePulse.onComplete = (System.Action)Delegate.Combine(sizePulse.onComplete, (System.Action)delegate
				{
					UnityEngine.Object.Destroy(pulse);
				});
			}
			prevLength = orders.Count;
		}
		if (buttonObjects != null)
		{
			for (; i < 6; i++)
			{
				BuildQueueButton componentInChildren3 = buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
				componentInChildren3.SetOrder(null);
			}
		}
	}

	public void SetFabricator(IHasBuildQueue fabricator)
	{
		availableMaterialStorages.Clear();
		this.fabricator = fabricator;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		RefreshButtons();
	}

	public void AddAvailableMaterialStorage(Storage storage)
	{
		if (!availableMaterialStorages.Contains(storage))
		{
			availableMaterialStorages.Add(storage);
		}
	}

	protected override void OnDeactivate()
	{
		for (int i = 0; i < 6; i++)
		{
			BuildQueueButton componentInChildren = buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
			if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
			{
				componentInChildren.SetOrder(null);
			}
		}
	}
}
