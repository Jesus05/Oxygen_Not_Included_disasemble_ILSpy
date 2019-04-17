using System.Collections.Generic;
using UnityEngine;

public class Weapon : KMonoBehaviour
{
	[MyCmpReq]
	private FactionAlignment alignment;

	public AttackProperties properties;

	public void Configure(float base_damage_min, float base_damage_max, AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard, AttackProperties.TargetType targetType = AttackProperties.TargetType.Single, int maxHits = 1, float aoeRadius = 0f)
	{
		properties = new AttackProperties();
		properties.base_damage_min = base_damage_min;
		properties.base_damage_max = base_damage_max;
		properties.maxHits = maxHits;
		properties.damageType = attackType;
		properties.aoe_radius = aoeRadius;
		properties.attacker = this;
	}

	public void AddEffect(string effectID = "WasAttacked", float probability = 1f)
	{
		if (properties.effects == null)
		{
			properties.effects = new List<AttackEffect>();
		}
		properties.effects.Add(new AttackEffect(effectID, probability));
	}

	public void AttackArea(Vector3 centerPoint)
	{
		Vector3 a = centerPoint;
		Vector3 b = Vector3.zero;
		alignment = GetComponent<FactionAlignment>();
		if (!((Object)alignment == (Object)null))
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Health item in Components.Health.Items)
			{
				if (!((Object)item.gameObject == (Object)base.gameObject) && !item.IsDefeated())
				{
					FactionAlignment component = item.GetComponent<FactionAlignment>();
					if (!((Object)component == (Object)null) && component.IsAlignmentActive() && FactionManager.Instance.GetDisposition(alignment.Alignment, component.Alignment) == FactionManager.Disposition.Attack)
					{
						b = item.transform.GetPosition();
						b.z = a.z;
						if (Vector3.Distance(a, b) <= properties.aoe_radius)
						{
							list.Add(item.gameObject);
						}
					}
				}
			}
			AttackTargets(list.ToArray());
		}
	}

	public void AttackTarget(GameObject target)
	{
		AttackTargets(new GameObject[1]
		{
			target
		});
	}

	public void AttackTargets(GameObject[] targets)
	{
		if (properties == null)
		{
			Debug.LogWarning($"Attack properties not configured. {base.gameObject.name} cannot attack with weapon.");
		}
		else
		{
			new Attack(properties, targets);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		properties.attacker = this;
	}
}
