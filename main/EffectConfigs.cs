using System.Collections.Generic;
using UnityEngine;

public class EffectConfigs : IMultiEntityConfig
{
	public static string EffectTemplateId = "EffectTemplateFx";

	public static string AttackSplashId = "AttackSplashFx";

	public static string OreAbsorbId = "OreAbsorbFx";

	public static string PlantDeathId = "PlantDeathFx";

	public static string BuildSplashId = "BuildSplashFx";

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		var array = new[]
		{
			new
			{
				id = EffectTemplateId,
				animFiles = new string[0],
				initialAnim = "",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = false
			},
			new
			{
				id = AttackSplashId,
				animFiles = new string[1]
				{
					"attack_beam_contact_fx_kanim"
				},
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			},
			new
			{
				id = OreAbsorbId,
				animFiles = new string[1]
				{
					"ore_collision_kanim"
				},
				initialAnim = "idle",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new
			{
				id = PlantDeathId,
				animFiles = new string[1]
				{
					"plant_death_fx_kanim"
				},
				initialAnim = "plant_death",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new
			{
				id = BuildSplashId,
				animFiles = new string[1]
				{
					"sparks_radial_build_kanim"
				},
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			}
		};
		var array2 = array;
		foreach (var anon in array2)
		{
			GameObject gameObject = EntityTemplates.CreateEntity(anon.id, anon.id, false);
			KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
			kBatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Simple;
			kBatchedAnimController.initialAnim = anon.initialAnim;
			kBatchedAnimController.initialMode = anon.initialMode;
			kBatchedAnimController.isMovable = true;
			kBatchedAnimController.destroyOnAnimComplete = anon.destroyOnAnimComplete;
			if (anon.animFiles.Length > 0)
			{
				KAnimFile[] array3 = new KAnimFile[anon.animFiles.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					array3[j] = Assets.GetAnim(anon.animFiles[j]);
				}
				kBatchedAnimController.AnimFiles = array3;
			}
			gameObject.AddOrGet<LoopingSounds>();
			list.Add(gameObject);
		}
		return list;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
