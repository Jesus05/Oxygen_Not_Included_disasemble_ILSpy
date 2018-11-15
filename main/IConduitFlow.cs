using System;

public interface IConduitFlow
{
	void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default);

	void RemoveConduitUpdater(Action<float> callback);

	bool IsConduitEmpty(int cell);
}
