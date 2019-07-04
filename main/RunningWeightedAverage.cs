public class RunningWeightedAverage
{
	private float[] samples;

	private float min;

	private float max;

	private bool ignoreZero = false;

	private int validValues = 0;

	public float GetWeightedAverage => WeightedAverage();

	public float GetUnweightedAverage => WeightedAverage();

	public RunningWeightedAverage(float minValue = float.MinValue, float maxValue = float.MaxValue, int sampleCount = 15, bool allowZero = true)
	{
		min = minValue;
		max = maxValue;
		ignoreZero = !allowZero;
		samples = new float[sampleCount];
	}

	public void AddSample(float value)
	{
		if (!ignoreZero || value != 0f)
		{
			if (value > max)
			{
				value = max;
			}
			if (value < min)
			{
				value = min;
			}
			if (validValues < samples.Length)
			{
				validValues++;
			}
			for (int i = 0; i < samples.Length - 1; i++)
			{
				samples[i] = samples[i + 1];
			}
			samples[samples.Length - 1] = value;
		}
	}

	private float WeightedAverage()
	{
		float num = 0f;
		float num2 = 0f;
		for (int num3 = samples.Length - 1; num3 > samples.Length - 1 - validValues; num3--)
		{
			float num4 = (float)(num3 + 1) / ((float)validValues + 1f);
			num += samples[num3] * num4;
			num2 += num4;
		}
		num /= num2;
		if (!float.IsNaN(num))
		{
			return num;
		}
		return 0f;
	}

	private float UnweightedAverage()
	{
		float num = 0f;
		for (int num2 = samples.Length - 1; num2 > samples.Length - 1 - validValues; num2--)
		{
			num += samples[num2];
		}
		num /= (float)samples.Length;
		if (!float.IsNaN(num))
		{
			return num;
		}
		return 0f;
	}
}
