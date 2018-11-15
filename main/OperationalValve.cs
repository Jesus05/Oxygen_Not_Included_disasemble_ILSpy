using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalValve : ValveBase
{
	[MyCmpReq]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<OperationalValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalValve>(delegate(OperationalValve component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		OnOperationalChanged(operational.IsOperational);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(-592767678, OnOperationalChangedDelegate, false);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		if ((bool)data)
		{
			base.CurrentFlow = base.MaxFlow;
		}
		else
		{
			base.CurrentFlow = 0f;
		}
	}

	public override void UpdateAnim()
	{
		float averageRate = Game.Instance.accumulators.GetAverageRate(flowAccumulator);
		if (operational.IsOperational)
		{
			if (averageRate > 0f)
			{
				controller.Play("on_flow", KAnim.PlayMode.Loop, 1f, 0f);
			}
			else
			{
				controller.Play("on", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		else if (averageRate > 0f)
		{
			controller.Play("off_flow", KAnim.PlayMode.Loop, 1f, 0f);
		}
		else
		{
			controller.Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
	}
}
