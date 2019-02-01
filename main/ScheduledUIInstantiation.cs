using System;
using UnityEngine;

public class ScheduledUIInstantiation : KMonoBehaviour
{
	[Serializable]
	public struct Instantiation
	{
		public string Name;

		public string Comment;

		public GameObject[] prefabs;

		public Transform parent;
	}

	public Instantiation[] UIElements;

	public bool InstantiateOnAwake = false;

	public GameHashes InstantiationEvent = GameHashes.StartGameUser;

	private bool completed;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (InstantiateOnAwake)
		{
			InstantiateElements(null);
		}
		else
		{
			Game.Instance.Subscribe((int)InstantiationEvent, InstantiateElements);
		}
	}

	public void InstantiateElements(object data)
	{
		if (!completed)
		{
			completed = true;
			Instantiation[] uIElements = UIElements;
			for (int i = 0; i < uIElements.Length; i++)
			{
				Instantiation instantiation = uIElements[i];
				GameObject[] prefabs = instantiation.prefabs;
				foreach (GameObject gameObject in prefabs)
				{
					Vector3 v = gameObject.rectTransform().anchoredPosition;
					GameObject go = Util.KInstantiateUI(gameObject, instantiation.parent.gameObject, false);
					go.rectTransform().anchoredPosition = v;
					go.rectTransform().localScale = Vector3.one;
				}
			}
			if (!InstantiateOnAwake)
			{
				Unsubscribe((int)InstantiationEvent, InstantiateElements);
			}
		}
	}
}
