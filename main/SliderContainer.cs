public class SliderContainer : KMonoBehaviour
{
	public bool isPercentValue = true;

	public KSlider slider;

	public LocText nameLabel;

	public LocText valueLabel;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		slider.onValueChanged.AddListener(UpdateSliderLabel);
	}

	public void UpdateSliderLabel(float newValue)
	{
		if (isPercentValue)
		{
			valueLabel.text = (newValue * 100f).ToString("F0") + "%";
		}
		else
		{
			valueLabel.text = newValue.ToString();
		}
	}
}
