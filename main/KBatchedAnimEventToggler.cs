using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimEventToggler : KMonoBehaviour
{
	[Serializable]
	public struct Entry
	{
		public string anim;

		public HashedString context;

		public KBatchedAnimController controller;
	}

	[SerializeField]
	public GameObject eventSource;

	[SerializeField]
	public string enableEvent;

	[SerializeField]
	public string disableEvent;

	[SerializeField]
	public List<Entry> entries;

	private AnimEventHandler animEventHandler;

	protected override void OnPrefabInit()
	{
		Vector3 position = eventSource.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		int layer = LayerMask.NameToLayer("Default");
		foreach (Entry entry in entries)
		{
			Entry current = entry;
			current.controller.transform.SetPosition(position);
			current.controller.SetLayer(layer);
			current.controller.gameObject.SetActive(false);
		}
		int hash = Hash.SDBMLower(enableEvent);
		int hash2 = Hash.SDBMLower(disableEvent);
		Subscribe(eventSource, hash, Enable);
		Subscribe(eventSource, hash2, Disable);
	}

	protected override void OnSpawn()
	{
		animEventHandler = GetComponentInParent<AnimEventHandler>();
	}

	private void Enable(object data)
	{
		StopAll();
		HashedString context = animEventHandler.GetContext();
		if (context.IsValid)
		{
			foreach (Entry entry in entries)
			{
				Entry current = entry;
				if (current.context == context)
				{
					current.controller.gameObject.SetActive(true);
					current.controller.Play(current.anim, KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
		}
	}

	private void Disable(object data)
	{
		StopAll();
	}

	private void StopAll()
	{
		foreach (Entry entry in entries)
		{
			Entry current = entry;
			current.controller.StopAndClear();
			current.controller.gameObject.SetActive(false);
		}
	}
}
