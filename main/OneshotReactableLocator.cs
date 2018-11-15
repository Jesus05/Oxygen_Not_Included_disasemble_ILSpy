using UnityEngine;

public class OneshotReactableLocator : IEntityConfig
{
	public static readonly string ID = "OneshotReactableLocator";

	public static EmoteReactable CreateOneshotReactable(GameObject source, float lifetime, string id, ChoreType chore_type, HashedString animset, int range_width = 15, int range_height = 15, float min_reactor_time = 20f)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ID), source.transform.GetPosition());
		EmoteReactable emoteReactable = new EmoteReactable(gameObject, id, chore_type, animset, range_width, range_height, 100000f, min_reactor_time, float.PositiveInfinity);
		emoteReactable.AddPrecondition(ReactorIsNotSource(source));
		OneshotReactableHost component = gameObject.GetComponent<OneshotReactableHost>();
		component.lifetime = lifetime;
		component.SetReactable(emoteReactable);
		gameObject.SetActive(true);
		return emoteReactable;
	}

	private static Reactable.ReactablePrecondition ReactorIsNotSource(GameObject source)
	{
		return (GameObject reactor, Navigator.ActiveTransition transition) => (Object)reactor != (Object)source;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, false);
		gameObject.AddTag(GameTags.NotConversationTopic);
		gameObject.AddOrGet<OneshotReactableHost>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
