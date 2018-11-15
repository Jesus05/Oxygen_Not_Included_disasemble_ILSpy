using System;
using UnityEngine;

namespace Klei.AI
{
	public class ModifierInstance<ModifierType> : IStateMachineTarget
	{
		public ModifierType modifier;

		public GameObject gameObject
		{
			get;
			private set;
		}

		public Transform transform => gameObject.transform;

		public bool isNull => (UnityEngine.Object)gameObject == (UnityEngine.Object)null;

		public string name => gameObject.name;

		public ModifierInstance(GameObject game_object, ModifierType modifier)
		{
			gameObject = game_object;
			this.modifier = modifier;
		}

		public ComponentType GetComponent<ComponentType>()
		{
			return this.gameObject.GetComponent<ComponentType>();
		}

		public int Subscribe(int hash, Action<object> handler)
		{
			return gameObject.GetComponent<KMonoBehaviour>().Subscribe(hash, handler);
		}

		public void Unsubscribe(int hash, Action<object> handler)
		{
			gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(hash, handler);
		}

		public void Unsubscribe(int id)
		{
			gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(id);
		}

		public void Trigger(int hash, object data = null)
		{
			gameObject.GetComponent<KPrefabID>().Trigger(hash, data);
		}

		public virtual void OnCleanUp()
		{
		}
	}
}
