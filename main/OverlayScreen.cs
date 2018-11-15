using FMOD.Studio;
using FMODUnity;
using OverlayModes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayScreen : KMonoBehaviour
{
	public static HashSet<Tag> WireIDs = new HashSet<Tag>();

	public static HashSet<Tag> GasVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> LiquidVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> HarvestableIDs = new HashSet<Tag>();

	public static HashSet<Tag> DiseaseIDs = new HashSet<Tag>();

	public static HashSet<Tag> SuitIDs = new HashSet<Tag>();

	public static HashSet<Tag> SolidConveyorIDs = new HashSet<Tag>();

	[EventRef]
	[SerializeField]
	public string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	public static OverlayScreen Instance;

	[Header("Power")]
	[SerializeField]
	private Canvas powerLabelParent;

	[SerializeField]
	private LocText powerLabelPrefab;

	[SerializeField]
	private BatteryUI batUIPrefab;

	[SerializeField]
	private Vector3 powerLabelOffset;

	[SerializeField]
	private Vector3 batteryUIOffset;

	[SerializeField]
	private Vector3 batteryUITransformerOffset;

	[SerializeField]
	private Vector3 batteryUISmallTransformerOffset;

	[SerializeField]
	private Color consumerColour;

	[SerializeField]
	private Color generatorColour;

	[SerializeField]
	private Color buildingDisabledColour = Color.gray;

	[Header("Circuits")]
	[SerializeField]
	private Color32 circuitUnpoweredColour;

	[SerializeField]
	private Color32 circuitSafeColour;

	[SerializeField]
	private Color32 circuitStrainingColour;

	[Header("Crops")]
	[SerializeField]
	private GameObject harvestableNotificationPrefab;

	[Header("Disease")]
	[SerializeField]
	private GameObject diseaseOverlayPrefab;

	[Header("Suit")]
	[SerializeField]
	private GameObject suitOverlayPrefab;

	[Header("ToolTip")]
	[SerializeField]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	[Header("Logic")]
	[SerializeField]
	private LogicModeUI logicModeUIPrefab;

	public Action<SimViewMode> OnOverlayChanged;

	private Mode currentMode = new None();

	private Dictionary<SimViewMode, Mode> modes = new Dictionary<SimViewMode, Mode>();

	public SimViewMode mode => currentMode.ViewMode();

	protected override void OnPrefabInit()
	{
		Instance = this;
		powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
	}

	protected override void OnLoadLevel()
	{
		currentMode = null;
		harvestableNotificationPrefab = null;
		powerLabelParent = null;
		Instance = null;
		Mode.Clear();
		modes = null;
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		techViewSound = KFMOD.CreateInstance(techViewSoundPath);
		techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
		RegisterModes();
	}

	private void RegisterModes()
	{
		modes.Clear();
		None none = new None();
		RegisterMode(none);
		RegisterMode(new Oxygen());
		RegisterMode(new Power(powerLabelParent, powerLabelPrefab, batUIPrefab, powerLabelOffset, batteryUIOffset, batteryUITransformerOffset, batteryUISmallTransformerOffset, consumerColour, generatorColour, buildingDisabledColour, circuitUnpoweredColour, circuitSafeColour, circuitStrainingColour));
		RegisterMode(new Temperature());
		RegisterMode(new ThermalConductivity());
		RegisterMode(new OverlayModes.Light());
		RegisterMode(new LiquidConduitMode());
		RegisterMode(new GasConduitMode());
		RegisterMode(new Decor());
		RegisterMode(new Disease(powerLabelParent, diseaseOverlayPrefab));
		RegisterMode(new OverlayModes.Crop(powerLabelParent, harvestableNotificationPrefab));
		RegisterMode(new Harvest());
		RegisterMode(new Priorities());
		RegisterMode(new HeatFlow());
		RegisterMode(new Rooms());
		RegisterMode(new Suit(powerLabelParent, suitOverlayPrefab));
		RegisterMode(new Logic(logicModeUIPrefab));
		RegisterMode(new SolidConveyorMode());
		IEnumerator enumerator = Enum.GetValues(typeof(SimViewMode)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SimViewMode key = (SimViewMode)enumerator.Current;
				if (!modes.ContainsKey(key))
				{
					modes[key] = none;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void RegisterMode(Mode mode)
	{
		modes[mode.ViewMode()] = mode;
	}

	private void LateUpdate()
	{
		currentMode.Update();
	}

	public void ToggleOverlay(SimViewMode newMode)
	{
		bool flag = (currentMode.ViewMode() != newMode) ? true : false;
		if (newMode != 0)
		{
			ManagementMenu.Instance.CloseAll();
		}
		currentMode.Disable();
		if (newMode != currentMode.ViewMode() && newMode == SimViewMode.None)
		{
			ManagementMenu.Instance.CloseAll();
		}
		ResourceCategoryScreen.Instance.Show(newMode == SimViewMode.None);
		SimDebugView.Instance.SetMode(newMode);
		currentMode = modes[newMode];
		currentMode.Enable();
		if (flag)
		{
			UpdateOverlaySounds();
		}
		if (currentMode.ViewMode() == SimViewMode.None)
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicOverlayInactive();
			techViewSound.stop(STOP_MODE.ALLOWFADEOUT);
			techViewSoundPlaying = false;
		}
		else if (!techViewSoundPlaying)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
			MusicManager.instance.SetDynamicMusicOverlayActive();
			techViewSound.start();
			techViewSound.setParameterValue("View", (float)currentMode.ViewMode());
			techViewSoundPlaying = true;
		}
		if (OnOverlayChanged != null)
		{
			OnOverlayChanged(currentMode.ViewMode());
		}
		ActivateLegend();
	}

	private void ActivateLegend()
	{
		if (!((UnityEngine.Object)OverlayLegend.Instance == (UnityEngine.Object)null))
		{
			OverlayLegend.Instance.SetLegend(currentMode.ViewMode(), false);
		}
	}

	public void Refresh()
	{
		LateUpdate();
	}

	public SimViewMode GetMode()
	{
		return currentMode.ViewMode();
	}

	private void UpdateOverlaySounds()
	{
		string soundName = currentMode.GetSoundName();
		if (soundName != string.Empty)
		{
			soundName = GlobalAssets.GetSound(soundName, false);
			KMonoBehaviour.PlaySound(soundName);
		}
	}
}
