using System.Collections.Generic;
using UnityEngine;

public class IntSliderSideScreen : SideScreenContent
{
	private IIntSliderControl target;

	public List<SliderSet> sliderSets;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < sliderSets.Count; i++)
		{
			sliderSets[i].SetupSlider(i);
			sliderSets[i].valueSlider.wholeNumbers = true;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IIntSliderControl>() != null || target.GetSMI<IIntSliderControl>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			target = new_target.GetComponent<IIntSliderControl>();
			if (target == null)
			{
				target = new_target.GetSMI<IIntSliderControl>();
			}
			if (target == null)
			{
				Debug.LogError("The gameObject received does not contain a Manual Generator component", null);
			}
			else
			{
				titleKey = target.SliderTitleKey;
				for (int i = 0; i < sliderSets.Count; i++)
				{
					sliderSets[i].SetTarget(target);
				}
			}
		}
	}
}
