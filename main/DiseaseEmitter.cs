using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DiseaseEmitter : SimComponent
{
	[SerializeField]
	public HashedString diseaseID;

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public float emitInterval = 1f;

	[SerializeField]
	public int emitCount = 1;

	private byte diseaseIdx;

	[CompilerGenerated]
	private static Action<int> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		diseaseIdx = Db.Get().Diseases.GetIndex(diseaseID);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		SimMessages.AddDiseaseEmitter(cb_handle.index);
	}

	protected override void OnSimUnregister()
	{
		StaticUnregister(simHandle);
	}

	private static void StaticUnregister(int sim_handle)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SimMessages.RemoveDiseaseEmitter(-1, sim_handle);
		}
	}

	protected override Action<int> GetStaticUnregister()
	{
		return StaticUnregister;
	}

	public void SetEmitting(bool emitting)
	{
		SetSimActive(emitting);
	}

	protected override void OnSimActivate()
	{
		if (diseaseIdx != 255 && emitCount > 0 && emitInterval > 0f)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			SimMessages.ModifyDiseaseEmitter(simHandle, cell, emitRange, diseaseIdx, emitInterval, emitCount);
		}
	}

	protected override void OnSimDeactivate()
	{
		SimMessages.ModifyDiseaseEmitter(simHandle, 0, 0, byte.MaxValue, 0f, 0);
	}
}
