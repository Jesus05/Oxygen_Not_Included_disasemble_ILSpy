using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionController : KModalScreen
{
	[SerializeField]
	private CharacterContainer containerPrefab;

	[SerializeField]
	private CarePackageContainer carePackageContainerPrefab;

	[SerializeField]
	private GameObject containerParent;

	[SerializeField]
	protected KButton proceedButton;

	protected int numberOfDuplicantOptions = 3;

	protected int numberOfCarePackageOptions = 0;

	[SerializeField]
	protected int selectableCount;

	[SerializeField]
	private bool allowsReplacing = false;

	protected List<ITelepadDeliverable> selectedDeliverables;

	protected List<ITelepadDeliverableContainer> containers;

	public System.Action OnLimitReachedEvent;

	public System.Action OnLimitUnreachedEvent;

	public Action<bool> OnReshuffleEvent;

	public Action<ITelepadDeliverable> OnReplacedEvent;

	public System.Action OnProceedEvent;

	public bool IsStarterMinion
	{
		get;
		set;
	}

	public bool AllowsReplacing => allowsReplacing;

	protected virtual void OnProceed()
	{
	}

	protected virtual void OnDeliverableAdded()
	{
	}

	protected virtual void OnDeliverableRemoved()
	{
	}

	protected virtual void OnLimitReached()
	{
	}

	protected virtual void OnLimitUnreached()
	{
	}

	protected virtual void InitializeContainers()
	{
		DisableProceedButton();
		if (containers == null || containers.Count <= 0)
		{
			OnReplacedEvent = null;
			containers = new List<ITelepadDeliverableContainer>();
			if (IsStarterMinion || CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CarePackages).id != "Enabled")
			{
				numberOfDuplicantOptions = 3;
				numberOfCarePackageOptions = 0;
			}
			else
			{
				numberOfCarePackageOptions = ((UnityEngine.Random.Range(0, 101) <= 70) ? 1 : 2);
				numberOfDuplicantOptions = 4 - numberOfCarePackageOptions;
			}
			for (int i = 0; i < numberOfDuplicantOptions; i++)
			{
				CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(containerPrefab.gameObject, containerParent, false);
				characterContainer.SetController(this);
				containers.Add(characterContainer);
			}
			for (int j = 0; j < numberOfCarePackageOptions; j++)
			{
				CarePackageContainer carePackageContainer = Util.KInstantiateUI<CarePackageContainer>(carePackageContainerPrefab.gameObject, containerParent, false);
				carePackageContainer.SetController(this);
				containers.Add(carePackageContainer);
				carePackageContainer.gameObject.transform.SetSiblingIndex(UnityEngine.Random.Range(0, carePackageContainer.transform.parent.childCount));
			}
			selectedDeliverables = new List<ITelepadDeliverable>();
		}
	}

	public virtual void OnPressBack()
	{
		foreach (ITelepadDeliverableContainer container in containers)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
			{
				characterContainer.ForceStopEditingTitle();
			}
		}
		Show(false);
	}

	public void RemoveLast()
	{
		if (selectedDeliverables != null && selectedDeliverables.Count != 0)
		{
			ITelepadDeliverable obj = selectedDeliverables[selectedDeliverables.Count - 1];
			if (OnReplacedEvent != null)
			{
				OnReplacedEvent(obj);
			}
		}
	}

	public void AddDeliverable(ITelepadDeliverable deliverable)
	{
		if (selectedDeliverables.Contains(deliverable))
		{
			Debug.Log("Tried to add the same minion twice.");
		}
		else if (selectedDeliverables.Count >= selectableCount)
		{
			Debug.LogError("Tried to add minions beyond the allowed limit");
		}
		else
		{
			selectedDeliverables.Add(deliverable);
			OnDeliverableAdded();
			if (selectedDeliverables.Count == selectableCount)
			{
				EnableProceedButton();
				if (OnLimitReachedEvent != null)
				{
					OnLimitReachedEvent();
				}
				OnLimitReached();
			}
		}
	}

	public void RemoveDeliverable(ITelepadDeliverable deliverable)
	{
		bool flag = selectedDeliverables.Count >= selectableCount;
		selectedDeliverables.Remove(deliverable);
		OnDeliverableRemoved();
		if (flag && selectedDeliverables.Count < selectableCount)
		{
			DisableProceedButton();
			if (OnLimitUnreachedEvent != null)
			{
				OnLimitUnreachedEvent();
			}
			OnLimitUnreached();
		}
	}

	public bool IsSelected(ITelepadDeliverable deliverable)
	{
		return selectedDeliverables.Contains(deliverable);
	}

	protected void EnableProceedButton()
	{
		proceedButton.isInteractable = true;
		proceedButton.ClearOnClick();
		proceedButton.onClick += delegate
		{
			OnProceed();
		};
	}

	protected void DisableProceedButton()
	{
		proceedButton.ClearOnClick();
		proceedButton.isInteractable = false;
		proceedButton.onClick += delegate
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		};
	}
}
