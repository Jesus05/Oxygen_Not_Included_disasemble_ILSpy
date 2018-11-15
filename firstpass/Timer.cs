using UnityEngine;

public class Timer
{
	private float startTime;

	private bool isStarted;

	public void Start()
	{
		if (!isStarted)
		{
			startTime = Time.time;
			isStarted = true;
		}
	}

	public void Stop()
	{
		isStarted = false;
	}

	public float GetElapsed()
	{
		return Time.time - startTime;
	}

	public bool TryStop(float elapsed_time)
	{
		if (isStarted && GetElapsed() >= elapsed_time)
		{
			Stop();
			return true;
		}
		return false;
	}
}
