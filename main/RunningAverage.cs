public class RunningAverage
{
	private float[] samples;

	private float min;

	private float max;

	private bool ignoreZero;

	private int validValues;

	public float AverageValue => GetAverage();

	public RunningAverage(float minValue = float.MinValue, float maxValue = float.MaxValue, int sampleCount = 15, bool allowZero = true)
	{
		min = minValue;
		max = maxValue;
		ignoreZero = !allowZero;
		samples = new float[sampleCount];
	}

	public void AddSample(float value)
	{
		if (!(value < min) && !(value > max) && (!ignoreZero || value != 0f))
		{
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

	private float GetAverage()
	{
		float num = 0f;
		for (int num2 = samples.Length - 1; num2 > samples.Length - 1 - validValues; num2--)
		{
			num += samples[num2];
		}
		return num / (float)validValues;
	}
}
