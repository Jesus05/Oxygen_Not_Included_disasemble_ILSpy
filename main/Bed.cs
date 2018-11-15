using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Workable, IEffectDescriptor
{
	[MyCmpReq]
	private Sleepable sleepable;

	private Worker targetWorker;

	public string[] effects;

	private static Dictionary<string, string> roomSleepingEffects = new Dictionary<string, string>
	{
		{
			"Barracks",
			"BarracksStamina"
		},
		{
			"Bedroom",
			"BedroomStamina"
		}
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		sleepable = GetComponent<Sleepable>();
		Sleepable obj = sleepable;
		obj.OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(obj.OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
	}

	private void OnWorkableEvent(WorkableEvent workable_event)
	{
		switch (workable_event)
		{
		case WorkableEvent.WorkStarted:
			AddEffects();
			break;
		case WorkableEvent.WorkStopped:
			RemoveEffects();
			break;
		}
	}

	private void AddEffects()
	{
		targetWorker = sleepable.worker;
		if (effects != null)
		{
			string[] array = effects;
			foreach (string effect_id in array)
			{
				targetWorker.GetComponent<Effects>().Add(effect_id, false);
			}
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			RoomType roomType = roomOfGameObject.roomType;
			foreach (KeyValuePair<string, string> roomSleepingEffect in roomSleepingEffects)
			{
				if (roomSleepingEffect.Key == roomType.Id)
				{
					targetWorker.GetComponent<Effects>().Add(roomSleepingEffect.Value, false);
				}
			}
			roomType.TriggerRoomEffects(GetComponent<KPrefabID>(), targetWorker.GetComponent<Effects>());
		}
	}

	private void RemoveEffects()
	{
		if (!((UnityEngine.Object)targetWorker == (UnityEngine.Object)null))
		{
			if (effects != null)
			{
				string[] array = effects;
				foreach (string effect_id in array)
				{
					targetWorker.GetComponent<Effects>().Remove(effect_id);
				}
			}
			foreach (KeyValuePair<string, string> roomSleepingEffect in roomSleepingEffects)
			{
				targetWorker.GetComponent<Effects>().Remove(roomSleepingEffect.Value);
			}
			targetWorker = null;
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (effects != null)
		{
			string[] array = effects;
			foreach (string text in array)
			{
				if (text != null && text != string.Empty)
				{
					Effect.AddModifierDescriptions(base.gameObject, list, text, false);
				}
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)sleepable != (UnityEngine.Object)null)
		{
			Sleepable obj = sleepable;
			obj.OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Remove(obj.OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
		}
	}
}
