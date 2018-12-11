using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicMassSensor : Switch, ISaveLoadable, IThresholdSwitch
{
	[SerializeField]
	[Serialize]
	private float threshold = 0f;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	[MyCmpGet]
	private LogicPorts logicPorts;

	private bool was_pressed = false;

	private bool was_on = false;

	public float rangeMin = 0f;

	public float rangeMax = 1f;

	[Serialize]
	private float massSolid = 0f;

	[Serialize]
	private float massPickupables = 0f;

	[Serialize]
	private float massActivators = 0f;

	private const float MIN_TOGGLE_TIME = 0.15f;

	private float toggleCooldown = 0.15f;

	private HandleVector<int>.Handle solidChangedEntry;

	private HandleVector<int>.Handle pickupablesChangedEntry;

	private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicMassSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicMassSensor>(delegate(LogicMassSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateAboveThreshold;
		}
		set
		{
			activateAboveThreshold = value;
		}
	}

	public float CurrentValue => massSolid + massPickupables + massActivators;

	public float RangeMin => rangeMin;

	public float RangeMax => rangeMax;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicMassSensor component = gameObject.GetComponent<LogicMassSensor>();
		if ((Object)component != (Object)null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateVisualState(true);
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		solidChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SolidChanged", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		pickupablesChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
		floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SwitchActivatorChanged", base.gameObject, cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, OnActivatorsChanged);
		base.OnToggle += SwitchToggled;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref solidChangedEntry);
		GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
		GameScenePartitioner.Instance.Free(ref floorSwitchActivatorChangedEntry);
		base.OnCleanUp();
	}

	private void Update()
	{
		toggleCooldown = Mathf.Max(0f, toggleCooldown - Time.deltaTime);
		if (toggleCooldown == 0f)
		{
			float currentValue = CurrentValue;
			bool flag = (!activateAboveThreshold) ? (currentValue < threshold) : (currentValue > threshold);
			if (flag != base.IsSwitchedOn)
			{
				Toggle();
				toggleCooldown = 0.15f;
			}
			UpdateVisualState(false);
		}
	}

	private void OnSolidChanged(object data)
	{
		int i = Grid.CellAbove(this.NaturalBuildingCell());
		if (Grid.Solid[i])
		{
			massSolid = Grid.Mass[i];
		}
		else
		{
			massSolid = 0f;
		}
	}

	private void OnPickupablesChanged(object data)
	{
		float num = 0f;
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
		GameScenePartitioner instance = GameScenePartitioner.Instance;
		Vector2I vector2I = Grid.CellToXY(cell);
		int x = vector2I.x;
		Vector2I vector2I2 = Grid.CellToXY(cell);
		instance.GatherEntries(x, vector2I2.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			Pickupable pickupable = pooledList[i].obj as Pickupable;
			if (!((Object)pickupable == (Object)null) && !pickupable.wasAbsorbed)
			{
				KPrefabID component = pickupable.GetComponent<KPrefabID>();
				if (!component.HasTag(GameTags.Creature) || component.HasTag(GameTags.Creatures.GroundBased) || pickupable.HasTag(GameTags.Creatures.Flopping))
				{
					num += pickupable.PrimaryElement.Mass;
				}
			}
		}
		pooledList.Recycle();
		massPickupables = num;
	}

	private void OnActivatorsChanged(object data)
	{
		float num = 0f;
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
		GameScenePartitioner instance = GameScenePartitioner.Instance;
		Vector2I vector2I = Grid.CellToXY(cell);
		int x = vector2I.x;
		Vector2I vector2I2 = Grid.CellToXY(cell);
		instance.GatherEntries(x, vector2I2.y, 1, 1, GameScenePartitioner.Instance.floorSwitchActivatorLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			FloorSwitchActivator floorSwitchActivator = pooledList[i].obj as FloorSwitchActivator;
			if (!((Object)floorSwitchActivator == (Object)null))
			{
				num += floorSwitchActivator.PrimaryElement.Mass;
			}
		}
		pooledList.Recycle();
		massActivators = num;
	}

	public float GetRangeMinInputField()
	{
		return rangeMin;
	}

	public float GetRangeMaxInputField()
	{
		return rangeMax;
	}

	public string Format(float value, bool units)
	{
		GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
		bool includeSuffix = units;
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, includeSuffix, "{0:0.#}");
	}

	public float ProcessedSliderValue(float input)
	{
		input = Mathf.Round(input);
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return GameUtil.GetCurrentMassUnit(false);
	}

	private void SwitchToggled(bool toggled_on)
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, toggled_on ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		bool flag = CurrentValue > threshold;
		if (flag != was_pressed || was_on != base.IsSwitchedOn || force)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			if (flag)
			{
				if (force)
				{
					component.Play((!base.IsSwitchedOn) ? "off_down" : "on_down", KAnim.PlayMode.Once, 1f, 0f);
				}
				else
				{
					component.Play((!base.IsSwitchedOn) ? "off_down_pre" : "on_down_pre", KAnim.PlayMode.Once, 1f, 0f);
					component.Queue((!base.IsSwitchedOn) ? "off_down" : "on_down", KAnim.PlayMode.Once, 1f, 0f);
				}
			}
			else if (force)
			{
				component.Play((!base.IsSwitchedOn) ? "off_up" : "on_up", KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				component.Play((!base.IsSwitchedOn) ? "off_up_pre" : "on_up_pre", KAnim.PlayMode.Once, 1f, 0f);
				component.Queue((!base.IsSwitchedOn) ? "off_up" : "on_up", KAnim.PlayMode.Once, 1f, 0f);
			}
			was_pressed = flag;
			was_on = base.IsSwitchedOn;
		}
	}
}
