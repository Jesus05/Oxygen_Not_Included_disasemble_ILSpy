using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class Deconstructable : Workable
{
	private Chore chore = null;

	public bool allowDeconstruction = true;

	[Serialize]
	private bool isMarkedForDeconstruction;

	[Serialize]
	public Tag[] constructionElements;

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnDeconstruct(data);
	});

	private static readonly Vector2 INITIAL_VELOCITY_RANGE = new Vector2(0.5f, 4f);

	private bool destroyed = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		Building component = GetComponent<Building>();
		CellOffset[][] table = OffsetGroups.InvertedStandardTable;
		if (component.Def.IsTilePiece)
		{
			table = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(component.Def.PlacementOffsets, table, component.Def.ConstructionOffsetFilter);
		SetOffsetTable(offsetTable);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-111137758, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(-790448070, OnDeconstructDelegate);
		if (constructionElements == null || constructionElements.Length == 0)
		{
			constructionElements = new Tag[1];
			constructionElements[0] = GetComponent<PrimaryElement>().Element.tag;
		}
		if (isMarkedForDeconstruction)
		{
			QueueDeconstruction();
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	public override float GetWorkTime()
	{
		return GetComponent<Building>().Def.ConstructionTime * 0.5f;
	}

	protected override void OnStartWork(Worker worker)
	{
		progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		Building building = GetComponent<Building>();
		SimCellOccupier component2 = GetComponent<SimCellOccupier>();
		if ((UnityEngine.Object)DetailsScreen.Instance != (UnityEngine.Object)null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(false);
		}
		float temperature = component.Temperature;
		byte disease_idx = component.DiseaseIdx;
		int disease_count = component.DiseaseCount;
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			if (building.Def.TileLayer != ObjectLayer.NumLayers)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if ((UnityEngine.Object)Grid.Objects[num, (int)building.Def.TileLayer] == (UnityEngine.Object)base.gameObject)
				{
					Grid.Objects[num, (int)building.Def.ObjectLayer] = null;
					Grid.Objects[num, (int)building.Def.TileLayer] = null;
					Grid.Foundation[num] = false;
					TileVisualizer.RefreshCell(num, building.Def.TileLayer, building.Def.ReplacementLayer);
				}
			}
			component2.DestroySelf(delegate
			{
				TriggerDestroy(building, temperature, disease_idx, disease_count);
			});
		}
		else
		{
			TriggerDestroy(building, temperature, disease_idx, disease_count);
		}
		string sound = GlobalAssets.GetSound("Finish_Deconstruction_" + building.Def.AudioSize, false);
		if (sound != null)
		{
			KMonoBehaviour.PlaySound3DAtLocation(sound, base.gameObject.transform.GetPosition());
		}
		Trigger(-702296337, this);
	}

	private void TriggerDestroy(Building building, float temperature, byte disease_idx, int disease_count)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && !destroyed)
		{
			for (int i = 0; i < constructionElements.Length && building.Def.Mass.Length > i; i++)
			{
				GameObject gameObject = SpawnItem(base.transform.GetPosition(), building.Def, constructionElements[i], building.Def.Mass[i], temperature, disease_idx, disease_count);
				gameObject.transform.SetPosition(gameObject.transform.GetPosition() + Vector3.up * 0.5f);
				int num = Grid.PosToCell(gameObject.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				Vector2 initial_velocity = default(Vector2);
				if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
				{
					initial_velocity = Vector2.zero;
				}
				else
				{
					Vector3 position = gameObject.transform.GetPosition();
					position.x += (UnityEngine.Random.value - 0.5f) * 0.5f;
					float num3 = UnityEngine.Random.Range(-1f, 1f);
					Vector2 iNITIAL_VELOCITY_RANGE = INITIAL_VELOCITY_RANGE;
					float x = num3 * iNITIAL_VELOCITY_RANGE.x;
					Vector2 iNITIAL_VELOCITY_RANGE2 = INITIAL_VELOCITY_RANGE;
					initial_velocity = new Vector2(x, iNITIAL_VELOCITY_RANGE2.y);
				}
				if (GameComps.Fallers.Has(gameObject))
				{
					GameComps.Fallers.Remove(gameObject);
				}
				GameComps.Fallers.Add(gameObject, initial_velocity);
			}
			destroyed = true;
			base.gameObject.DeleteObject();
		}
	}

	private void QueueDeconstruction()
	{
		if (chore == null)
		{
			if (DebugHandler.InstantBuildMode)
			{
				OnCompleteWork(null);
			}
			else
			{
				Prioritizable.AddRef(base.gameObject);
				chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, null, true, null, null, null, true, null, false, false, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, true);
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
				isMarkedForDeconstruction = true;
				Trigger(2108245096, "Deconstruct");
			}
		}
	}

	private void OnDeconstruct()
	{
		if (chore == null)
		{
			QueueDeconstruction();
		}
		else
		{
			CancelDeconstruction();
		}
	}

	public bool IsMarkedForDeconstruction()
	{
		return chore != null;
	}

	public void SetAllowDeconstruction(bool allow)
	{
		allowDeconstruction = allow;
		if (!allowDeconstruction)
		{
			CancelDeconstruction();
		}
	}

	public static GameObject SpawnItem(Vector3 position, BuildingDef def, Tag src_element, float src_mass, float src_temperature, byte disease_idx, int disease_count)
	{
		GameObject gameObject = null;
		int cell = Grid.PosToCell(position);
		CellOffset[] placementOffsets = def.PlacementOffsets;
		Element element = ElementLoader.GetElement(src_element);
		if (element != null)
		{
			float num = src_mass;
			for (int i = 0; (float)i < src_mass / 400f; i++)
			{
				int num2 = i % def.PlacementOffsets.Length;
				int cell2 = Grid.OffsetCell(cell, placementOffsets[num2]);
				float mass = num;
				if (num > 400f)
				{
					mass = 400f;
					num -= 400f;
				}
				gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(cell2, Grid.SceneLayer.Ore), mass, src_temperature, disease_idx, disease_count, false, false);
			}
		}
		else
		{
			for (int j = 0; (float)j < src_mass; j++)
			{
				int num3 = j % def.PlacementOffsets.Length;
				int cell3 = Grid.OffsetCell(cell, placementOffsets[num3]);
				GameObject prefab = Assets.GetPrefab(src_element);
				gameObject = GameUtil.KInstantiate(prefab, Grid.CellToPosCBC(cell3, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore, null, 0);
				gameObject.SetActive(true);
			}
		}
		return gameObject;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (allowDeconstruction)
		{
			object buttonInfo;
			if (chore == null)
			{
				string iconName = "action_deconstruct";
				string text = UI.USERMENUACTIONS.DEMOLISH.NAME;
				System.Action on_click = OnDeconstruct;
				string tooltipText = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_deconstruct";
				string text = UI.USERMENUACTIONS.DEMOLISH.NAME_OFF;
				System.Action on_click = OnDeconstruct;
				string iconName = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private void CancelDeconstruction()
	{
		if (chore != null)
		{
			chore.Cancel("Cancelled deconstruction");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
			ShowProgressBar(false);
			isMarkedForDeconstruction = false;
			Prioritizable.RemoveRef(base.gameObject);
		}
	}

	private void OnCancel(object data)
	{
		CancelDeconstruction();
	}

	private void OnDeconstruct(object data)
	{
		if (allowDeconstruction)
		{
			QueueDeconstruction();
		}
	}
}
