using UnityEngine;

public static class FXHelpers
{
	public static KBatchedAnimController CreateEffect(string anim_file_name, Vector3 position, Transform parent = null, bool update_looping_sounds_position = false, Grid.SceneLayer layer = Grid.SceneLayer.Front, bool set_inactive = false)
	{
		KBatchedAnimController component = GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.EffectTemplateId), position, layer, null, 0).GetComponent<KBatchedAnimController>();
		KPrefabID component2 = component.GetComponent<KPrefabID>();
		component2.PrefabTag = TagManager.Create(anim_file_name);
		component.name = anim_file_name;
		if ((Object)parent != (Object)null)
		{
			component.transform.SetParent(parent, false);
		}
		component.transform.SetPosition(position);
		if (update_looping_sounds_position)
		{
			LoopingSounds loopingSounds = component.FindOrAddComponent<LoopingSounds>();
			loopingSounds.updatePosition = true;
		}
		KAnimFile anim = Assets.GetAnim(anim_file_name);
		if ((Object)anim == (Object)null)
		{
			Debug.LogWarning("Missing effect anim: " + anim_file_name);
		}
		else
		{
			component.AnimFiles = new KAnimFile[1]
			{
				anim
			};
		}
		if (!set_inactive)
		{
			component.gameObject.SetActive(true);
		}
		return component;
	}
}
