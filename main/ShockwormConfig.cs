using STRINGS;
using TUNING;
using UnityEngine;

public class ShockwormConfig : IEntityConfig
{
	public const string ID = "ShockWorm";

	public GameObject CreatePrefab()
	{
		string id = "ShockWorm";
		string name = STRINGS.CREATURES.SPECIES.SHOCKWORM.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SHOCKWORM.DESC;
		float mass = 50f;
		KAnimFile anim = Assets.GetAnim("shockworm_kanim");
		string initialAnim = "idle";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		FactionManager.FactionID faction = FactionManager.FactionID.Hostile;
		initialAnim = null;
		desc = "FlyerNavGrid1x2";
		NavType navType = NavType.Hover;
		mass = 2f;
		name = "Meat";
		int onDeathDropCount = 3;
		float fREEZING_ = TUNING.CREATURES.TEMPERATURE.FREEZING_1;
		float hOT_ = TUNING.CREATURES.TEMPERATURE.HOT_1;
		EntityTemplates.ExtendEntityToBasicCreature(template, faction, initialAnim, desc, navType, 32, mass, name, onDeathDropCount, true, true, fREEZING_, hOT_, TUNING.CREATURES.TEMPERATURE.FREEZING_2, TUNING.CREATURES.TEMPERATURE.HOT_2);
		gameObject.AddOrGet<LoopingSounds>();
		Weapon weapon = gameObject.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f);
		weapon.AddEffect("WasAttacked", 1f);
		SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "Shockworm_attack_arc", NOISE_POLLUTION.CREATURES.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
