using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDiseaseSensor : ConduitThresholdSensor, IThresholdSwitch
{
	private const float rangeMin = 0f;

	private const float rangeMax = 100000f;

	private static readonly HashedString TINT_SYMBOL = "germs";

	public override float CurrentValue
	{
		get
		{
			int cell = Grid.PosToCell(this);
			ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
			return (float)contents.diseaseCount;
		}
	}

	public float RangeMin => 0f;

	public float RangeMax => 100000f;

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	protected override void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			if (switchedOn)
			{
				animController.Play(ConduitSensor.ON_ANIMS, KAnim.PlayMode.Loop);
				int cell = Grid.PosToCell(this);
				ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
				ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
				Color32 c = Color.white;
				if (contents.diseaseIdx != 255)
				{
					Disease disease = Db.Get().Diseases[contents.diseaseIdx];
					c = disease.overlayColour;
				}
				animController.SetSymbolTint(TINT_SYMBOL, c);
			}
			else
			{
				animController.Play(ConduitSensor.OFF_ANIMS, KAnim.PlayMode.Once);
			}
		}
	}

	public float GetRangeMinInputField()
	{
		return 0f;
	}

	public float GetRangeMaxInputField()
	{
		return 100000f;
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedInt((float)(int)value, GameUtil.TimeSlice.None);
	}

	public float ProcessedSliderValue(float input)
	{
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;
	}
}
