using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DrowningMonitor : KMonoBehaviour, IWiltCause, ISim1000ms
{
	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Effects effects;

	private OccupyArea _occupyArea;

	[Serialize]
	[SerializeField]
	private float timeToDrown;

	[Serialize]
	private bool drowned;

	private bool drowning = false;

	protected const float MaxDrownTime = 75f;

	protected const float RegenRate = 5f;

	protected const float CellLiquidThreshold = 0.95f;

	public bool canDrownToDeath = true;

	private Extents extents;

	private HandleVector<int>.Handle partitionerEntry;

	public static Effect drowningEffect;

	[CompilerGenerated]
	private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[1]
			{
				WiltCondition.Condition.Drowning
			};
		}
	}

	private OccupyArea occupyArea
	{
		get
		{
			if ((UnityEngine.Object)_occupyArea == (UnityEngine.Object)null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public bool Drowning => drowning;

	public string WiltStateString => CREATURES.STATUSITEMS.DROWNING.NAME;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		timeToDrown = 75f;
		if (drowningEffect == null)
		{
			drowningEffect = new Effect("Drowning", CREATURES.STATUSITEMS.DROWNING.NAME, CREATURES.STATUSITEMS.DROWNING.TOOLTIP, 0f, false, false, true, null, 0f, null);
			drowningEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.DROWNING.NAME, false, false, true));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnMove();
		CheckDrowning(null);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnMove, "DrowningMonitor.OnSpawn");
	}

	private void OnMove()
	{
		if (partitionerEntry.IsValid())
		{
			Extents extents = occupyArea.GetExtents();
			GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, extents.x, extents.y);
		}
		else
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, OnLiquidChanged);
		}
		CheckDrowning(null);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnMove);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void CheckDrowning(object data = null)
	{
		if (!drowned)
		{
			int cell = Grid.PosToCell(base.gameObject.transform.GetPosition());
			if (!IsCellSafe(cell))
			{
				if (!drowning)
				{
					drowning = true;
					Trigger(1949704522, null);
					GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning);
				}
				if (timeToDrown <= 0f && canDrownToDeath)
				{
					this.GetSMI<DeathMonitor.Instance>()?.Kill(Db.Get().Deaths.Drowned);
					Trigger(-750750377, null);
					drowned = true;
				}
			}
			else if (drowning)
			{
				drowning = false;
				GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Drowning);
				Trigger(99949694, null);
			}
			selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.Drowning, drowning, this);
			if ((UnityEngine.Object)effects != (UnityEngine.Object)null)
			{
				if (drowning)
				{
					effects.Add(drowningEffect, false);
				}
				else
				{
					effects.Remove(drowningEffect);
				}
			}
		}
	}

	private static bool CellSafeTest(int testCell, object data)
	{
		int num = Grid.CellAbove(testCell);
		if (Grid.IsValidCell(testCell) && Grid.IsValidCell(num))
		{
			if (!Grid.IsSubstantialLiquid(testCell, 0.95f))
			{
				if (Grid.IsLiquid(testCell))
				{
					if (Grid.Element[num].IsLiquid)
					{
						return false;
					}
					if (Grid.Element[num].IsSolid)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public bool IsCellSafe(int cell)
	{
		return occupyArea.TestArea(cell, this, CellSafeTest);
	}

	private void OnLiquidChanged(object data)
	{
		CheckDrowning(null);
	}

	public void Sim1000ms(float dt)
	{
		CheckDrowning(null);
		if (drowning)
		{
			if (!drowned)
			{
				timeToDrown -= dt;
				if (timeToDrown <= 0f)
				{
					CheckDrowning(null);
				}
			}
		}
		else
		{
			timeToDrown += dt * 5f;
			timeToDrown = Mathf.Clamp(timeToDrown, 0f, 75f);
		}
	}
}
