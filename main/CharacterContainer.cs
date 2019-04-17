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
	private Transform aptitudeContainer;

	[SerializeField]
	private Transform aptitudeLabel;

	[SerializeField]
	private LocText expectation;

	[SerializeField]
	private LocText expectationRight;

	private List<LocText> expectationLabels;

	private List<LocText> aptitudeLabels;

	private List<LocText> traitLabels;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private KToggle selectButton;

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
		traitLabels = new List<LocText>();
		expectationLabels = new List<LocText>();
		aptitudeLabels = new List<LocText>();
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

	private void GenerateCharacter(bool is_starter)
	{
		int num = 0;
		do
		{
			stats = new MinionStartingStats(is_starter);
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
		if ((UnityEngine.Object)animController == (UnityEngine.Object)null)
		{
			animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("MinionSelectPreview")), contentBody.gameObject, false).GetComponent<KBatchedAnimController>();
			animController.gameObject.SetActive(true);
			KCanvasScaler kCanvasScaler = UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
			animController.animScale = baseCharacterScale * (1f / kCanvasScaler.GetCanvasScale());
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
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
		animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void SetInfoText()
	{
		traitLabels.ForEach(delegate(LocText tl)
		{
			UnityEngine.Object.Destroy(tl.gameObject);
		});
		traitLabels.Clear();
		characterNameTitle.SetTitle(stats.Name);
		for (int i = 1; i < stats.Traits.Count; i++)
		{
			Trait trait = stats.Traits[i];
			LocText locText = (!trait.PositiveTrait) ? badTrait : goodTrait;
			LocText locText2 = Util.KInstantiateUI<LocText>(locText.gameObject, goodTrait.transform.parent.gameObject, false);
			locText2.gameObject.SetActive(true);
			locText2.text = stats.Traits[i].Name;
			locText2.color = ((!trait.PositiveTrait) ? Constants.NEGATIVE_COLOR : Constants.POSITIVE_COLOR);
			locText2.GetComponent<ToolTip>().SetSimpleTooltip(trait.GetTooltip());
			traitLabels.Add(locText2);
		}
		aptitudeLabels.ForEach(delegate(LocText al)
		{
			UnityEngine.Object.Destroy(al.gameObject);
		});
		aptitudeLabels.Clear();
		expectationLabels.ForEach(delegate(LocText el)
		{
			UnityEngine.Object.Destroy(el.gameObject);
		});
		expectationLabels.Clear();
		foreach (Klei.AI.Attribute resource in Db.Get().Attributes.resources)
		{
			if (resource.ShowInUI == Klei.AI.Attribute.Display.Expectation)
			{
				LocText locText3 = Util.KInstantiateUI<LocText>(expectation.gameObject, expectation.transform.parent.gameObject, false);
				locText3.gameObject.SetActive(true);
				AttributeInstance attributeInstance = resource.Lookup(animController);
				locText3.text = string.Format(UI.CHARACTERCONTAINER_NEED, resource.Name, attributeInstance.GetFormattedValue());
				expectationLabels.Add(locText3);
				string tooltip = resource.GetTooltip(attributeInstance);
				locText3.GetComponent<ToolTip>().SetSimpleTooltip(tooltip);
			}
		}
		foreach (KeyValuePair<HashedString, float> skillAptitude in stats.skillAptitudes)
		{
			if (skillAptitude.Value != 0f)
			{
				SkillGroup skillGroup = Db.Get().SkillGroups.Get(skillAptitude.Key);
				if (skillGroup == null)
				{
					Debug.LogWarningFormat("Role group not found for aptitude: {0}", skillAptitude.Key);
				}
				else
				{
					LocText locText4 = Util.KInstantiateUI<LocText>(aptitudeLabel.gameObject, aptitudeContainer.gameObject, false);
					locText4.gameObject.SetActive(true);
					locText4.text = skillGroup.Name;
					string simpleTooltip = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS);
					locText4.GetComponent<ToolTip>().SetSimpleTooltip(simpleTooltip);
					aptitudeLabels.Add(locText4);
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
	}

	private void Reshuffle(bool is_starter)
	{
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null && controller.IsSelected(stats))
		{
			DeselectDeliverable();
		}
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
}
