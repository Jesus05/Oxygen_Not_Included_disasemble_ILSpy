using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Polymerizer : StateMachineComponent<Polymerizer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Polymerizer, object>.GameInstance
	{
		public StatesInstance(Polymerizer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Polymerizer>
	{
		public State off;

		public State on;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational);
			off.EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
			on.EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.converter.CanConvertAtAll());
			converting.Enter("Ready", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				smi.master.TryEmit();
			}).EventTransition(GameHashes.OnStorageChange, on, (StatesInstance smi) => !smi.master.converter.CanConvertAtAll())
				.Exit("Ready", delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
		}
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[SerializeField]
	public float emitMass = 1f;

	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public Vector3 emitOffset = Vector3.zero;

	[SerializeField]
	public SimHashes exhaustElement = SimHashes.Vacuum;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	[MyCmpGet]
	private ElementConverter converter;

	private MeterController plasticMeter;

	private MeterController oilMeter;

	private static readonly EventSystem.IntraObjectHandler<Polymerizer> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<Polymerizer>(delegate(Polymerizer component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnSpawn()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		plasticMeter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0f, 0f, 0f), (string[])null);
		oilMeter = new MeterController((KAnimControllerBase)component, "meter2_target", "meter2", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0f, 0f, 0f), (string[])null);
		component.SetSymbolVisiblity("meter_target", true);
		float positionPercent = 0f;
		PrimaryElement primaryElement = storage.FindPrimaryElement(SimHashes.Petroleum);
		if ((Object)primaryElement != (Object)null)
		{
			positionPercent = Mathf.Clamp01(primaryElement.Mass / consumer.capacityKG);
		}
		oilMeter.SetPositionPercent(positionPercent);
		base.smi.StartSM();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void TryEmit()
	{
		GameObject gameObject = storage.FindFirst(emitTag);
		if ((Object)gameObject != (Object)null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			UpdatePercentDone(component);
			TryEmit(component);
		}
	}

	private void TryEmit(PrimaryElement primary_elem)
	{
		if (primary_elem.Mass >= emitMass)
		{
			plasticMeter.SetPositionPercent(0f);
			GameObject gameObject = storage.Drop(primary_elem.gameObject, true);
			Rotatable component = GetComponent<Rotatable>();
			Vector3 vector = component.transform.GetPosition() + component.GetRotatedOffset(emitOffset);
			int i = Grid.PosToCell(vector);
			if (Grid.Solid[i])
			{
				vector += component.GetRotatedOffset(Vector3.left);
			}
			gameObject.transform.SetPosition(vector);
			PrimaryElement primaryElement = storage.FindPrimaryElement(exhaustElement);
			if ((Object)primaryElement != (Object)null)
			{
				int gameCell = Grid.PosToCell(vector);
				SimMessages.AddRemoveSubstance(gameCell, primaryElement.ElementID, null, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, true, -1);
				primaryElement.Mass = 0f;
				primaryElement.ModifyDiseaseCount(-2147483648, "Polymerizer.Exhaust");
			}
		}
	}

	private void UpdatePercentDone(PrimaryElement primary_elem)
	{
		float positionPercent = Mathf.Clamp01(primary_elem.Mass / emitMass);
		plasticMeter.SetPositionPercent(positionPercent);
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!((Object)gameObject == (Object)null))
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.ElementID == SimHashes.Petroleum)
			{
				float positionPercent = Mathf.Clamp01(component.Mass / consumer.capacityKG);
				oilMeter.SetPositionPercent(positionPercent);
			}
		}
	}
}
