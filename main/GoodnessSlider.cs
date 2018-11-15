using UnityEngine;
using UnityEngine.UI;

public class GoodnessSlider : KMonoBehaviour
{
	public Image icon;

	public Text text;

	public Slider slider;

	public Image fill;

	public Gradient gradient;

	public string[] names;

	protected override void OnSpawn()
	{
		Spawn();
		UpdateValues();
	}

	public void UpdateValues()
	{
		Text obj = text;
		Color color = gradient.Evaluate(slider.value);
		fill.color = color;
		obj.color = color;
		for (int i = 0; i < gradient.colorKeys.Length; i++)
		{
			if (gradient.colorKeys[i].time < slider.value)
			{
				text.text = names[i];
			}
			if (i == gradient.colorKeys.Length - 1 && gradient.colorKeys[i - 1].time < slider.value)
			{
				text.text = names[i];
			}
		}
	}
}
