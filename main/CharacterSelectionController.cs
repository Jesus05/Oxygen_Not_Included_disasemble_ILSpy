using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionController : KModalScreen
{
	[SerializeField]
	private CharacterContainer containerPrefab;

	[SerializeField]
	private GameObject containerParent;

	[SerializeField]
	protected KButton proceedButton;

	[SerializeField]
	protected int availableCharCount;

	[SerializeField]
	protected int selectableCharCount;

	[SerializeField]
	private bool allowsReplacing;

	protected List<MinionStartingStats> startingStats;

	protected List<CharacterContainer> containers;

	public System.Action OnLimitReachedEvent;

	public System.Action OnLimitUnreachedEvent;

	public Action<bool> OnReshuffleEvent;

	public Action<MinionStartingStats> OnReplacedEvent;

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

	protected virtual void OnCharacterAdded()
	{
	}

	protected virtual void OnCharacterRemoved()
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
			containers = new List<CharacterContainer>();
			for (int i = 0; i < availableCharCount; i++)
			{
				CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(containerPrefab.gameObject, containerParent, false);
				characterContainer.SetController(this);
				containers.Add(characterContainer);
			}
			startingStats = new List<MinionStartingStats>();
		}
	}

	public virtual void OnPressBack()
	{
		foreach (CharacterContainer container in containers)
		{
			container.ForceStopEditingTitle();
		}
		Show(false);
	}

	public void RemoveLast()
	{
		if (startingStats != null && startingStats.Count != 0)
		{
			MinionStartingStats obj = startingStats[startingStats.Count - 1];
			if (OnReplacedEvent != null)
			{
				OnReplacedEvent(obj);
			}
		}
	}

	public void AddCharacter(MinionStartingStats charStats)
	{
		if (startingStats.Contains(charStats))
		{
			Debug.Log("Tried to add the same minion twice.", null);
		}
		else if (startingStats.Count >= selectableCharCount)
		{
			Debug.LogError("Tried to add minions beyond the allowed limit", null);
		}
		else
		{
			startingStats.Add(charStats);
			OnCharacterAdded();
			if (startingStats.Count == selectableCharCount)
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

	public void RemoveCharacter(MinionStartingStats charStats)
	{
		bool flag = startingStats.Count >= selectableCharCount;
		startingStats.Remove(charStats);
		OnCharacterRemoved();
		if (flag && startingStats.Count < selectableCharCount)
		{
			DisableProceedButton();
			if (OnLimitUnreachedEvent != null)
			{
				OnLimitUnreachedEvent();
			}
			OnLimitUnreached();
		}
	}

	public bool IsSelected(MinionStartingStats charStats)
	{
		return startingStats.Contains(charStats);
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
