using TUNING;
using UnityEngine;

public class GasBottler : Workable
{
	private class Controller : GameStateMachine<Controller, Controller.Instance, GasBottler>
	{
		public new class Instance : GameInstance
		{
			public Instance(GasBottler master)
				: base(master)
			{
			}
		}

		public State empty;

		public State filling;

		public State ready;

		public State pickup;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			empty.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, filling, (Instance smi) => smi.master.storage.IsFull());
			filling.PlayAnim("working").OnAnimQueueComplete(ready);
			ready.EventTransition(GameHashes.OnStorageChange, pickup, (Instance smi) => !smi.master.storage.IsFull()).Enter(delegate(Instance smi)
			{
				smi.master.storage.allowItemRemoval = true;
				foreach (GameObject item in smi.master.storage.items)
				{
					item.Trigger(-778359855, smi.master.storage);
				}
			}).Exit(delegate(Instance smi)
			{
				smi.master.storage.allowItemRemoval = false;
				foreach (GameObject item2 in smi.master.storage.items)
				{
					item2.Trigger(-778359855, smi.master.storage);
				}
			});
			pickup.PlayAnim("pick_up").OnAnimQueueComplete(empty);
		}
	}

	public Storage storage;

	private Controller.Instance smi;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new Controller.Instance(this);
		smi.StartSM();
		UpdateStoredItemState();
	}

	protected override void OnCleanUp()
	{
		if (smi != null)
		{
			smi.StopSM("OnCleanUp");
		}
		base.OnCleanUp();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Hauler", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(MaterialsManager.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	private void UpdateStoredItemState()
	{
		storage.allowItemRemoval = (smi != null && smi.GetCurrentState() == smi.sm.ready);
		foreach (GameObject item in storage.items)
		{
			if ((Object)item != (Object)null)
			{
				item.Trigger(-778359855, storage);
			}
		}
	}
}
