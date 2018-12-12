using System;
using UnityEngine;

public class NonLinearSlider : KSlider
{
	[Serializable]
	public struct Range
	{
		public float width;

		public float peakValue;

		public Range(float width, float peakValue)
		{
			this.width = width;
			this.peakValue = peakValue;
		}
	}

	public Range[] ranges;

	public static Range[] GetDefaultRange(float maxValue)
	{
		return new Range[1]
		{
			new Range(100f, maxValue)
		};
	}

	protected override void Start()
	{
		base.Start();
		base.minValue = 0f;
		base.maxValue = 100f;
	}

	public void SetRanges(Range[] ranges)
	{
		this.ranges = ranges;
	}

	public float GetPercentageFromValue(float value)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < ranges.Length; i++)
		{
			if (value > num2 && value <= ranges[i].peakValue)
			{
				float t = (value - num2) / (ranges[i].peakValue - num2);
				return Mathf.Lerp(num, num + ranges[i].width, t);
			}
			num += ranges[i].width;
			num2 = ranges[i].peakValue;
		}
		return 100f;
	}

	public float GetValueForPercentage(float percentage)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < ranges.Length; i++)
		{
			if (percentage > num && num + ranges[i].width >= percentage)
			{
				float t = (percentage - num) / ranges[i].width;
				return Mathf.Lerp(num2, ranges[i].peakValue, t);
			}
			num += ranges[i].width;
			num2 = ranges[i].peakValue;
		}
		return num2;
	}

	protected override void Set(float input, bool sendCallback)
	{
		base.Set(input, sendCallback);
	}
}
