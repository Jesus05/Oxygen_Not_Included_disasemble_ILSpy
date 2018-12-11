using Klei.AI;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SolidConsumerMonitor : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>
{
	public class Def : BaseDef
	{
		public Diet diet;
	}

	public new class Instance : GameInstance
	{
		public GameObject targetEdible;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void OnEatSolidComplete(object data)
		{
			KPrefabID kPrefabID = data as KPrefabID;
			if (!((UnityEngine.Object)kPrefabID == (UnityEngine.Object)null))
			{
				PrimaryElement component = kPrefabID.GetComponent<PrimaryElement>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					Diet.Info dietInfo = base.def.diet.GetDietInfo(kPrefabID.PrefabTag);
					if (dietInfo != null)
					{
						AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(base.smi.gameObject);
						string properName = kPrefabID.GetProperName();
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, properName, kPrefabID.transform, 1.5f, false);
						float calories = amountInstance.GetMax() - amountInstance.value;
						float a = dietInfo.ConvertCaloriesToConsumptionMass(calories);
						Growing component2 = kPrefabID.GetComponent<Growing>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							AmountInstance amountInstance2 = Db.Get().Amounts.Maturity.Lookup(component2.gameObject);
							float value = amountInstance2.value;
							a = Mathf.Min(a, value);
							amountInstance2.value -= a;
							kPrefabID.Trigger(-1793167409, null);
						}
						else
						{
							a = Mathf.Min(a, component.Mass);
							component.Mass -= a;
							Pickupable component3 = component.GetComponent<Pickupable>();
							if ((UnityEngine.Object)component3.storage != (UnityEngine.Object)null)
							{
								component3.storage.Trigger(-1452790913, base.gameObject);
								component3.storage.Trigger(-1697596308, base.gameObject);
							}
						}
						float calories2 = dietInfo.ConvertConsumptionMassToCalories(a);
						CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = default(CreatureCalorieMonitor.CaloriesConsumedEvent);
						caloriesConsumedEvent.tag = kPrefabID.PrefabTag;
						caloriesConsumedEvent.calories = calories2;
						CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent2 = caloriesConsumedEvent;
						Trigger(-2038961714, caloriesConsumedEvent2);
						targetEdible = null;
					}
				}
			}
		}
	}

	private State satisfied;

	private State lookingforfood;

	private static TagBits plantMask = new TagBits(GameTags.Plant);

	private static TagBits creatureMask = new TagBits(new Tag[2]
	{
		GameTags.Creatures.ReservedByCreature,
		GameTags.CreatureBrain
	});

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.EventHandler(GameHashes.EatSolidComplete, delegate(Instance smi, object data)
		{
			smi.OnEatSolidComplete(data);
		}).ToggleBehaviour(GameTags.Creatures.WantsToEat, (Instance smi) => (UnityEngine.Object)smi.targetEdible != (UnityEngine.Object)null && !smi.targetEdible.HasTag(GameTags.Creatures.ReservedByCreature), null);
		satisfied.TagTransition(GameTags.Creatures.Hungry, lookingforfood, false);
		lookingforfood.TagTransition(GameTags.Creatures.Hungry, satisfied, true).Update(FindFood, UpdateRate.SIM_1000ms, false);
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void EndDetailedSample()
	{
	}

	private static void FindFood(Instance smi, float dt)
	{
		ListPool<KMonoBehaviour, SolidConsumerMonitor>.PooledList pooledList = ListPool<KMonoBehaviour, SolidConsumerMonitor>.Allocate();
		ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
		foreach (CreatureFeeder item in Components.CreatureFeeders.Items)
		{
			item.GetComponents(pooledList2);
			foreach (Storage item2 in pooledList2)
			{
				if (!((UnityEngine.Object)item2 == (UnityEngine.Object)null))
				{
					foreach (GameObject item3 in item2.items)
					{
						pooledList.Add((!((UnityEngine.Object)item3 != (UnityEngine.Object)null)) ? null : item3.GetComponent<KMonoBehaviour>());
					}
				}
			}
		}
		pooledList2.Recycle();
		int x = 0;
		int y = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out x, out y);
		x -= 8;
		y -= 8;
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
		GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.plants, pooledList3);
		foreach (ScenePartitionerEntry item4 in pooledList3)
		{
			pooledList.Add(item4.obj as KMonoBehaviour);
		}
		pooledList3.Recycle();
		Diet diet = smi.def.diet;
		for (int i = 0; i != pooledList.Count; i++)
		{
			KMonoBehaviour kMonoBehaviour = pooledList[i];
			if (!((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null))
			{
				KPrefabID component = kMonoBehaviour.GetComponent<KPrefabID>();
				component.UpdateTagBits();
				if (component.HasAnyTags_AssumeLaundered(ref creatureMask) || diet.GetDietInfo(component.PrefabTag) == null)
				{
					pooledList[i] = null;
				}
				else if (component.HasAnyTags_AssumeLaundered(ref plantMask))
				{
					AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(component);
					if (amountInstance != null)
					{
						float num = 0.25f;
						if (amountInstance.value / amountInstance.GetMax() < num)
						{
							pooledList[i] = null;
						}
					}
				}
			}
		}
		Navigator component2 = smi.GetComponent<Navigator>();
		smi.targetEdible = null;
		int num2 = -1;
		foreach (KMonoBehaviour item5 in pooledList)
		{
			if (!((UnityEngine.Object)item5 == (UnityEngine.Object)null))
			{
				int navigationCost = component2.GetNavigationCost(Grid.PosToCell(item5.gameObject.transform.GetPosition()));
				if (navigationCost != -1 && (navigationCost < num2 || num2 == -1))
				{
					num2 = navigationCost;
					smi.targetEdible = item5.gameObject;
				}
			}
		}
		pooledList.Recycle();
	}
}
