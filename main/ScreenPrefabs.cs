using System;
using UnityEngine;

public class ScreenPrefabs : KMonoBehaviour
{
	public ControlsScreen ControlsScreen;

	public Hud HudScreen;

	public HoverTextScreen HoverTextScreen;

	public OverlayScreen OverlayScreen;

	public TileScreen TileScreen;

	public SpeedControlScreen SpeedControlScreen;

	public OverviewScreen ManagementScreen;

	public ManagementMenu ManagementMenu;

	public ToolTipScreen ToolTipScreen;

	public DebugPaintElementScreen DebugPaintElementScreen;

	public UserMenuScreen UserMenuScreen;

	public KButtonMenu OwnerScreen;

	public EnergyInfoScreen EnergyInfoScreen;

	public KButtonMenu ButtonGrid;

	public NameDisplayScreen NameDisplayScreen;

	public ConfirmDialogScreen ConfirmDialogScreen;

	public CustomizableDialogScreen CustomizableDialogScreen;

	public InfoDialogScreen InfoDialogScreen;

	public FileNameDialog FileNameDialog;

	public TagFilterScreen TagFilterScreen;

	public ResearchScreen ResearchScreen;

	public MessageDialogFrame MessageDialogFrame;

	public ResourceCategoryScreen ResourceCategoryScreen;

	public LanguageOptionsScreen languageOptionsScreen;

	public ModsScreen modsMenu;

	public GameObject GameOverScreen;

	public GameObject StatusItemIndicatorScreen;

	public GameObject CollapsableContentPanel;

	public GameObject DescriptionLabel;

	public LoadingOverlay loadingOverlay;

	public LoadScreen LoadScreen;

	public InspectSaveScreen InspectSaveScreen;

	public OptionsMenuScreen OptionsScreen;

	public WorldGenScreen WorldGenScreen;

	public ModeSelectScreen ModeSelectScreen;

	public NewGameSettingsScreen NewGameSettingsScreen;

	public static ScreenPrefabs Instance
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void ConfirmDoAction(string message, System.Action action, Transform parent)
	{
		ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(Instance.ConfirmDialogScreen.gameObject, parent.gameObject);
		confirmDialogScreen.PopupConfirmDialog(message, action, delegate
		{
		}, null, null, null, null, null, null);
	}
}
