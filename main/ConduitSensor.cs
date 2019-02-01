public abstract class ConduitSensor : Switch
{
	public ConduitType conduitType;

	protected bool wasOn = false;

	protected KBatchedAnimController animController;

	protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
	{
		"on_pre",
		"on"
	};

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
	{
		"on_pst",
		"off"
	};

	protected abstract void ConduitUpdate(float dt);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(true);
		wasOn = switchedOn;
		Conduit.GetFlowManager(conduitType).AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(conduitType).RemoveConduitUpdater(ConduitUpdate);
		base.OnCleanUp();
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	protected virtual void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			if (switchedOn)
			{
				animController.Play(ON_ANIMS, KAnim.PlayMode.Loop);
			}
			else
			{
				animController.Play(OFF_ANIMS, KAnim.PlayMode.Once);
			}
		}
	}
}
