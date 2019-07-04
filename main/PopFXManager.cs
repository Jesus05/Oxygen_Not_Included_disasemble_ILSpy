using Klei;
using System.Collections.Generic;
using UnityEngine;

public class PopFXManager : KScreen
{
	public static PopFXManager Instance;

	public GameObject Prefab_PopFX;

	public List<PopFX> Pool = new List<PopFX>();

	public Sprite sprite_Plus;

	public Sprite sprite_Negative;

	public Sprite sprite_Resource;

	public Sprite sprite_Building;

	public Sprite sprite_Research;

	private bool ready = false;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ready = true;
		if (!GenericGameSettings.instance.disablePopFx)
		{
			for (int i = 0; i < 20; i++)
			{
				PopFX item = CreatePopFX();
				Pool.Add(item);
			}
		}
	}

	public bool Ready()
	{
		return ready;
	}

	public PopFX SpawnFX(Sprite icon, string text, Transform target_transform, Vector3 offset, float lifetime = 1.5f, bool track_target = false, bool force_spawn = false)
	{
		if (!GenericGameSettings.instance.disablePopFx)
		{
			if (!Game.IsQuitting())
			{
				Vector3 vector = offset;
				if ((Object)target_transform != (Object)null)
				{
					vector += target_transform.GetPosition();
				}
				if (!force_spawn)
				{
					int cell = Grid.PosToCell(vector);
					if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
					{
						return null;
					}
				}
				PopFX popFX = null;
				if (Pool.Count > 0)
				{
					popFX = Pool[0];
					Pool[0].gameObject.SetActive(true);
					Pool[0].Spawn(icon, text, target_transform, offset, lifetime, track_target);
					Pool.RemoveAt(0);
				}
				else
				{
					popFX = CreatePopFX();
					popFX.gameObject.SetActive(true);
					popFX.Spawn(icon, text, target_transform, offset, lifetime, track_target);
				}
				return popFX;
			}
			return null;
		}
		return null;
	}

	public PopFX SpawnFX(Sprite icon, string text, Transform target_transform, float lifetime = 1.5f, bool track_target = false)
	{
		return SpawnFX(icon, text, target_transform, Vector3.zero, lifetime, track_target, false);
	}

	private PopFX CreatePopFX()
	{
		GameObject gameObject = Util.KInstantiate(Prefab_PopFX, base.gameObject, "Pooled_PopFX");
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<PopFX>();
	}

	public void RecycleFX(PopFX fx)
	{
		Pool.Add(fx);
	}
}
