using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : KScreen, ITelepadDeliverableContainer
{
	[Serializable]
	public struct ProfessionIcon
	{
		public string professionName;

		public Sprite iconImg;
	}

	[SerializeField]
	private GameObject contentBody;

	[SerializeField]
	private LocText characterName;

	[SerializeField]
	private EditableTitleBar characterNameTitle;

	[SerializeField]
	private LocText characterJob;

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
	private GameObject iconGroup;

	private List<GameObject> iconGroups;

	[SerializeField]
	private LocText goodTrait;

	[SerializeField]
	private LocText badTrait;

	[SerializeField]
	private GameObject aptitudeEntry;

	[SerializeField]
	private Transform aptitudeLabel;

	[SerializeField]
	private Transform attributeLabelAptitude;

	[SerializeField]
	private Transform attributeLabelTrait;

	[SerializeField]
	private LocText expectationRight;

	private List<LocText> expectationLabels;

	[SerializeField]
	private DropDown archetypeDropDown;

	[SerializeField]
	private Image selectedArchetypeIcon;

	[SerializeField]
	private Sprite noArchetypeIcon;

	[SerializeField]
	private Sprite dropdownArrowIcon;

	private string guaranteedAptitudeID;

	private List<GameObject> aptitudeEntries;

	private List<GameObject> traitEntries;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private KToggle selectButton;

	[SerializeField]
	private KBatchedAnimController fxAnim;

	private MinionStartingStats stats;

	private CharacterSelectionController controller;

	private static List<CharacterContainer> containers;

	private KAnimFile idle_anim;

	[HideInInspector]
	public bool addMinionToIdentityList = true;

	[SerializeField]
	private Sprite enabledSpr;

	[SerializeField]
	private List<ProfessionIcon> professionIcons;

	private Dictionary<string, Sprite> professionIconMap;

	private static readonly HashedString[] idleAnims = new HashedString[7]
	{
		"anim_idle_healthy_kanim",
		"anim_idle_susceptible_kanim",
		"anim_idle_keener_kanim",
		"anim_idle_coaster_kanim",
		"anim_idle_fastfeet_kanim",
		"anim_idle_breatherdeep_kanim",
		"anim_idle_breathershallow_kanim"
	};

	public float baseCharacterScale = 0.38f;

	public MinionStartingStats Stats => stats;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Initialize();
		characterNameTitle.OnStartedEditing += OnStartedEditing;
		characterNameTitle.OnNameChanged += OnNameChanged;
		reshuffleButton.onClick += delegate
		{
			Reshuffle(true);
		};
		List<IListableOption> list = new List<IListableOption>();
		List<SkillGroup> list2 = new List<SkillGroup>(Db.Get().SkillGroups.resources);
		foreach (SkillGroup item in list2)
		{
			list.Add(item);
		}
		archetypeDropDown.Initialize(list, OnArchetypeEntryClick, archetypeDropDownSort, archetypeDropEntryRefreshAction, false, null);
		archetypeDropDown.CustomizeEmptyRow(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_NOARCHETYPESELECTED"), noArchetypeIcon);
		StartCoroutine(DelayedGeneration());
	}

	public void ForceStopEditingTitle()
	{
		characterNameTitle.ForceStopEditing();
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
		iconGroups = new List<GameObject>();
		traitEntries = new List<GameObject>();
		expectationLabels = new List<LocText>();
		aptitudeEntries = new List<GameObject>();
		if (containers == null)
		{
			containers = new List<CharacterContainer>();
		}
		containers.Add(this);
	}

	private void OnNameChanged(string newName)
	{
		stats.Name = newName;
		stats.personality.Name = newName;
		description.text = stats.personality.description;
	}

	private void OnStartedEditing()
	{
		KScreenManager.Instance.RefreshStack();
	}

	private void GenerateCharacter(bool is_starter, string guaranteedAptitudeID = null)
	{
		int num = 0;
		do
		{
			stats = new MinionStartingStats(is_starter, guaranteedAptitudeID);
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
		StartCoroutine(SetAttributes());
		selectButton.ClearOnClick();
		if (!controller.IsStarterMinion)
		{
			selectButton.enabled = true;
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
		Transform transform = animController.transform.parent.gameObject.transform.Find("BG");
		KBatchedAnimController kBatchedAnimController = (!((UnityEngine.Object)transform != (UnityEngine.Object)null)) ? null : transform.gameObject.GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)kBatchedAnimController != (UnityEngine.Object)null)
		{
			kBatchedAnimController.animScale = baseCharacterScale * (1f / kCanvasScaler.GetCanvasScale());
		}
	}

	private void SetAnimator()
	{
		if ((UnityEngine.Object)animController == (UnityEngine.Object)null)
		{
			animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("MinionSelectPreview")), contentBody.gameObject, false).GetComponent<KBatchedAnimController>();
			animController.gameObject.SetActive(true);
			KCanvasScaler kCanvasScaler = UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
			animController.animScale = baseCharacterScale * (1f / kCanvasScaler.GetCanvasScale());
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
			Transform transform = animController.transform.parent.gameObject.transform.Find("BG");
			KBatchedAnimController kBatchedAnimController = (!((UnityEngine.Object)transform != (UnityEngine.Object)null)) ? null : transform.gameObject.GetComponent<KBatchedAnimController>();
			if ((UnityEngine.Object)kBatchedAnimController != (UnityEngine.Object)null)
			{
				kBatchedAnimController.animScale = baseCharacterScale * (1f / kCanvasScaler.GetCanvasScale());
			}
		}
		stats.ApplyTraits(animController.gameObject);
		stats.ApplyRace(animController.gameObject);
		stats.ApplyAccessories(animController.gameObject);
		stats.ApplyExperience(animController.gameObject);
		HashedString name = idleAnims[UnityEngine.Random.Range(0, idleAnims.Length)];
		idle_anim = Assets.GetAnim(name);
		if ((UnityEngine.Object)idle_anim != (UnityEngine.Object)null)
		{
			animController.AddAnimOverrides(idle_anim, 0f);
		}
		HashedString name2 = new HashedString("crewSelect_fx_kanim");
		KAnimFile anim = Assets.GetAnim(name2);
		if ((UnityEngine.Object)anim != (UnityEngine.Object)null)
		{
			animController.AddAnimOverrides(anim, 0f);
		}
		animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void SetInfoText()
	{
		traitEntries.ForEach(delegate(GameObject tl)
		{
			UnityEngine.Object.Destroy(tl.gameObject);
		});
		traitEntries.Clear();
		characterNameTitle.SetTitle(stats.Name);
		for (int i = 1; i < stats.Traits.Count; i++)
		{
			Trait trait = stats.Traits[i];
			LocText locText = (!trait.PositiveTrait) ? badTrait : goodTrait;
			LocText locText2 = Util.KInstantiateUI<LocText>(locText.gameObject, locText.transform.parent.gameObject, false);
			locText2.gameObject.SetActive(true);
			locText2.text = stats.Traits[i].Name;
			locText2.color = ((!trait.PositiveTrait) ? Constants.NEGATIVE_COLOR : Constants.POSITIVE_COLOR);
			locText2.GetComponent<ToolTip>().SetSimpleTooltip(trait.description);
			for (int num = 0; num < trait.SelfModifiers.Count; num++)
			{
				GameObject gameObject = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject.SetActive(true);
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				string format = (!(trait.SelfModifiers[num].Value > 0f)) ? UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED : UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED;
				componentInChildren.text = string.Format(format, Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[num].AttributeId.ToUpper() + ".NAME"));
				if (!(trait.SelfModifiers[num].AttributeId == "GermResistance"))
				{
					goto IL_01c2;
				}
				goto IL_01c2;
				IL_01c2:
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(trait.SelfModifiers[num].AttributeId);
				string text = attribute.Description;
				string text2 = text;
				text = text2 + "\n\n" + Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[num].AttributeId.ToUpper() + ".NAME") + ": " + trait.SelfModifiers[num].GetFormattedString(null);
				List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(attribute);
				for (int j = 0; j < convertersForAttribute.Count; j++)
				{
					string text3 = convertersForAttribute[j].DescriptionFromAttribute(convertersForAttribute[j].multiplier * trait.SelfModifiers[num].Value, null);
					if (text3 != "")
					{
						text = text + "\n    • " + text3;
					}
				}
				componentInChildren.GetComponent<ToolTip>().SetSimpleTooltip(text);
				traitEntries.Add(gameObject);
			}
			if (trait.disabledChoreGroups != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject2.SetActive(true);
				LocText componentInChildren2 = gameObject2.GetComponentInChildren<LocText>();
				componentInChildren2.text = trait.GetDisabledChoresString(false);
				string text4 = "";
				string text5 = "";
				for (int k = 0; k < trait.disabledChoreGroups.Length; k++)
				{
					if (k > 0)
					{
						text4 += ", ";
						text5 += "\n";
					}
					text4 += trait.disabledChoreGroups[k].Name;
					text5 += trait.disabledChoreGroups[k].description;
				}
				componentInChildren2.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(DUPLICANTS.TRAITS.CANNOT_DO_TASK_TOOLTIP, text4, text5));
				traitEntries.Add(gameObject2);
			}
			if (trait.ignoredEffects != null && trait.ignoredEffects.Length > 0)
			{
				GameObject gameObject3 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject3.SetActive(true);
				LocText componentInChildren3 = gameObject3.GetComponentInChildren<LocText>();
				componentInChildren3.text = trait.GetIgnoredEffectsString(false);
				string text6 = "";
				string text7 = "";
				for (int l = 0; l < trait.ignoredEffects.Length; l++)
				{
					if (l > 0)
					{
						text6 += ", ";
						text7 += "\n";
					}
					text6 += Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[l].ToUpper() + ".NAME");
					text7 += Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[l].ToUpper() + ".CAUSE");
				}
				componentInChildren3.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS_TOOLTIP, text6, text7));
				traitEntries.Add(gameObject3);
			}
			if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC", out StringEntry result))
			{
				GameObject gameObject4 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject4.SetActive(true);
				LocText componentInChildren4 = gameObject4.GetComponentInChildren<LocText>();
				componentInChildren4.text = result.String;
				componentInChildren4.GetComponent<ToolTip>().SetSimpleTooltip(Strings.Get("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC_TOOLTIP"));
				traitEntries.Add(gameObject4);
			}
			traitEntries.Add(locText2.gameObject);
		}
		aptitudeEntries.ForEach(delegate(GameObject al)
		{
			UnityEngine.Object.Destroy(al.gameObject);
		});
		aptitudeEntries.Clear();
		expectationLabels.ForEach(delegate(LocText el)
		{
			UnityEngine.Object.Destroy(el.gameObject);
		});
		expectationLabels.Clear();
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in stats.skillAptitudes)
		{
			if (skillAptitude.Value != 0f)
			{
				SkillGroup skillGroup = Db.Get().SkillGroups.Get(skillAptitude.Key.IdHash);
				if (skillGroup == null)
				{
					Debug.LogWarningFormat("Role group not found for aptitude: {0}", skillAptitude.Key);
				}
				else
				{
					GameObject gameObject5 = Util.KInstantiateUI(aptitudeEntry.gameObject, aptitudeEntry.transform.parent.gameObject, false);
					LocText locText3 = Util.KInstantiateUI<LocText>(aptitudeLabel.gameObject, gameObject5, false);
					locText3.gameObject.SetActive(true);
					locText3.text = skillGroup.Name;
					string text8 = "";
					if (skillGroup.choreGroupID != "")
					{
						ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(skillGroup.choreGroupID);
						text8 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION_CHOREGROUP, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS, choreGroup.description);
					}
					else
					{
						text8 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS);
					}
					locText3.GetComponent<ToolTip>().SetSimpleTooltip(text8);
					float num2 = (float)DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[stats.skillAptitudes.Count - 1];
					LocText locText4 = Util.KInstantiateUI<LocText>(attributeLabelAptitude.gameObject, gameObject5, false);
					locText4.gameObject.SetActive(true);
					locText4.text = "+" + num2 + " " + skillAptitude.Key.relevantAttributes[0].Name;
					string text9 = skillAptitude.Key.relevantAttributes[0].Description;
					string text2 = text9;
					text9 = text2 + "\n\n" + skillAptitude.Key.relevantAttributes[0].Name + ": +" + DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[stats.skillAptitudes.Count - 1];
					List<AttributeConverter> convertersForAttribute2 = Db.Get().AttributeConverters.GetConvertersForAttribute(skillAptitude.Key.relevantAttributes[0]);
					for (int m = 0; m < convertersForAttribute2.Count; m++)
					{
						text9 = text9 + "\n    • " + convertersForAttribute2[m].DescriptionFromAttribute(convertersForAttribute2[m].multiplier * num2, null);
					}
					locText4.GetComponent<ToolTip>().SetSimpleTooltip(text9);
					gameObject5.gameObject.SetActive(true);
					aptitudeEntries.Add(gameObject5);
				}
			}
		}
		if (stats.stressTrait != null)
		{
			LocText locText5 = Util.KInstantiateUI<LocText>(expectationRight.gameObject, expectationRight.transform.parent.gameObject, false);
			locText5.gameObject.SetActive(true);
			locText5.text = string.Format(UI.CHARACTERCONTAINER_STRESSTRAIT, stats.stressTrait.Name);
			locText5.GetComponent<ToolTip>().SetSimpleTooltip(stats.stressTrait.GetTooltip());
			expectationLabels.Add(locText5);
		}
		if (stats.congenitaltrait != null)
		{
			LocText locText6 = Util.KInstantiateUI<LocText>(expectationRight.gameObject, expectationRight.transform.parent.gameObject, false);
			locText6.gameObject.SetActive(true);
			locText6.text = string.Format(UI.CHARACTERCONTAINER_CONGENITALTRAIT, stats.congenitaltrait.Name);
			locText6.GetComponent<ToolTip>().SetSimpleTooltip(stats.congenitaltrait.GetTooltip());
			expectationLabels.Add(locText6);
		}
		description.text = stats.personality.description;
	}

	private IEnumerator SetAttributes()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void SelectDeliverable()
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			controller.AddDeliverable(stats);
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
		animController.Play("cheer_pre", KAnim.PlayMode.Once, 1f, 0f);
		animController.Play("cheer_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void DeselectDeliverable()
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			controller.RemoveDeliverable(stats);
		}
		selectButton.GetComponent<ImageToggleState>().SetInactive();
		selectButton.Deselect();
		selectButton.ClearOnClick();
		selectButton.onClick += delegate
		{
			SelectDeliverable();
		};
		selectedBorder.SetActive(false);
		titleBar.color = deselectedTitleColor;
		animController.Queue("cheer_pst", KAnim.PlayMode.Once, 1f, 0f);
		animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void OnReplacedEvent(ITelepadDeliverable deliverable)
	{
		if (deliverable == stats)
		{
			DeselectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitReached()
	{
		if (!((UnityEngine.Object)controller != (UnityEngine.Object)null) || !controller.IsSelected(stats))
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
		if (!((UnityEngine.Object)controller != (UnityEngine.Object)null) || !controller.IsSelected(stats))
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
		archetypeDropDown.gameObject.SetActive(enable);
	}

	private void Reshuffle(bool is_starter)
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null && controller.IsSelected(stats))
		{
			DeselectDeliverable();
		}
		if ((UnityEngine.Object)fxAnim != (UnityEngine.Object)null)
		{
			fxAnim.Play("loop", KAnim.PlayMode.Once, 1f, 0f);
		}
		GenerateCharacter(is_starter, guaranteedAptitudeID);
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
		return (UnityEngine.Object)containers.Find((CharacterContainer c) => (UnityEngine.Object)c != (UnityEngine.Object)null && c.stats != null && (UnityEngine.Object)c != (UnityEngine.Object)this && c.stats.Name == stats.Name) != (UnityEngine.Object)null || Components.LiveMinionIdentities.Items.Any((MinionIdentity id) => id.GetProperName() == stats.Name);
	}

	public string GetValueColor(bool isPositive)
	{
		return (!isPositive) ? "<color=#ff2222ff>" : "<color=green>";
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(Action.Escape))
		{
			characterNameTitle.ForceStopEditing();
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
		if (stats != null)
		{
			SetAnimator();
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		characterNameTitle.ForceStopEditing();
	}

	private void OnArchetypeEntryClick(IListableOption skill, object data)
	{
		if (skill != null)
		{
			SkillGroup skillGroup = skill as SkillGroup;
			guaranteedAptitudeID = skillGroup.Id;
			selectedArchetypeIcon.sprite = Assets.GetSprite(skillGroup.choreGroupIcon);
			Reshuffle(true);
		}
		else
		{
			guaranteedAptitudeID = null;
			selectedArchetypeIcon.sprite = dropdownArrowIcon;
			Reshuffle(true);
		}
	}

	private int archetypeDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		if (!b.Equals("Random"))
		{
			return b.GetProperName().CompareTo(a.GetProperName());
		}
		return -1;
	}

	private void archetypeDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillGroup skillGroup = entry.entryData as SkillGroup;
			entry.image.sprite = Assets.GetSprite(skillGroup.choreGroupIcon);
		}
	}
}
