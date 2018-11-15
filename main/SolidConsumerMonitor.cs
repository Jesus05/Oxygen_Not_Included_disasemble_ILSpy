using Klei.AI;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SolidConsumerMonitor : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>
{
	public class Def : BaseDef
	{
		public Diet diet;
	}

	private struct EdibleIterator : GameScenePartitioner.Iterator
	{
		private Navigator navigator;

		private Diet diet;

		private GameObject result;

		private int resultCost;

		public EdibleIterator(Navigator navigator, Diet diet)
		{
			this.navigator = navigator;
			this.diet = diet;
			result = null;
			resultCost = -1;
		}

		public void Iterate(object target_obj)
		{
			KMonoBehaviour kMonoBehaviour = target_obj as KMonoBehaviour;
			if (!((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null))
			{
				KPrefabID component = kMonoBehaviour.GetComponent<KPrefabID>();
				if (!component.HasTag(GameTags.Creatures.ReservedByCreature) && !component.HasTag(GameTags.CreatureBrain))
				{
					FindEdibleInFeeder(ref this, kMonoBehaviour);
					GameObject gameObject = kMonoBehaviour.gameObject;
					Diet.Info dietInfo = diet.GetDietInfo(component.PrefabTag);
					if (dietInfo != null)
					{
						if (component.HasTag(GameTags.Plant))
						{
							AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(component);
							if (amountInstance != null)
							{
								float num = 0.25f;
								if (amountInstance.value / amountInstance.GetMax() < num)
								{
									return;
								}
							}
						}
						int cell = Grid.PosToCell(gameObject.transform.GetPosition());
						int navigationCost = navigator.GetNavigationCost(cell);
						if (navigationCost != -1 && (navigationCost < resultCost || resultCost == -1))
						{
							resultCost = navigationCost;
							result = gameObject;
						}
					}
				}
			}
		}

		public void Cleanup()
		{
		}

		private void FindEdibleInFeeder(ref EdibleIterator edible_iterator, KMonoBehaviour target)
		{
			if (target.HasTag(RoomConstraints.ConstraintTags.CreatureFeeder))
			{
				ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList = ListPool<Storage, SolidConsumerMonitor>.Allocate();
				target.GetComponents(pooledList);
				foreach (Storage item in pooledList)
				{
					if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
					{
						foreach (GameObject item2 in item.items)
						{
							if (!((UnityEngine.Object)item2 == (UnityEngine.Object)null))
							{
								edible_iterator.Iterate(item2.GetComponent<KMonoBehaviour>());
							}
						}
					}
				}
				pooledList.Recycle();
			}
		}

		public GameObject GetResult()
		{
			return result;
		}
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

	private static void FindFood(Instance smi, float dt)
	{
		int x = 0;
		int y = 0;
		int cell = Grid.PosToCell(smi.gameObject.transform.GetPosition());
		Grid.CellToXY(cell, out x, out y);
		int radius = 8;
		EdibleIterator iterator = new EdibleIterator(smi.GetComponent<Navigator>(), smi.def.diet);
		foreach (CreatureFeeder item in Components.CreatureFeeders.Items)
		{
			iterator.Iterate(item);
		}
		if ((UnityEngine.Object)iterator.GetResult() == (UnityEngine.Object)null)
		{
			GameScenePartitioner.Instance.Iterate(cell, radius, GameScenePartitioner.Instance.pickupablesLayer, ref iterator);
			GameScenePartitioner.Instance.Iterate(cell, radius, GameScenePartitioner.Instance.plants, ref iterator);
		}
		iterator.Cleanup();
		smi.targetEdible = iterator.GetResult();
	}
}
