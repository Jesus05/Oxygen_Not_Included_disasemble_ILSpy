using UnityEngine;

public class AlgaeHabitat : StateMachineComponent<AlgaeHabitat.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, AlgaeHabitat, object>.GameInstance
	{
		public ElementConverter converter;

		public Chore emptyChore;

		public SMInstance(AlgaeHabitat master)
			: base(master)
		{
			converter = master.GetComponent<ElementConverter>();
		}

		public bool HasEnoughMass(Tag tag)
		{
			return converter.HasEnoughMass(tag);
		}

		public bool NeedsEmptying()
		{
			return base.smi.master.pollutedWaterStorage.RemainingCapacity() <= 0f;
		}

		public void CreateEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("dupe");
			}
			AlgaeHabitatEmpty component = base.master.GetComponent<AlgaeHabitatEmpty>();
			emptyChore = new WorkChore<AlgaeHabitatEmpty>(Db.Get().ChoreTypes.EmptyStorage, component, null, true, OnEmptyComplete, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
		}

		public void CancelEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("Cancelled");
				emptyChore = null;
			}
		}

		private void OnEmptyComplete(Chore chore)
		{
			emptyChore = null;
			base.master.pollutedWaterStorage.DropAll(true, false, default(Vector3), true);
		}
	}

	public class States : GameStateMachine<States, SMInstance, AlgaeHabitat>
	{
		public State generatingOxygen;

		public State stoppedGeneratingOxygen;

		public State stoppedGeneratingOxygenTransition;

		public State noWater;

		public State noAlgae;

		public State needsEmptying;

		public State gotAlgae;

		public State gotWater;

		public State gotEmptied;

		public State lostAlgae;

		public State notoperational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = noAlgae;
			root.EventTransition(GameHashes.OperationalChanged, notoperational, (SMInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, noAlgae, (SMInstance smi) => smi.master.operational.IsOperational);
			notoperational.QueueAnim("off", false, null);
			gotAlgae.PlayAnim("on_pre").OnAnimQueueComplete(noWater);
			gotEmptied.PlayAnim("on_pre").OnAnimQueueComplete(generatingOxygen);
			lostAlgae.PlayAnim("on_pst").OnAnimQueueComplete(noAlgae);
			noAlgae.QueueAnim("off", false, null).EventTransition(GameHashes.OnStorageChange, gotAlgae, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae)).Enter(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			});
			noWater.QueueAnim("on", false, null).Enter(delegate(SMInstance smi)
			{
				smi.master.GetComponent<PassiveElementConsumer>().EnableConsumption(true);
			}).EventTransition(GameHashes.OnStorageChange, lostAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae))
				.EventTransition(GameHashes.OnStorageChange, gotWater, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water));
			needsEmptying.QueueAnim("off", false, null).Enter(delegate(SMInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(SMInstance smi)
			{
				smi.CancelEmptyChore();
			})
				.ToggleStatusItem(Db.Get().BuildingStatusItems.HabitatNeedsEmptying, (object)null)
				.EventTransition(GameHashes.OnStorageChange, noAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae) || !smi.HasEnoughMass(GameTags.Water))
				.EventTransition(GameHashes.OnStorageChange, gotEmptied, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water) && !smi.NeedsEmptying());
			gotWater.PlayAnim("working_pre").OnAnimQueueComplete(needsEmptying);
			generatingOxygen.Enter(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Update("GeneratingOxygen", delegate(SMInstance smi, float dt)
			{
				int num = Grid.PosToCell(smi.master.transform.GetPosition());
				smi.converter.OutputMultiplier = ((Grid.LightCount[num] <= 0) ? 1f : smi.master.lightBonusMultiplier);
			}, UpdateRate.SIM_200ms, false)
				.QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.OnStorageChange, stoppedGeneratingOxygen, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Water) || !smi.HasEnoughMass(GameTags.Algae) || smi.NeedsEmptying());
			stoppedGeneratingOxygen.PlayAnim("working_pst").OnAnimQueueComplete(stoppedGeneratingOxygenTransition);
			stoppedGeneratingOxygenTransition.EventTransition(GameHashes.OnStorageChange, needsEmptying, (SMInstance smi) => smi.NeedsEmptying()).EventTransition(GameHashes.OnStorageChange, noWater, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Water)).EventTransition(GameHashes.OnStorageChange, lostAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae))
				.EventTransition(GameHashes.OnStorageChange, gotWater, (SMInstance smi) => smi.HasEnoughMass(GameTags.Water) && smi.HasEnoughMass(GameTags.Algae));
		}
	}

	[MyCmpGet]
	private Operational operational;

	private Storage pollutedWaterStorage;

	[SerializeField]
	public float lightBonusMultiplier = 1.1f;

	public CellOffset pressureSampleOffset = CellOffset.none;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater, true);
		}, null, null);
		ConfigurePollutedWaterOutput();
		Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private void ConfigurePollutedWaterOutput()
	{
		Storage storage = null;
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		Storage[] components = GetComponents<Storage>();
		foreach (Storage storage2 in components)
		{
			if (storage2.storageFilters.Contains(tag))
			{
				storage = storage2;
				break;
			}
		}
		ElementConverter[] components2 = GetComponents<ElementConverter>();
		foreach (ElementConverter elementConverter in components2)
		{
			ElementConverter.OutputElement[] outputElements = elementConverter.outputElements;
			for (int k = 0; k < outputElements.Length; k++)
			{
				ElementConverter.OutputElement outputElement = outputElements[k];
				if (outputElement.elementHash == SimHashes.DirtyWater)
				{
					elementConverter.SetStorage(storage);
					break;
				}
			}
		}
		pollutedWaterStorage = storage;
	}
}
