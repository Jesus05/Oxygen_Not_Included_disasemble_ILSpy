using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarePackageContainer : KScreen, ITelepadDeliverableContainer
{
	[Serializable]
	public struct ProfessionIcon
	{
		public string professionName;

		public Sprite iconImg;
	}

	[Header("UI References")]
	[SerializeField]
	private GameObject contentBody;

	[SerializeField]
	private LocText characterName;

	public GameObject selectedBorder;

	[SerializeField]
	private Image titleBar;

	[SerializeField]
	private Color selectedTitleColor;

	[SerializeField]
	private Color deselectedTitleColor;

	[SerializeField]
	private KButton reshuffleButton;

	private KBatchedAnimController animController;

	[SerializeField]
	private LocText itemName;

	[SerializeField]
	private LocText quantity;

	[SerializeField]
	private LocText currentQuantity;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private KToggle selectButton;

	private CarePackageInfo info;

	private CharacterSelectionController controller;

	private static List<ITelepadDeliverableContainer> containers;

	[SerializeField]
	private Sprite enabledSpr;

	[SerializeField]
	private List<ProfessionIcon> professionIcons;

	private Dictionary<string, Sprite> professionIconMap;

	public float baseCharacterScale = 0.38f;

	private List<GameObject> entryIcons = new List<GameObject>();

	public CarePackageInfo Info => info;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Initialize();
		reshuffleButton.onClick += delegate
		{
			Reshuffle(true);
		};
		StartCoroutine(DelayedGeneration());
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	private IEnumerator DelayedGeneration()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((UnityEngine.Object)animController != (UnityEngine.Object)null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
			animController.gameObject.DeleteObject();
			animController = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			CharacterSelectionController characterSelectionController = controller;
			characterSelectionController.OnLimitReachedEvent = (System.Action)Delegate.Remove(characterSelectionController.OnLimitReachedEvent, new System.Action(OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = controller;
			characterSelectionController2.OnLimitUnreachedEvent = (System.Action)Delegate.Remove(characterSelectionController2.OnLimitUnreachedEvent, new System.Action(OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Remove(characterSelectionController3.OnReshuffleEvent, new Action<bool>(Reshuffle));
		}
		if ((UnityEngine.Object)animController != (UnityEngine.Object)null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
		}
	}

	private void Initialize()
	{
		professionIconMap = new Dictionary<string, Sprite>();
		professionIcons.ForEach(delegate(ProfessionIcon ic)
		{
			professionIconMap.Add(ic.professionName, ic.iconImg);
		});
		if (containers == null)
		{
			containers = new List<ITelepadDeliverableContainer>();
		}
		containers.Add(this);
	}

	private void GenerateCharacter(bool is_starter)
	{
		int num = 0;
		do
		{
			info = Immigration.Instance.RandomCarePackage();
			num++;
		}
		while (IsCharacterRedundant() && num < 20);
		if ((UnityEngine.Object)animController != (UnityEngine.Object)null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
			UnityEngine.Object.Destroy(animController.gameObject);
			animController = null;
		}
		SetAnimator();
		SetInfoText();
		selectButton.ClearOnClick();
		if (!controller.IsStarterMinion)
		{
			selectButton.onClick += delegate
			{
				SelectDeliverable();
			};
		}
	}

	private void OnResize()
	{
		KCanvasScaler kCanvasScaler = UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
		animController.animScale = baseCharacterScale * (1f / kCanvasScaler.GetCanvasScale());
	}

	private void SetAnimator()
	{
		GameObject prefab = Assets.GetPrefab(info.id.ToTag());
		EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(info.id);
		int num = (ElementLoader.FindElementByName(info.id) != null) ? 1 : ((foodInfo == null) ? ((int)info.quantity) : ((int)(info.quantity % foodInfo.CaloriesPerUnit)));
		if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
		{
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(contentBody, contentBody.transform.parent.gameObject, false);
				gameObject.SetActive(true);
				Image component = gameObject.GetComponent<Image>();
				Tuple<Sprite, Color> uISprite = Def.GetUISprite(prefab, "ui", false);
				component.sprite = uISprite.first;
				component.color = uISprite.second;
				entryIcons.Add(gameObject);
				if (num > 1)
				{
					int num2 = 0;
					int num3 = 0;
					int num4;
					if (num % 2 == 1)
					{
						num4 = Mathf.CeilToInt((float)(num / 2));
						num2 = num4 - i;
						num3 = ((num2 > 0) ? 1 : (-1));
						num2 = Mathf.Abs(num2);
					}
					else
					{
						num4 = num / 2 - 1;
						if (i <= num4)
						{
							num2 = Mathf.Abs(num4 - i);
							num3 = -1;
						}
						else
						{
							num2 = Mathf.Abs(num4 + 1 - i);
							num3 = 1;
						}
					}
					int num5 = 0;
					if (num % 2 == 0)
					{
						num5 = ((i > num4) ? 6 : (-6));
						gameObject.transform.SetPosition(gameObject.transform.position += new Vector3((float)num5, 0f, 0f));
					}
					gameObject.transform.localScale = new Vector3(1f - (float)num2 * 0.1f, 1f - (float)num2 * 0.1f, 1f);
					gameObject.transform.Rotate(0f, 0f, 3f * (float)num2 * (float)num3);
					gameObject.transform.SetPosition(gameObject.transform.position + new Vector3(25f * (float)num2 * (float)num3, 5f * (float)num2) + new Vector3((float)num5, 0f, 0f));
					gameObject.GetComponent<Canvas>().sortingOrder = num - num2;
				}
			}
		}
		else
		{
			GameObject gameObject2 = Util.KInstantiateUI(contentBody, contentBody.transform.parent.gameObject, false);
			gameObject2.SetActive(true);
			Image component2 = gameObject2.GetComponent<Image>();
			component2.sprite = Def.GetUISpriteFromMultiObjectAnim(ElementLoader.GetElement(info.id.ToTag()).substance.anim, "ui", false, "");
			component2.color = ElementLoader.GetElement(info.id.ToTag()).substance.uiColour;
			entryIcons.Add(gameObject2);
		}
	}

	private string GetSpawnableName()
	{
		GameObject prefab = Assets.GetPrefab(info.id);
		if (!((UnityEngine.Object)prefab == (UnityEngine.Object)null))
		{
			return prefab.GetProperName();
		}
		Element element = ElementLoader.FindElementByName(info.id);
		if (element == null)
		{
			return "";
		}
		return element.substance.name;
	}

	private string GetSpawnableQuantityOnly()
	{
		if (ElementLoader.GetElement(info.id.ToTag()) == null)
		{
			if (Game.Instance.ediblesManager.GetFoodInfo(info.id) == null)
			{
				return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, info.quantity.ToString());
			}
			return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedCaloriesForItem(info.id, info.quantity, GameUtil.TimeSlice.None, true));
		}
		return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedMass(info.quantity, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetCurrentQuantity()
	{
		if (ElementLoader.GetElement(info.id.ToTag()) == null)
		{
			if (Game.Instance.ediblesManager.GetFoodInfo(info.id) == null)
			{
				float amount = WorldInventory.Instance.GetAmount(info.id.ToTag());
				return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, amount.ToString());
			}
			float calories = RationTracker.Get().CountRationsByFoodType(info.id, true);
			return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true));
		}
		float amount2 = WorldInventory.Instance.GetAmount(info.id.ToTag());
		return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, GameUtil.GetFormattedMass(amount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetSpawnableQuantity()
	{
		if (ElementLoader.GetElement(info.id.ToTag()) == null)
		{
			if (Game.Instance.ediblesManager.GetFoodInfo(info.id) == null)
			{
				return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, Assets.GetPrefab(info.id).GetProperName(), info.quantity.ToString());
			}
			return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, GameUtil.GetFormattedCaloriesForItem(info.id, info.quantity, GameUtil.TimeSlice.None, true), Assets.GetPrefab(info.id).GetProperName());
		}
		return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, GameUtil.GetFormattedMass(info.quantity, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), Assets.GetPrefab(info.id).GetProperName());
	}

	private string GetSpawnableDescription()
	{
		Element element = ElementLoader.GetElement(info.id.ToTag());
		if (element == null)
		{
			GameObject prefab = Assets.GetPrefab(info.id);
			if (!((UnityEngine.Object)prefab == (UnityEngine.Object)null))
			{
				InfoDescription component = prefab.GetComponent<InfoDescription>();
				if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
				{
					return prefab.GetProperName();
				}
				return component.description;
			}
			return "";
		}
		return element.Description();
	}

	private void SetInfoText()
	{
		characterName.SetText(GetSpawnableName());
		description.SetText(GetSpawnableDescription());
		itemName.SetText(GetSpawnableName());
		quantity.SetText(GetSpawnableQuantityOnly());
		currentQuantity.SetText(GetCurrentQuantity());
	}

	public void SelectDeliverable()
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			controller.AddDeliverable(info);
		}
		if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
		{
			MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 1f, true);
		}
		selectButton.GetComponent<ImageToggleState>().SetActive();
		selectButton.ClearOnClick();
		selectButton.onClick += delegate
		{
			DeselectDeliverable();
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 0f, true);
			}
		};
		selectedBorder.SetActive(true);
		titleBar.color = selectedTitleColor;
	}

	public void DeselectDeliverable()
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			controller.RemoveDeliverable(info);
		}
		ImageToggleState component = selectButton.GetComponent<ImageToggleState>();
		component.SetInactive();
		selectButton.Deselect();
		selectButton.ClearOnClick();
		selectButton.onClick += delegate
		{
			SelectDeliverable();
		};
		selectedBorder.SetActive(false);
		titleBar.color = deselectedTitleColor;
	}

	private void OnReplacedEvent(ITelepadDeliverable stats)
	{
		if (stats == info)
		{
			DeselectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitReached()
	{
		if (!((UnityEngine.Object)controller != (UnityEngine.Object)null) || !controller.IsSelected(info))
		{
			selectButton.ClearOnClick();
			if (controller.AllowsReplacing)
			{
				selectButton.onClick += ReplaceCharacterSelection;
			}
			else
			{
				selectButton.onClick += CantSelectCharacter;
			}
		}
	}

	private void CantSelectCharacter()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	private void ReplaceCharacterSelection()
	{
		if (!((UnityEngine.Object)controller == (UnityEngine.Object)null))
		{
			controller.RemoveLast();
			SelectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitUnReached()
	{
		if (!((UnityEngine.Object)controller != (UnityEngine.Object)null) || !controller.IsSelected(info))
		{
			selectButton.ClearOnClick();
			selectButton.onClick += delegate
			{
				SelectDeliverable();
			};
		}
	}

	public void SetReshufflingState(bool enable)
	{
		reshuffleButton.gameObject.SetActive(enable);
	}

	private void Reshuffle(bool is_starter)
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null && controller.IsSelected(info))
		{
			DeselectDeliverable();
		}
		ClearEntryIcons();
		GenerateCharacter(is_starter);
	}

	public void SetController(CharacterSelectionController csc)
	{
		if (!((UnityEngine.Object)csc == (UnityEngine.Object)controller))
		{
			controller = csc;
			CharacterSelectionController characterSelectionController = controller;
			characterSelectionController.OnLimitReachedEvent = (System.Action)Delegate.Combine(characterSelectionController.OnLimitReachedEvent, new System.Action(OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = controller;
			characterSelectionController2.OnLimitUnreachedEvent = (System.Action)Delegate.Combine(characterSelectionController2.OnLimitUnreachedEvent, new System.Action(OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Combine(characterSelectionController3.OnReshuffleEvent, new Action<bool>(Reshuffle));
			CharacterSelectionController characterSelectionController4 = controller;
			characterSelectionController4.OnReplacedEvent = (Action<ITelepadDeliverable>)Delegate.Combine(characterSelectionController4.OnReplacedEvent, new Action<ITelepadDeliverable>(OnReplacedEvent));
		}
	}

	public void DisableSelectButton()
	{
		selectButton.soundPlayer.AcceptClickCondition = (() => false);
		selectButton.GetComponent<ImageToggleState>().SetDisabled();
		selectButton.soundPlayer.Enabled = false;
	}

	private bool IsCharacterRedundant()
	{
		foreach (ITelepadDeliverableContainer container in containers)
		{
			if (!object.ReferenceEquals(container, this))
			{
				CarePackageContainer carePackageContainer = container as CarePackageContainer;
				if ((UnityEngine.Object)carePackageContainer != (UnityEngine.Object)null && carePackageContainer.info == info)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GetValueColor(bool isPositive)
	{
		return (!isPositive) ? "<color=#ff2222ff>" : "<color=green>";
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(Action.Escape))
		{
			controller.OnPressBack();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	protected override void OnCmpEnable()
	{
		base.OnActivate();
		if (info != null)
		{
			ClearEntryIcons();
			SetAnimator();
			SetInfoText();
		}
	}

	private void ClearEntryIcons()
	{
		for (int i = 0; i < entryIcons.Count; i++)
		{
			UnityEngine.Object.Destroy(entryIcons[i]);
		}
	}
}
