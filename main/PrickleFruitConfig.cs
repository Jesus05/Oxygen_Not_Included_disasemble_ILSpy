using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFruitConfig : IEntityConfig
{
	public static float SEEDS_PER_FRUIT_CHANCE = 0.05f;

	public static string ID = "PrickleFruit";

	private static readonly EventSystem.IntraObjectHandler<Edible> OnEatCompleteDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		OnEatComplete(component);
	});

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.PRICKLEFRUIT.NAME, ITEMS.FOOD.PRICKLEFRUIT.DESC, 1f, false, Assets.GetAnim("bristleberry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.PRICKLEFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.Subscribe(-10536414, OnEatCompleteDelegate);
	}

	private static void OnEatComplete(Edible edible)
	{
		if ((Object)edible != (Object)null)
		{
			int num = 0;
			float unitsConsumed = edible.unitsConsumed;
			int num2 = Mathf.FloorToInt(unitsConsumed);
			float num3 = unitsConsumed % 1f;
			if (Random.value < num3)
			{
				num2++;
			}
			for (int i = 0; i < num2; i++)
			{
				if (Random.value < SEEDS_PER_FRUIT_CHANCE)
				{
					num++;
				}
			}
			if (num > 0)
			{
				Vector3 pos = edible.transform.GetPosition() + new Vector3(0f, 0.05f, 0f);
				pos = Grid.CellToPosCCC(Grid.PosToCell(pos), Grid.SceneLayer.Ore);
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("PrickleFlowerSeed")), pos, Grid.SceneLayer.Ore, null, 0);
				PrimaryElement component = edible.GetComponent<PrimaryElement>();
				PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
				component2.Temperature = component.Temperature;
				component2.Units = (float)num;
				gameObject.SetActive(true);
			}
		}
	}
}
