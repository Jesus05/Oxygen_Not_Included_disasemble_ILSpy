public interface ICPULoad
{
	float GetEstimatedFrameTime();

	bool AdjustLoad(float currentFrameTime, float frameTimeDelta);
}
