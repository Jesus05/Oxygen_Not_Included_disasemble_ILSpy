using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicElementSensor : Switch, ISaveLoadable, ISim200ms
{
	private bool wasOn = false;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private bool[] samples = new bool[8];

	private int sampleIdx = 0;

	private byte desiredElementIdx = byte.MaxValue;

	private static readonly EventSystem.IntraObjectHandler<LogicElementSensor> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<LogicElementSensor>(delegate(LogicElementSensor component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Filterable component = GetComponent<Filterable>();
		component.onFilterChanged += OnElementSelected;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(true);
		wasOn = switchedOn;
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	public void Sim200ms(float dt)
	{
		int i = Grid.PosToCell(this);
		if (sampleIdx < 8)
		{
			samples[sampleIdx] = (Grid.ElementIdx[i] == desiredElementIdx);
			sampleIdx++;
		}
		else
		{
			sampleIdx = 0;
			bool flag = true;
			bool[] array = samples;
			for (int j = 0; j < array.Length; j++)
			{
				flag = (array[j] && flag);
			}
			if (base.IsSwitchedOn != flag)
			{
				Toggle();
			}
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		bool flag = switchedOn && GetComponent<Operational>().IsOperational;
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, flag ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private void OnElementSelected(Tag element_tag)
	{
		desiredElementIdx = ElementLoader.GetElementIndex(element_tag);
	}

	private void OnOperationalChanged(object data)
	{
		UpdateLogicCircuit();
		UpdateVisualState(false);
	}
}
