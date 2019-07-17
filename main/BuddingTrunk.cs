using Klei.AI;
using KSerialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingTrunk : KMonoBehaviour, ISim4000ms
{
	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private WiltCondition wilting;

	[MyCmpReq]
	private UprootedMonitor uprooted;

	public string budPrefabID;

	[Serialize]
	private Ref<HarvestDesignatable>[] buds = new Ref<HarvestDesignatable>[7];

	private StatusItem growingBranchesStatusItem;

	[Serialize]
	private bool hasExtraSeedAvailable = false;

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private Coroutine newGameSpawnRoutine;

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnUprootedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnDrownedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnHarvestDesignationChangedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.UpdateAllBudsHarvestStatus(data);
	});

	private static List<int> spawn_choices = new List<int>();

	public bool ExtraSeedAvailable => hasExtraSeedAvailable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		simRenderLoadBalance = true;
		growingBranchesStatusItem = new StatusItem("GROWINGBRANCHES", "MISC", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 129022);
		Subscribe(1119167081, OnNewGameSpawnDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
		Subscribe(-750750377, OnDrownedDelegate);
		Subscribe(-266953818, OnHarvestDesignationChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		if (newGameSpawnRoutine != null)
		{
			StopCoroutine(newGameSpawnRoutine);
		}
		base.OnCleanUp();
	}

	private void OnNewGameSpawn(object data)
	{
		float percent = 1f;
		if ((double)Random.value < 0.1)
		{
			percent = Random.Range(0.75f, 0.99f);
		}
		else
		{
			newGameSpawnRoutine = StartCoroutine(NewGameSproutBudRoutine());
		}
		growing.OverrideMaturityLevel(percent);
	}

	private IEnumerator NewGameSproutBudRoutine()
	{
		int i = 0;
		if (i < buds.Length)
		{
			yield return (object)new WaitForEndOfFrame();
			/*Error: Unable to find new state assignment for yield return*/;
		}
		newGameSpawnRoutine = null;
		yield return (object)0;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void Sim4000ms(float dt)
	{
		if (growing.IsGrown() && !wilting.IsWilting())
		{
			TrySpawnRandomBud(null, 0f);
			GetComponent<KSelectable>().AddStatusItem(growingBranchesStatusItem, null);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(growingBranchesStatusItem, false);
		}
	}

	private void OnUprooted(object data = null)
	{
		YieldWood();
	}

	private void YieldWood()
	{
		Ref<HarvestDesignatable>[] array = buds;
		for (int i = 0; i < array.Length; i++)
		{
			HarvestDesignatable harvestDesignatable = array[i]?.Get();
			if ((Object)harvestDesignatable != (Object)null)
			{
				harvestDesignatable.Trigger(-216549700, null);
			}
		}
	}

	public float GetMaxBranchMaturity()
	{
		float max_maturity = 0f;
		GetMostMatureBranch(out max_maturity);
		return max_maturity;
	}

	public void ConsumeMass(float mass_to_consume)
	{
		float max_maturity;
		HarvestDesignatable mostMatureBranch = GetMostMatureBranch(out max_maturity);
		if ((bool)mostMatureBranch)
		{
			Growing component = mostMatureBranch.GetComponent<Growing>();
			if ((bool)component)
			{
				component.ConsumeMass(mass_to_consume);
			}
		}
	}

	private HarvestDesignatable GetMostMatureBranch(out float max_maturity)
	{
		max_maturity = 0f;
		HarvestDesignatable result = null;
		Ref<HarvestDesignatable>[] array = buds;
		for (int i = 0; i < array.Length; i++)
		{
			HarvestDesignatable harvestDesignatable = array[i]?.Get();
			if ((Object)harvestDesignatable != (Object)null)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(harvestDesignatable);
				if (amountInstance != null)
				{
					float num = amountInstance.value / amountInstance.GetMax();
					if (num > max_maturity)
					{
						max_maturity = num;
						result = harvestDesignatable;
					}
				}
			}
		}
		return result;
	}

	public void TrySpawnRandomBud(object data = null, float growth_percentage = 0f)
	{
		if (!uprooted.IsUprooted)
		{
			for (int i = 0; i < buds.Length; i++)
			{
				Vector3 budPosition = GetBudPosition(i);
				int cell = Grid.PosToCell(budPosition);
				if ((buds[i] == null || (Object)buds[i].Get() == (Object)null) && CanGrowInto(cell))
				{
					spawn_choices.Add(i);
				}
			}
			if (spawn_choices.Count > 0)
			{
				int index = Random.Range(0, spawn_choices.Count);
				int num = spawn_choices[index];
				Vector3 budPosition2 = GetBudPosition(num);
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(budPrefabID), budPosition2);
				gameObject.SetActive(true);
				gameObject.GetComponent<Growing>().OverrideMaturityLevel(growth_percentage);
				gameObject.GetComponent<TreeBud>().SetTrunkPosition(this, num);
				gameObject.GetComponent<BudUprootedMonitor>().SetParentObject(GetComponent<KPrefabID>());
				HarvestDesignatable component = gameObject.GetComponent<HarvestDesignatable>();
				buds[num] = new Ref<HarvestDesignatable>(component);
				UpdateBudHarvestState(component);
				if (!hasExtraSeedAvailable && Random.Range(0, 100) < 5)
				{
					hasExtraSeedAvailable = true;
				}
			}
			spawn_choices.Clear();
		}
	}

	public void ExtractExtraSeed()
	{
		if (hasExtraSeedAvailable)
		{
			hasExtraSeedAvailable = false;
			Vector3 position = base.transform.position;
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("ForestTreeSeed"), position);
			gameObject.SetActive(true);
		}
	}

	private void UpdateBudHarvestState(HarvestDesignatable bud)
	{
		HarvestDesignatable component = GetComponent<HarvestDesignatable>();
		bud.SetHarvestWhenReady(component.HarvestWhenReady);
	}

	public void OnBranchRemoved(int idx, TreeBud treeBud)
	{
		if (idx < 0 || idx >= buds.Length)
		{
			Debug.Assert(false, "invalid branch index " + idx);
		}
		HarvestDesignatable component = treeBud.GetComponent<HarvestDesignatable>();
		HarvestDesignatable harvestDesignatable = (buds[idx] == null) ? null : buds[idx].Get();
		if ((Object)component != (Object)harvestDesignatable)
		{
			Debug.LogWarningFormat(base.gameObject, "OnBranchRemoved branch {0} does not match known branch {1}", component, harvestDesignatable);
		}
		buds[idx] = null;
	}

	private void UpdateAllBudsHarvestStatus(object data = null)
	{
		Ref<HarvestDesignatable>[] array = buds;
		foreach (Ref<HarvestDesignatable> @ref in array)
		{
			if (@ref != null)
			{
				HarvestDesignatable harvestDesignatable = @ref.Get();
				if ((Object)harvestDesignatable == (Object)null)
				{
					Debug.LogWarning("harvest_designatable was null");
				}
				else
				{
					UpdateBudHarvestState(harvestDesignatable);
				}
			}
		}
	}

	public bool CanGrowInto(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			if (!Grid.Solid[cell])
			{
				int cell2 = Grid.CellAbove(cell);
				if (Grid.IsValidCell(cell2))
				{
					if (!Grid.IsSubstantialLiquid(cell2, 0.35f))
					{
						if (!((Object)Grid.Objects[cell, 1] != (Object)null))
						{
							if (!((Object)Grid.Objects[cell, 5] != (Object)null))
							{
								if (!Grid.Foundation[cell])
								{
									return true;
								}
								return false;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	private Vector3 GetBudPosition(int idx)
	{
		Vector3 result = base.transform.position;
		switch (idx)
		{
		case 0:
			result = base.transform.position + Vector3.left;
			break;
		case 1:
			result = base.transform.position + Vector3.left + Vector3.up;
			break;
		case 2:
			result = base.transform.position + Vector3.left + Vector3.up + Vector3.up;
			break;
		case 3:
			result = base.transform.position + Vector3.up + Vector3.up;
			break;
		case 4:
			result = base.transform.position + Vector3.right + Vector3.up + Vector3.up;
			break;
		case 5:
			result = base.transform.position + Vector3.right + Vector3.up;
			break;
		case 6:
			result = base.transform.position + Vector3.right;
			break;
		}
		return result;
	}
}
