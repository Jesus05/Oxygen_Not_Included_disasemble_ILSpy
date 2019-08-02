using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ElementEmitter : SimComponent
{
	[SerializeField]
	public ElementConverter.OutputElement outputElement;

	[SerializeField]
	public float emissionFrequency = 1f;

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public float maxPressure = 1f;

	private Guid statusHandle = Guid.Empty;

	public bool showDescriptor = true;

	private HandleVector<Game.CallbackInfo>.Handle onBlockedHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;

	private HandleVector<Game.CallbackInfo>.Handle onUnblockedHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;

	[CompilerGenerated]
	private static Action<int> _003C_003Ef__mg_0024cache0;

	public bool isEmitterBlocked
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		onBlockedHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(OnEmitterBlocked, true));
		onUnblockedHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(OnEmitterUnblocked, true));
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.ManualReleaseHandle(onBlockedHandle);
		Game.Instance.ManualReleaseHandle(onUnblockedHandle);
		base.OnCleanUp();
	}

	public void SetEmitting(bool emitting)
	{
		SetSimActive(emitting);
	}

	protected override void OnSimActivate()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		int game_cell = Grid.OffsetCell(cell, (int)outputElement.outputElementOffset.x, (int)outputElement.outputElementOffset.y);
		if (outputElement.elementHash != 0 && outputElement.massGenerationRate > 0f && emissionFrequency > 0f)
		{
			float emit_temperature = (outputElement.minOutputTemperature != 0f) ? outputElement.minOutputTemperature : GetComponent<PrimaryElement>().Temperature;
			SimMessages.ModifyElementEmitter(simHandle, game_cell, emitRange, outputElement.elementHash, emissionFrequency, outputElement.massGenerationRate, emit_temperature, maxPressure, outputElement.addedDiseaseIdx, outputElement.addedDiseaseCount);
		}
		if (showDescriptor)
		{
			statusHandle = GetComponent<KSelectable>().ReplaceStatusItem(statusHandle, Db.Get().BuildingStatusItems.ElementEmitterOutput, this);
		}
	}

	protected override void OnSimDeactivate()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		int game_cell = Grid.OffsetCell(cell, (int)outputElement.outputElementOffset.x, (int)outputElement.outputElementOffset.y);
		SimMessages.ModifyElementEmitter(simHandle, game_cell, emitRange, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
		if (showDescriptor)
		{
			statusHandle = GetComponent<KSelectable>().RemoveStatusItem(statusHandle, false);
		}
	}

	public void ForceEmit(float mass, byte disease_idx, int disease_count, float temperature = -1f)
	{
		if (!(mass <= 0f))
		{
			float temperature2 = (!(temperature > 0f)) ? outputElement.minOutputTemperature : temperature;
			Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
			if (element.IsGas || element.IsLiquid)
			{
				int gameCell = Grid.PosToCell(base.transform.GetPosition());
				SimMessages.AddRemoveSubstance(gameCell, outputElement.elementHash, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature2, disease_idx, disease_count, true, -1);
			}
			else if (element.IsSolid)
			{
				element.substance.SpawnResource(base.transform.GetPosition() + new Vector3(0f, 0.5f, 0f), mass, temperature2, disease_idx, disease_count, false, true, false);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(outputElement.elementHash).name, base.gameObject.transform, 1.5f, false);
		}
	}

	private void OnEmitterBlocked()
	{
		isEmitterBlocked = true;
		Trigger(1615168894, this);
	}

	private void OnEmitterUnblocked()
	{
		isEmitterBlocked = false;
		Trigger(-657992955, this);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		Game.Instance.simComponentCallbackManager.GetItem(cb_handle);
		SimMessages.AddElementEmitter(maxPressure, cb_handle.index, onBlockedHandle.index, onUnblockedHandle.index);
	}

	protected override void OnSimUnregister()
	{
		StaticUnregister(simHandle);
	}

	private static void StaticUnregister(int sim_handle)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		SimMessages.RemoveElementEmitter(-1, sim_handle);
	}

	private void OnDrawGizmosSelected()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		int cell2 = Grid.OffsetCell(cell, (int)outputElement.outputElementOffset.x, (int)outputElement.outputElementOffset.y);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(Grid.CellToPos(cell2) + Vector3.right / 2f + Vector3.up / 2f, 0.2f);
	}

	protected override Action<int> GetStaticUnregister()
	{
		return StaticUnregister;
	}
}
