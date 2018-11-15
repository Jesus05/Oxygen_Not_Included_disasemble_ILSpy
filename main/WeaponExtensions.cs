using UnityEngine;

public static class WeaponExtensions
{
	public static Weapon AddWeapon(this GameObject prefab, float base_damage_min, float base_damage_max, AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard, AttackProperties.TargetType targetType = AttackProperties.TargetType.Single, int maxHits = 1, float aoeRadius = 0f)
	{
		Weapon weapon = prefab.AddOrGet<Weapon>();
		weapon.Configure(base_damage_min, base_damage_max, attackType, targetType, maxHits, aoeRadius);
		return weapon;
	}
}
