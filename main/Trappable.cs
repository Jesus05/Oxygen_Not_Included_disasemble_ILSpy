using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class Trappable : KMonoBehaviour, IGameObjectEffectDescriptor
{
	private bool registered = false;

	private static readonly EventSystem.IntraObjectHandler<Trappable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Trappable>(delegate(Trappable component, object data)
	{
		component.OnStore(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Register();
		OnCellChange();
	}

	protected override void OnCleanUp()
	{
		Unregister();
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		int cell = Grid.PosToCell(this);
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.trapsLayer, this);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Register();
	}

	protected override void OnCmpDisable()
	{
		Unregister();
		base.OnCmpDisable();
	}

	private void Register()
	{
		if (!registered)
		{
			Subscribe(856640610, OnStoreDelegate);
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "Trappable.Register");
			registered = true;
		}
	}

	private void Unregister()
	{
		if (registered)
		{
			Unsubscribe(856640610, OnStoreDelegate, false);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
			registered = false;
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_TRAP, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_TRAP, Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public void OnStore(object data)
	{
		Storage storage = data as Storage;
		Trap exists = (!(bool)storage) ? null : storage.GetComponent<Trap>();
		if ((bool)exists)
		{
			base.gameObject.AddTag(GameTags.Trapped);
		}
		else
		{
			base.gameObject.RemoveTag(GameTags.Trapped);
		}
	}
}
