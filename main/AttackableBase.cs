using Klei.AI;
using TUNING;
using UnityEngine;

public class AttackableBase : Workable, IApproachable
{
	private HandleVector<int>.Handle scenePartitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> SetupScenePartitionerDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		component.SetupScenePartitioner(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnCellChangedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		GameScenePartitioner.Instance.UpdatePosition(component.scenePartitionerEntry, Grid.PosToCell(component.gameObject));
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		attributeConverter = Db.Get().AttributeConverters.AttackDamage;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
		SetupScenePartitioner(null);
		Subscribe(1088554450, OnCellChangedDelegate);
		Subscribe(-1506500077, OnDefeatedDelegate);
		Subscribe(-1256572400, SetupScenePartitionerDelegate);
		Subscribe(1623392196, OnDefeatedDelegate);
	}

	public float GetDamageMultiplier()
	{
		if (attributeConverter != null && (Object)base.worker != (Object)null)
		{
			AttributeConverterInstance converter = base.worker.GetComponent<AttributeConverters>().GetConverter(attributeConverter.Id);
			return Mathf.Max(1f + converter.Evaluate(), 0.1f);
		}
		return 1f;
	}

	private void SetupScenePartitioner(object data = null)
	{
		Vector2I vector2I = Grid.PosToXY(base.transform.GetPosition());
		int x = vector2I.x;
		Vector2I vector2I2 = Grid.PosToXY(base.transform.GetPosition());
		Extents extents = new Extents(x, vector2I2.y, 1, 1);
		scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, GetComponent<FactionAlignment>(), extents, GameScenePartitioner.Instance.attackableEntitiesLayer, null);
	}

	private void OnDefeated(object data = null)
	{
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
	}

	public override float GetEfficiencyMultiplier(Worker worker)
	{
		return 1f;
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(-1506500077, OnDefeatedDelegate, false);
		Unsubscribe(1623392196, OnDefeatedDelegate, false);
		Unsubscribe(-1256572400, SetupScenePartitionerDelegate, false);
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		base.OnCleanUp();
	}
}
