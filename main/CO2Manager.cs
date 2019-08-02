using System.Collections.Generic;
using UnityEngine;

public class CO2Manager : KMonoBehaviour, ISim33ms
{
	private const float CO2Lifetime = 3f;

	[SerializeField]
	private Vector3 acceleration;

	[SerializeField]
	private CO2 prefab;

	[SerializeField]
	private GameObject breathPrefab;

	[SerializeField]
	private Color tintColour;

	private List<CO2> co2Items = new List<CO2>();

	private ObjectPool breathPool;

	private ObjectPool co2Pool;

	public static CO2Manager instance;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		prefab.gameObject.SetActive(false);
		breathPrefab.SetActive(false);
		co2Pool = new ObjectPool(InstantiateCO2, 16);
		breathPool = new ObjectPool(InstantiateBreath, 16);
	}

	private GameObject InstantiateCO2()
	{
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private GameObject InstantiateBreath()
	{
		GameObject gameObject = GameUtil.KInstantiate(breathPrefab, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	public void Sim33ms(float dt)
	{
		Vector2I xy = default(Vector2I);
		Vector2I xy2 = default(Vector2I);
		Vector3 vector = acceleration * dt;
		int num = co2Items.Count;
		for (int i = 0; i < num; i++)
		{
			CO2 cO = co2Items[i];
			cO.velocity += vector;
			cO.lifetimeRemaining -= dt;
			Grid.PosToXY(cO.transform.GetPosition(), out xy);
			cO.transform.SetPosition(cO.transform.GetPosition() + cO.velocity * dt);
			Grid.PosToXY(cO.transform.GetPosition(), out xy2);
			int num2 = Grid.XYToCell(xy.x, xy.y);
			int num3 = num2;
			for (int num4 = xy.y; num4 >= xy2.y; num4--)
			{
				int num5 = Grid.XYToCell(xy.x, num4);
				bool flag = !Grid.IsValidCell(num5) || cO.lifetimeRemaining <= 0f;
				if (!flag)
				{
					Element element = Grid.Element[num5];
					flag = (element.IsLiquid || element.IsSolid);
				}
				if (flag)
				{
					bool flag2 = false;
					int num6;
					if (num3 != num5)
					{
						num6 = num3;
						flag2 = true;
					}
					else
					{
						num6 = num5;
						while (Grid.IsValidCell(num6))
						{
							Element element2 = Grid.Element[num6];
							if (!element2.IsLiquid && !element2.IsSolid)
							{
								flag2 = true;
								break;
							}
							num6 = Grid.CellAbove(num6);
						}
					}
					cO.TriggerDestroy();
					if (flag2)
					{
						SimMessages.ModifyMass(num6, cO.mass, byte.MaxValue, 0, CellEventLogger.Instance.CO2ManagerFixedUpdate, cO.temperature, SimHashes.CarbonDioxide);
						num--;
						co2Items[i] = co2Items[num];
						co2Items.RemoveAt(num);
					}
					else
					{
						DebugUtil.LogWarningArgs("Couldn't emit CO2");
					}
					break;
				}
				num3 = num5;
			}
		}
	}

	public void SpawnCO2(Vector3 position, float mass, float temperature)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		GameObject gameObject = co2Pool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		CO2 component = gameObject.GetComponent<CO2>();
		component.mass = mass;
		component.temperature = temperature;
		component.velocity = Vector3.zero;
		component.lifetimeRemaining = 3f;
		KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
		component2.TintColour = tintColour;
		component2.onDestroySelf = OnDestroyCO2;
		component.StartLoop();
		co2Items.Add(component);
	}

	public void SpawnBreath(Vector3 position, float mass, float temperature)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		SpawnCO2(position, mass, temperature);
		GameObject gameObject = breathPool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.TintColour = tintColour;
		component.onDestroySelf = OnDestroyBreath;
		component.Play("breath", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void OnDestroyCO2(GameObject co2_go)
	{
		co2_go.SetActive(false);
		co2Pool.ReleaseInstance(co2_go);
	}

	private void OnDestroyBreath(GameObject breath_go)
	{
		breath_go.SetActive(false);
		breathPool.ReleaseInstance(breath_go);
	}
}
