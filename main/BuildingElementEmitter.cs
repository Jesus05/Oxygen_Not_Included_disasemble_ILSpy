using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingElementEmitter : KMonoBehaviour, IEffectDescriptor, IElementEmitter, ISim200ms
{
	[SerializeField]
	public float emitRate = 0.3f;

	[SerializeField]
	[Serialize]
	public float temperature = 293f;

	[SerializeField]
	[HashedEnum]
	public SimHashes element = SimHashes.Oxygen;

	[SerializeField]
	public Vector2 modifierOffset;

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public byte emitDiseaseIdx = byte.MaxValue;

	[SerializeField]
	public int emitDiseaseCount;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private int simHandle = -1;

	private bool simActive;

	private bool dirty = true;

	private Guid statusHandle;

	private static readonly EventSystem.IntraObjectHandler<BuildingElementEmitter> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<BuildingElementEmitter>(delegate(BuildingElementEmitter component, object data)
	{
		component.OnActiveChanged(data);
	});

	[CompilerGenerated]
	private static Action<int, object> _003C_003Ef__mg_0024cache0;

	public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(accumulator);

	public float EmitRate => emitRate;

	public SimHashes Element => element;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		accumulator = Game.Instance.accumulators.Add("Element", this);
		Subscribe(824508782, OnActiveChangedDelegate);
		SimRegister();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(accumulator);
		SimUnregister();
		base.OnCleanUp();
	}

	private void OnActiveChanged(object data)
	{
		simActive = ((Operational)data).IsActive;
		dirty = true;
	}

	public void Sim200ms(float dt)
	{
		UnsafeUpdate(dt);
	}

	private unsafe void UnsafeUpdate(float dt)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			UpdateSimState();
			int handleIndex = Sim.GetHandleIndex(simHandle);
			Sim.EmittedMassInfo emittedMassInfo = Game.Instance.simData.emittedMassEntries[handleIndex];
			if (emittedMassInfo.mass > 0f)
			{
				Game.Instance.accumulators.Accumulate(accumulator, emittedMassInfo.mass);
				if (element == SimHashes.Oxygen)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, emittedMassInfo.mass, base.gameObject.GetProperName(), null);
				}
			}
		}
	}

	private void UpdateSimState()
	{
		if (dirty)
		{
			dirty = false;
			if (simActive)
			{
				if (element != 0 && emitRate > 0f)
				{
					Vector3 position = base.transform.GetPosition();
					float x = position.x + modifierOffset.x;
					Vector3 position2 = base.transform.GetPosition();
					Vector3 pos = new Vector3(x, position2.y + modifierOffset.y, 0f);
					int game_cell = Grid.PosToCell(pos);
					SimMessages.ModifyElementEmitter(simHandle, game_cell, emitRange, element, 0.2f, emitRate * 0.2f, temperature, 3.40282347E+38f, emitDiseaseIdx, emitDiseaseCount);
				}
				statusHandle = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
			}
			else
			{
				SimMessages.ModifyElementEmitter(simHandle, 0, 0, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
				statusHandle = GetComponent<KSelectable>().RemoveStatusItem(statusHandle, this);
			}
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && simHandle == -1)
		{
			simHandle = -2;
			SimMessages.AddElementEmitter(3.40282347E+38f, Game.Instance.simComponentCallbackManager.Add(OnSimRegisteredCallback, this, "BuildingElementEmitter").index, -1, -1);
		}
	}

	private void SimUnregister()
	{
		if (simHandle != -1)
		{
			if (Sim.IsValidHandle(simHandle))
			{
				SimMessages.RemoveElementEmitter(-1, simHandle);
			}
			simHandle = -1;
		}
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((BuildingElementEmitter)data).OnSimRegistered(handle);
	}

	private void OnSimRegistered(int handle)
	{
		if ((UnityEngine.Object)this != (UnityEngine.Object)null)
		{
			simHandle = handle;
		}
		else
		{
			SimMessages.RemoveElementEmitter(-1, handle);
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.element);
		string arg = element.tag.ProperName();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, arg, GameUtil.GetFormattedMass(EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, arg, GameUtil.GetFormattedMass(EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
		list.Add(item);
		return list;
	}
}
