using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceptacleSideScreen : SideScreenContent, IRender1000ms
{
	protected class SelectableEntity
	{
		public Tag tag;

		public SingleEntityReceptacle.ReceptacleDirection direction;

		public GameObject asset;

		public float lastAmount = -1f;
	}

	[SerializeField]
	private KButton requestSelectedEntityBtn;

	[SerializeField]
	private string requestStringDeposit;

	[SerializeField]
	private string requestStringCancelDeposit;

	[SerializeField]
	private string requestStringRemove;

	[SerializeField]
	private string requestStringCancelRemove;

	public GameObject activeEntityContainer;

	public GameObject nothingDiscoveredContainer;

	[SerializeField]
	protected LocText descriptionLabel;

	private Dictionary<SingleEntityReceptacle, int> entityPreviousSelectionMap = new Dictionary<SingleEntityReceptacle, int>();

	[SerializeField]
	private string subtitleStringSelect;

	[SerializeField]
	private string subtitleStringSelectDescription;

	[SerializeField]
	private string subtitleStringAwaitingSelection;

	[SerializeField]
	private string subtitleStringAwaitingDelivery;

	[SerializeField]
	private string subtitleStringEntityDeposited;

	[SerializeField]
	private string subtitleStringAwaitingRemoval;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private List<DescriptorPanel> descriptorPanels;

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	[SerializeField]
	private GameObject requestObjectList;

	[SerializeField]
	private GameObject requestObjectListContainer;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private GameObject entityToggle;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	[SerializeField]
	private bool hideUndiscoveredEntities;

	private ReceptacleToggle selectedEntityToggle;

	protected SingleEntityReceptacle targetReceptacle;

	private Tag selectedDepositObjectTag;

	private Dictionary<ReceptacleToggle, SelectableEntity> depositObjectMap;

	private List<ReceptacleToggle> entityToggles = new List<ReceptacleToggle>();

	private int onObjectDestroyedHandle = -1;

	private int onOccupantValidChangedHandle = -1;

	private int onStorageChangedHandle = -1;

	public override string GetTitle()
	{
		if ((Object)targetReceptacle == (Object)null)
		{
			return Strings.Get(titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(titleKey), targetReceptacle.GetProperName());
	}

	public void Initialize(SingleEntityReceptacle target)
	{
		if ((Object)target == (Object)null)
		{
			Debug.LogError("SingleObjectReceptacle provided was null.");
		}
		else
		{
			targetReceptacle = target;
			base.gameObject.SetActive(true);
			depositObjectMap = new Dictionary<ReceptacleToggle, SelectableEntity>();
			entityToggles.ForEach(delegate(ReceptacleToggle rbi)
			{
				Object.Destroy(rbi.gameObject);
			});
			entityToggles.Clear();
			Tag[] possibleDepositObjectTags = target.possibleDepositObjectTags;
			ReceptacleToggle newToggle;
			foreach (Tag tag in possibleDepositObjectTags)
			{
				List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(tag);
				if ((Object)targetReceptacle.rotatable == (Object)null)
				{
					prefabsWithTag.RemoveAll(delegate(GameObject go)
					{
						IReceptacleDirection component3 = go.GetComponent<IReceptacleDirection>();
						return component3 != null && component3.Direction != targetReceptacle.Direction;
					});
				}
				List<IHasSortOrder> list = new List<IHasSortOrder>();
				foreach (GameObject item in prefabsWithTag)
				{
					IHasSortOrder component = item.GetComponent<IHasSortOrder>();
					if (component != null)
					{
						list.Add(component);
					}
				}
				Debug.Assert(list.Count == prefabsWithTag.Count, "Not all entities in this receptacle implement IHasSortOrder!");
				list.Sort((IHasSortOrder a, IHasSortOrder b) => a.sortOrder - b.sortOrder);
				foreach (IHasSortOrder item2 in list)
				{
					GameObject gameObject = (item2 as MonoBehaviour).gameObject;
					GameObject gameObject2 = Util.KInstantiateUI(entityToggle, requestObjectList, false);
					gameObject2.SetActive(true);
					newToggle = gameObject2.GetComponent<ReceptacleToggle>();
					IReceptacleDirection component2 = gameObject.GetComponent<IReceptacleDirection>();
					string properName = gameObject.GetProperName();
					newToggle.title.text = properName;
					Sprite entityIcon = GetEntityIcon(gameObject.PrefabID());
					if ((Object)entityIcon == (Object)null)
					{
						entityIcon = elementPlaceholderSpr;
					}
					newToggle.image.sprite = entityIcon;
					newToggle.toggle.onClick += delegate
					{
						ToggleClicked(newToggle);
					};
					newToggle.toggle.onPointerEnter += delegate
					{
						CheckAmountsAndUpdate(null);
					};
					depositObjectMap.Add(newToggle, new SelectableEntity
					{
						tag = gameObject.PrefabID(),
						direction = (component2?.Direction ?? SingleEntityReceptacle.ReceptacleDirection.Top),
						asset = gameObject
					});
					entityToggles.Add(newToggle);
				}
			}
			selectedEntityToggle = null;
			if (entityToggles.Count > 0)
			{
				if (entityPreviousSelectionMap.ContainsKey(targetReceptacle))
				{
					int index = entityPreviousSelectionMap[targetReceptacle];
					ToggleClicked(entityToggles[index]);
				}
				else
				{
					subtitleLabel.SetText(Strings.Get(subtitleStringSelect).ToString());
					requestSelectedEntityBtn.isInteractable = false;
					descriptionLabel.SetText(Strings.Get(subtitleStringSelectDescription).ToString());
					HideAllDescriptorPanels();
				}
			}
			onStorageChangedHandle = targetReceptacle.gameObject.Subscribe(-1697596308, CheckAmountsAndUpdate);
			onOccupantValidChangedHandle = targetReceptacle.gameObject.Subscribe(-1820564715, OnOccupantValidChanged);
			UpdateState(null);
			SimAndRenderScheduler.instance.Add(this, false);
		}
	}

	private void UpdateState(object data)
	{
		requestSelectedEntityBtn.ClearOnClick();
		if (!((Object)targetReceptacle == (Object)null))
		{
			if (CheckReceptacleOccupied())
			{
				Uprootable uprootable = targetReceptacle.Occupant.GetComponent<Uprootable>();
				if ((Object)uprootable != (Object)null && uprootable.IsMarkedForUproot)
				{
					requestSelectedEntityBtn.onClick += delegate
					{
						uprootable.ForceCancelUproot(null);
						UpdateState(null);
					};
					requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringCancelRemove).ToString();
					requestSelectedEntityBtn.isInteractable = true;
					subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingRemoval).ToString(), targetReceptacle.Occupant.GetProperName()));
				}
				else
				{
					requestSelectedEntityBtn.onClick += delegate
					{
						targetReceptacle.OrderRemoveOccupant();
						UpdateState(null);
					};
					requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringRemove).ToString();
					requestSelectedEntityBtn.isInteractable = true;
					subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringEntityDeposited).ToString(), targetReceptacle.Occupant.GetProperName()));
				}
				ToggleObjectPicker(false);
				Tag tag = targetReceptacle.Occupant.GetComponent<KSelectable>().PrefabID();
				ConfigureActiveEntity(tag);
				SetResultDescriptions(targetReceptacle.Occupant);
			}
			else if (targetReceptacle.GetActiveRequest != null)
			{
				requestSelectedEntityBtn.onClick += delegate
				{
					targetReceptacle.CancelActiveRequest();
					ClearSelection();
					UpdateAvailableAmounts(null);
					UpdateState(null);
				};
				requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringCancelDeposit).ToString();
				requestSelectedEntityBtn.isInteractable = true;
				ToggleObjectPicker(false);
				ConfigureActiveEntity(targetReceptacle.GetActiveRequest.tags[0]);
				GameObject prefab = Assets.GetPrefab(targetReceptacle.GetActiveRequest.tags[0]);
				if ((Object)prefab != (Object)null)
				{
					subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingDelivery).ToString(), prefab.GetProperName()));
					SetResultDescriptions(prefab);
				}
			}
			else if ((Object)selectedEntityToggle != (Object)null)
			{
				requestSelectedEntityBtn.onClick += delegate
				{
					targetReceptacle.CreateOrder(selectedDepositObjectTag);
					UpdateAvailableAmounts(null);
					UpdateState(null);
				};
				requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringDeposit).ToString();
				targetReceptacle.SetPreview(depositObjectMap[selectedEntityToggle].tag, false);
				bool flag = CanDepositEntity(depositObjectMap[selectedEntityToggle]);
				requestSelectedEntityBtn.isInteractable = flag;
				SetImageToggleState(selectedEntityToggle.toggle, (!flag) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Active);
				ToggleObjectPicker(true);
				GameObject prefab2 = Assets.GetPrefab(selectedDepositObjectTag);
				if ((Object)prefab2 != (Object)null)
				{
					subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingSelection).ToString(), prefab2.GetProperName()));
					SetResultDescriptions(prefab2);
				}
			}
			else
			{
				requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringDeposit).ToString();
				requestSelectedEntityBtn.isInteractable = false;
				ToggleObjectPicker(true);
			}
			UpdateAvailableAmounts(null);
			UpdateListeners();
		}
	}

	private void UpdateListeners()
	{
		if (CheckReceptacleOccupied())
		{
			if (onObjectDestroyedHandle == -1)
			{
				onObjectDestroyedHandle = targetReceptacle.Occupant.gameObject.Subscribe(1969584890, delegate
				{
					UpdateState(null);
				});
			}
		}
		else if (onObjectDestroyedHandle != -1)
		{
			onObjectDestroyedHandle = -1;
		}
	}

	private void OnOccupantValidChanged(object obj)
	{
		if (!((Object)targetReceptacle == (Object)null) && !CheckReceptacleOccupied() && targetReceptacle.GetActiveRequest != null)
		{
			bool flag = false;
			if (depositObjectMap.TryGetValue(selectedEntityToggle, out SelectableEntity value))
			{
				flag = CanDepositEntity(value);
			}
			if (!flag)
			{
				targetReceptacle.CancelActiveRequest();
				ClearSelection();
				UpdateState(null);
				UpdateAvailableAmounts(null);
			}
		}
	}

	private bool CanDepositEntity(SelectableEntity entity)
	{
		return ValidRotationForDeposit(entity.direction) && (!RequiresAvailableAmountToDeposit() || GetAvailableAmount(entity.tag) > 0f) && AdditionalCanDepositTest();
	}

	protected virtual bool AdditionalCanDepositTest()
	{
		return true;
	}

	protected virtual bool RequiresAvailableAmountToDeposit()
	{
		return true;
	}

	private void ClearSelection()
	{
		foreach (KeyValuePair<ReceptacleToggle, SelectableEntity> item in depositObjectMap)
		{
			item.Key.toggle.Deselect();
		}
	}

	private void ToggleObjectPicker(bool Show)
	{
		requestObjectListContainer.SetActive(Show);
		if ((Object)scrollBarContainer != (Object)null)
		{
			scrollBarContainer.SetActive(Show);
		}
		requestObjectList.SetActive(Show);
		activeEntityContainer.SetActive(!Show);
	}

	private void ConfigureActiveEntity(Tag tag)
	{
		GameObject prefab = Assets.GetPrefab(tag);
		string properName = prefab.GetProperName();
		activeEntityContainer.GetComponentInChildrenOnly<LocText>().text = properName;
		activeEntityContainer.transform.GetChild(0).gameObject.GetComponentInChildrenOnly<Image>().sprite = GetEntityIcon(tag);
	}

	protected virtual Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(prefab, "ui", false);
		return uISprite.first;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<SingleEntityReceptacle>() != (Object)null && (Object)target.GetComponent<PlantablePlot>() == (Object)null && (Object)target.GetComponent<EggIncubator>() == (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("The object selected doesn't have a SingleObjectReceptacle!");
		}
		else
		{
			Initialize(component);
			UpdateState(null);
		}
	}

	public override void ClearTarget()
	{
		if ((Object)targetReceptacle != (Object)null)
		{
			if (CheckReceptacleOccupied())
			{
				targetReceptacle.Occupant.gameObject.Unsubscribe(onObjectDestroyedHandle);
				onObjectDestroyedHandle = -1;
			}
			targetReceptacle.Unsubscribe(onStorageChangedHandle);
			onStorageChangedHandle = -1;
			targetReceptacle.Unsubscribe(onOccupantValidChangedHandle);
			onOccupantValidChangedHandle = -1;
			if (targetReceptacle.GetActiveRequest == null)
			{
				targetReceptacle.SetPreview(Tag.Invalid, false);
			}
			SimAndRenderScheduler.instance.Remove(this);
			targetReceptacle = null;
		}
	}

	private void SetImageToggleState(KToggle toggle, ImageToggleState.State state)
	{
		switch (state)
		{
		case ImageToggleState.State.Active:
			toggle.GetComponent<ImageToggleState>().SetActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = defaultMaterial;
			break;
		case ImageToggleState.State.Inactive:
			toggle.GetComponent<ImageToggleState>().SetInactive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = defaultMaterial;
			break;
		case ImageToggleState.State.Disabled:
			toggle.GetComponent<ImageToggleState>().SetDisabled();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = desaturatedMaterial;
			break;
		case ImageToggleState.State.DisabledActive:
			toggle.GetComponent<ImageToggleState>().SetDisabledActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = desaturatedMaterial;
			break;
		}
	}

	public void Render1000ms(float dt)
	{
		CheckAmountsAndUpdate(null);
	}

	private void CheckAmountsAndUpdate(object data)
	{
		if (!((Object)targetReceptacle == (Object)null) && UpdateAvailableAmounts(null))
		{
			UpdateState(null);
		}
	}

	private bool UpdateAvailableAmounts(object data)
	{
		bool result = false;
		foreach (KeyValuePair<ReceptacleToggle, SelectableEntity> item in depositObjectMap)
		{
			if (!DebugHandler.InstantBuildMode && hideUndiscoveredEntities && !WorldInventory.Instance.IsDiscovered(item.Value.tag))
			{
				item.Key.gameObject.SetActive(false);
			}
			else if (!item.Key.gameObject.activeSelf)
			{
				item.Key.gameObject.SetActive(true);
			}
			float availableAmount = GetAvailableAmount(item.Value.tag);
			if (item.Value.lastAmount != availableAmount)
			{
				result = true;
				item.Value.lastAmount = availableAmount;
				item.Key.amount.text = availableAmount.ToString();
			}
			if (!ValidRotationForDeposit(item.Value.direction) || availableAmount <= 0f)
			{
				if ((Object)selectedEntityToggle != (Object)item.Key)
				{
					SetImageToggleState(item.Key.toggle, ImageToggleState.State.Disabled);
				}
				else
				{
					SetImageToggleState(item.Key.toggle, ImageToggleState.State.DisabledActive);
				}
			}
			else if ((Object)selectedEntityToggle != (Object)item.Key)
			{
				SetImageToggleState(item.Key.toggle, ImageToggleState.State.Inactive);
			}
			else
			{
				SetImageToggleState(item.Key.toggle, ImageToggleState.State.Active);
			}
		}
		return result;
	}

	private float GetAvailableAmount(Tag tag)
	{
		return WorldInventory.Instance.GetAmount(tag);
	}

	private bool ValidRotationForDeposit(SingleEntityReceptacle.ReceptacleDirection depositDir)
	{
		return (Object)targetReceptacle.rotatable == (Object)null || depositDir == targetReceptacle.Direction;
	}

	private void ToggleClicked(ReceptacleToggle toggle)
	{
		if (!depositObjectMap.ContainsKey(toggle))
		{
			Debug.LogError("Recipe not found on recipe list.");
		}
		else
		{
			if ((Object)selectedEntityToggle != (Object)null)
			{
				bool flag = CanDepositEntity(depositObjectMap[selectedEntityToggle]);
				requestSelectedEntityBtn.isInteractable = flag;
				SetImageToggleState(selectedEntityToggle.toggle, flag ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
			}
			selectedEntityToggle = toggle;
			entityPreviousSelectionMap[targetReceptacle] = entityToggles.IndexOf(toggle);
			selectedDepositObjectTag = depositObjectMap[toggle].tag;
			UpdateAvailableAmounts(null);
			UpdateState(null);
		}
	}

	private void CreateOrder(bool isInfinite)
	{
		targetReceptacle.CreateOrder(selectedDepositObjectTag);
	}

	private bool CheckReceptacleOccupied()
	{
		if ((Object)targetReceptacle != (Object)null && (Object)targetReceptacle.Occupant != (Object)null)
		{
			return true;
		}
		return false;
	}

	protected virtual void SetResultDescriptions(GameObject go)
	{
		string text = "Entity prefab has no info description component.";
		InfoDescription component = go.GetComponent<InfoDescription>();
		if ((bool)component)
		{
			text = component.description;
		}
		descriptionLabel.SetText(text);
	}

	protected virtual void HideAllDescriptorPanels()
	{
		for (int i = 0; i < descriptorPanels.Count; i++)
		{
			descriptorPanels[i].gameObject.SetActive(false);
		}
	}
}
