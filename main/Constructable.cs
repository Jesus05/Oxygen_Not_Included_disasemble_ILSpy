using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Constructable : Workable, ISaveLoadable
{
	[MyCmpAdd]
	private Storage storage;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Rotatable rotatable;

	private Notification invalidLocation;

	private float initialTemperature = -1f;

	[Serialize]
	private bool isPrioritized;

	private FetchList2 fetchList;

	private Chore buildChore;

	private bool materialNeedsCleared;

	private bool hasUnreachableDigs;

	private bool finished;

	private bool unmarked;

	public bool isDiggingRequired = true;

	private bool waitForFetchesBeforeDigging;

	private bool hasLadderNearby;

	private Extents ladderDetectionExtents;

	[Serialize]
	public bool IsReplacementTile;

	[Serialize]
	public Tag[] choreTags;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle digPartitionerEntry;

	private HandleVector<int>.Handle ladderParititonerEntry;

	private LoggerFSS log = new LoggerFSS("Constructable", 35);

	[Serialize]
	private Tag[] selectedElementsTags;

	private Element[] selectedElements;

	[Serialize]
	private int[] ids;

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public Recipe Recipe => building.Def.CraftRecipe;

	public IList<Tag> SelectedElementsTags
	{
		get
		{
			return selectedElementsTags;
		}
		set
		{
			if (selectedElementsTags == null || selectedElementsTags.Length != value.Count)
			{
				selectedElementsTags = new Tag[value.Count];
			}
			value.CopyTo(selectedElementsTags, 0);
		}
	}

	public override string GetConversationTopic()
	{
		return building.Def.PrefabID;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		float num = 0f;
		float num2 = 0f;
		bool flag = true;
		foreach (GameObject item in storage.items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					num += component.Mass;
					num2 += component.Temperature * component.Mass;
					flag = (flag && component.HasTag(GameTags.Liquifiable));
				}
			}
		}
		if (num <= 0f)
		{
			Output.LogWarningWithObj(base.gameObject, "uhhh this constructable is about to generate a nan", "Item Count: ", storage.items.Count);
		}
		else
		{
			if (flag)
			{
				initialTemperature = Mathf.Min(num2 / num, 318.15f);
			}
			else
			{
				initialTemperature = Mathf.Clamp(num2 / num, 288.15f, 318.15f);
			}
			KAnimGraphTileVisualizer component2 = GetComponent<KAnimGraphTileVisualizer>();
			UtilityConnections connections = (!((UnityEngine.Object)component2 == (UnityEngine.Object)null)) ? component2.Connections : ((UtilityConnections)0);
			if (IsReplacementTile)
			{
				int cell = Grid.PosToCell(base.transform.GetLocalPosition());
				GameObject gameObject = Grid.Objects[cell, (int)building.Def.TileLayer];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					SimCellOccupier component3 = gameObject.GetComponent<SimCellOccupier>();
					if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
					{
						component3.DestroySelf(delegate
						{
							if ((UnityEngine.Object)this != (UnityEngine.Object)null && (UnityEngine.Object)base.gameObject != (UnityEngine.Object)null)
							{
								FinishConstruction(connections);
							}
						});
					}
					else
					{
						Conduit component4 = gameObject.GetComponent<Conduit>();
						if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
						{
							ConduitFlow flowManager = component4.GetFlowManager();
							flowManager.MarkForReplacement(cell);
						}
						BuildingComplete component5 = gameObject.GetComponent<BuildingComplete>();
						if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
						{
							component5.Subscribe(-21016276, delegate
							{
								GameScheduler.Instance.Schedule("finishConstruction", 0.001f, delegate
								{
									FinishConstruction(connections);
								}, null, null);
							});
						}
						else
						{
							Debug.LogWarning("Why am I trying to replace a: " + gameObject.name, null);
							FinishConstruction(connections);
						}
					}
					KAnimGraphTileVisualizer component6 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
					if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
					{
						component6.skipCleanup = true;
					}
					PrimaryElement component7 = gameObject.GetComponent<PrimaryElement>();
					float mass = component7.Mass;
					float temperature = component7.Temperature;
					byte diseaseIdx = component7.DiseaseIdx;
					int diseaseCount = component7.DiseaseCount;
					Deconstructable.SpawnItem(component7.transform.GetPosition(), component7.GetComponent<Building>().Def, component7.Element.tag, mass, temperature, diseaseIdx, diseaseCount);
					gameObject.Trigger(1606648047, null);
					gameObject.DeleteObject();
				}
			}
			else
			{
				FinishConstruction(connections);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, GetComponent<KSelectable>().GetName(), base.transform, 1.5f, false);
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(JuniorBuilder.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole(Builder.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole(SeniorBuilder.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	private void FinishConstruction(UtilityConnections connections)
	{
		Rotatable component = GetComponent<Rotatable>();
		Orientation orientation = ((UnityEngine.Object)component != (UnityEngine.Object)null) ? component.GetOrientation() : Orientation.Neutral;
		int cell = Grid.PosToCell(base.transform.GetLocalPosition());
		UnmarkArea();
		GameObject gameObject = building.Def.Build(cell, orientation, storage, selectedElementsTags, initialTemperature, true);
		gameObject.transform.rotation = base.transform.rotation;
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			component2.SetOrientation(orientation);
		}
		KAnimGraphTileVisualizer component3 = GetComponent<KAnimGraphTileVisualizer>();
		if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			KAnimGraphTileVisualizer component4 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
			component4.Connections = connections;
			component3.skipCleanup = true;
		}
		KSelectable component5 = GetComponent<KSelectable>();
		if ((UnityEngine.Object)component5 != (UnityEngine.Object)null && component5.IsSelected && (UnityEngine.Object)gameObject.GetComponent<KSelectable>() != (UnityEngine.Object)null)
		{
			component5.Unselect();
			if (PlayerController.Instance.ActiveTool.name == "SelectTool")
			{
				((SelectTool)PlayerController.Instance.ActiveTool).SelectNextFrame(gameObject.GetComponent<KSelectable>(), false);
			}
		}
		storage.ConsumeAllIgnoringDisease();
		finished = true;
		this.DeleteObject();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		invalidLocation = new Notification(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
		CellOffset[][] table = OffsetGroups.InvertedStandardTable;
		if (building.Def.IsTilePiece)
		{
			table = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(building.Def.PlacementOffsets, table, building.Def.ConstructionOffsetFilter);
		SetOffsetTable(offsetTable);
		storage.SetOffsetTable(offsetTable);
		faceTargetWhenWorking = true;
		Subscribe(-1432940121, OnReachableChangedDelegate);
		if ((UnityEngine.Object)rotatable == (UnityEngine.Object)null)
		{
			MarkArea();
		}
		workerStatusItem = Db.Get().DuplicantStatusItems.Building;
		workingStatusItem = null;
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		Prioritizable.AddRef(base.gameObject);
		synchronizeAnims = false;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		workingPstComplete = HashedString.Invalid;
		workingPstFailed = HashedString.Invalid;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(2127324410, OnCancelDelegate);
		if ((UnityEngine.Object)rotatable != (UnityEngine.Object)null)
		{
			MarkArea();
		}
		if (choreTags == null)
		{
			choreTags = GameTags.ChoreTypes.BuildingChores;
		}
		else if (Array.IndexOf(choreTags, GameTags.ChoreTypes.Building) < 0)
		{
			Array.Resize(ref choreTags, choreTags.Length + 1);
			choreTags[choreTags.Length - 1] = GameTags.ChoreTypes.Building;
		}
		this.fetchList = new FetchList2(storage, Db.Get().ChoreTypes.BuildFetch, choreTags);
		PrimaryElement component = GetComponent<PrimaryElement>();
		Element element = ElementLoader.GetElement(SelectedElementsTags[0]);
		component.ElementID = element.id;
		float num3 = component.Temperature = (component.Temperature = 293.15f);
		Recipe.Ingredient[] allIngredients = Recipe.GetAllIngredients(selectedElementsTags);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			FetchList2 fetchList = this.fetchList;
			Tag tag = ingredient.tag;
			num3 = ingredient.amount;
			fetchList.Add(tag, null, null, num3, FetchOrder2.OperationalRequirement.None);
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
		if (!building.Def.IsTilePiece)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Construction");
		}
		building.RunOnArea(delegate(int offset_cell)
		{
			if ((UnityEngine.Object)base.gameObject.GetComponent<ConduitBridge>() == (UnityEngine.Object)null)
			{
				GameObject gameObject2 = Grid.Objects[offset_cell, 7];
				if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
				{
					gameObject2.DeleteObject();
				}
			}
		});
		Diggable.UpdateBuildableDiggables(Grid.PosToCell(this));
		if (IsReplacementTile)
		{
			GameObject gameObject = null;
			if (building.Def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				int cell = Grid.PosToCell(base.transform.GetPosition());
				gameObject = Grid.Objects[cell, (int)building.Def.ReplacementLayer];
				if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null || (UnityEngine.Object)gameObject == (UnityEngine.Object)base.gameObject)
				{
					Grid.Objects[cell, (int)building.Def.ReplacementLayer] = base.gameObject;
					if ((UnityEngine.Object)base.gameObject.GetComponent<SimCellOccupier>() != (UnityEngine.Object)null)
					{
						int renderLayer = LayerMask.NameToLayer("Overlay");
						World.Instance.blockTileRenderer.AddBlock(renderLayer, building.Def, SimHashes.Void, cell);
					}
					TileVisualizer.RefreshCell(cell, building.Def.TileLayer, building.Def.ReplacementLayer);
				}
				else
				{
					Output.LogError("multiple replacement tiles on the same cell!");
					Util.KDestroyGameObject(base.gameObject);
				}
			}
		}
		bool flag = building.Def.BuildingComplete.GetComponent<Ladder>();
		waitForFetchesBeforeDigging = (flag || (bool)building.Def.BuildingComplete.GetComponent<SimCellOccupier>() || (bool)building.Def.BuildingComplete.GetComponent<Door>() || (bool)building.Def.BuildingComplete.GetComponent<LiquidPumpingStation>());
		if (flag)
		{
			int x = 0;
			int y = 0;
			int cell2 = Grid.PosToCell(this);
			Grid.CellToXY(cell2, out x, out y);
			int y2 = y - 3;
			ladderDetectionExtents = new Extents(x, y2, 1, 5);
			ladderParititonerEntry = GameScenePartitioner.Instance.Add("Constructable.OnNearbyBuildingLayerChanged", base.gameObject, ladderDetectionExtents, GameScenePartitioner.Instance.objectLayers[1], OnNearbyBuildingLayerChanged);
			OnNearbyBuildingLayerChanged(null);
		}
		this.fetchList.Submit(OnFetchListComplete, true);
		PlaceDiggables();
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Prioritizable component2 = GetComponent<Prioritizable>();
		Prioritizable obj = component2;
		obj.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(obj.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
		OnPriorityChanged(component2.GetMasterPriority());
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		building.RunOnArea(delegate(int cell)
		{
			Diggable diggable = Diggable.GetDiggable(cell);
			if ((UnityEngine.Object)diggable != (UnityEngine.Object)null)
			{
				diggable.GetComponent<Prioritizable>().SetMasterPriority(priority);
			}
		});
	}

	private void MarkArea()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingDef def = building.Def;
		Orientation orientation = building.Orientation;
		ObjectLayer layer = (!IsReplacementTile) ? def.ObjectLayer : def.ReplacementLayer;
		def.MarkArea(num, orientation, layer, base.gameObject);
		if (def.IsTilePiece)
		{
			GameObject x = Grid.Objects[num, (int)def.TileLayer];
			if ((UnityEngine.Object)x == (UnityEngine.Object)null)
			{
				def.MarkArea(num, orientation, def.TileLayer, base.gameObject);
				def.RunOnArea(num, orientation, delegate(int c)
				{
					TileVisualizer.RefreshCell(c, def.TileLayer, def.ReplacementLayer);
				});
			}
			Grid.IsTileUnderConstruction[num] = true;
		}
	}

	private void UnmarkArea()
	{
		if (!unmarked)
		{
			unmarked = true;
			int num = Grid.PosToCell(base.transform.GetPosition());
			BuildingDef def = building.Def;
			ObjectLayer layer = (!IsReplacementTile) ? building.Def.ObjectLayer : building.Def.ReplacementLayer;
			def.UnmarkArea(num, building.Orientation, layer, base.gameObject);
			if (def.IsTilePiece)
			{
				Grid.IsTileUnderConstruction[num] = false;
			}
		}
	}

	private void OnNearbyBuildingLayerChanged(object data)
	{
		hasLadderNearby = false;
		int num = ladderDetectionExtents.y;
		while (true)
		{
			if (num >= ladderDetectionExtents.y + ladderDetectionExtents.height)
			{
				return;
			}
			int num2 = Grid.OffsetCell(0, ladderDetectionExtents.x, num);
			if (Grid.IsValidCell(num2))
			{
				GameObject value = null;
				Grid.ObjectLayers[1].TryGetValue(num2, out value);
				if ((UnityEngine.Object)value != (UnityEngine.Object)null && (UnityEngine.Object)value.GetComponent<Ladder>() != (UnityEngine.Object)null)
				{
					break;
				}
			}
			num++;
		}
		hasLadderNearby = true;
	}

	private bool IsWire()
	{
		return building.Def.name.Contains("Wire");
	}

	public bool IconConnectionAnimation(float delay, int connectionCount, string defName, string soundName)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (this.building.Def.Name.Contains(defName))
		{
			Building building = null;
			GameObject gameObject = Grid.Objects[num, 1];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				building = gameObject.GetComponent<Building>();
			}
			if ((UnityEngine.Object)building != (UnityEngine.Object)null)
			{
				bool flag = IsWire();
				int num2 = (!flag) ? building.GetUtilityInputCell() : building.GetPowerInputCell();
				int num3 = (!flag) ? building.GetUtilityOutputCell() : num2;
				if (num == num2 || num == num3)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && ((!flag) ? component.RequiresUtilityConnection : component.RequiresPower))
					{
						component.ConnectedEventWithDelay(delay, connectionCount, num, soundName);
						return true;
					}
				}
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		if (IsReplacementTile && building.Def.isKAnimTile)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			GameObject gameObject = Grid.Objects[cell, (int)building.Def.ReplacementLayer];
			if ((UnityEngine.Object)gameObject == (UnityEngine.Object)base.gameObject && (UnityEngine.Object)gameObject.GetComponent<SimCellOccupier>() != (UnityEngine.Object)null)
			{
				World.Instance.blockTileRenderer.RemoveBlock(building.Def, SimHashes.Void, cell);
			}
		}
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref digPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref ladderParititonerEntry);
		SaveLoadRoot component = GetComponent<SaveLoadRoot>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			SaveLoader.Instance.saveManager.Unregister(component);
		}
		if (fetchList != null)
		{
			fetchList.Cancel("Constructable destroyed");
		}
		UnmarkArea();
		Queue<GameUtil.FloodFillInfo> floodFillNext = GameUtil.FloodFillNext;
		floodFillNext.Clear();
		int[] placementCells = building.PlacementCells;
		foreach (int cell2 in placementCells)
		{
			Diggable diggable = Diggable.GetDiggable(cell2);
			if ((UnityEngine.Object)diggable != (UnityEngine.Object)null)
			{
				diggable.gameObject.DeleteObject();
			}
			floodFillNext.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.CellLeft(cell2),
				depth = 0
			});
			floodFillNext.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.CellRight(cell2),
				depth = 0
			});
			floodFillNext.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.CellAbove(cell2),
				depth = 0
			});
			floodFillNext.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.CellBelow(cell2),
				depth = 0
			});
		}
		Diggable.UpdateBuildableDiggables(floodFillNext);
		base.OnCleanUp();
	}

	private void OnDiggableReachabilityChanged(object data)
	{
		if (!IsReplacementTile)
		{
			int diggable_count = 0;
			int unreachable_count = 0;
			building.RunOnArea(delegate(int offset_cell)
			{
				Diggable diggable = Diggable.GetDiggable(offset_cell);
				if ((UnityEngine.Object)diggable != (UnityEngine.Object)null)
				{
					diggable_count++;
					if (!diggable.GetComponent<KPrefabID>().HasTag(GameTags.Reachable))
					{
						unreachable_count++;
					}
				}
			});
			bool flag = unreachable_count > 0 && unreachable_count == diggable_count;
			if (flag != hasUnreachableDigs)
			{
				if (flag)
				{
					GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable, null);
				}
				else
				{
					GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable, false);
				}
				hasUnreachableDigs = flag;
			}
		}
	}

	private void PlaceDiggables()
	{
		if (waitForFetchesBeforeDigging && fetchList != null && !hasLadderNearby)
		{
			OnDiggableReachabilityChanged(null);
		}
		else
		{
			bool digs_complete = true;
			if (!solidPartitionerEntry.IsValid())
			{
				Extents validPlacementExtents = building.GetValidPlacementExtents();
				solidPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChangedOrDigDestroyed);
				digPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.digDestroyedLayer, OnSolidChangedOrDigDestroyed);
			}
			if (!IsReplacementTile)
			{
				building.RunOnArea(delegate(int offset_cell)
				{
					PrioritySetting masterPriority = GetComponent<Prioritizable>().GetMasterPriority();
					if (Diggable.IsDiggable(offset_cell))
					{
						digs_complete = false;
						Diggable diggable = Diggable.GetDiggable(offset_cell);
						if ((UnityEngine.Object)diggable == (UnityEngine.Object)null)
						{
							diggable = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), Grid.SceneLayer.Move, null, 0).GetComponent<Diggable>();
							diggable.gameObject.SetActive(true);
							diggable.transform.SetPosition(Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
							diggable.Subscribe(-1432940121, OnDiggableReachabilityChanged);
							Grid.Objects[offset_cell, 7] = diggable.gameObject;
						}
						else
						{
							diggable.Unsubscribe(-1432940121, OnDiggableReachabilityChanged);
							diggable.Subscribe(-1432940121, OnDiggableReachabilityChanged);
						}
						diggable.choreTypeIdHash = Db.Get().ChoreTypes.BuildDig.IdHash;
						diggable.choreTags = choreTags;
						diggable.GetComponent<Prioritizable>().SetMasterPriority(masterPriority);
						RenderUtil.EnableRenderer(diggable.transform, false);
						SaveLoadRoot component = diggable.GetComponent<SaveLoadRoot>();
						if ((UnityEngine.Object)component != (UnityEngine.Object)null)
						{
							UnityEngine.Object.Destroy(component);
						}
					}
				});
				OnDiggableReachabilityChanged(null);
			}
			bool flag = building.Def.IsValidBuildLocation(base.gameObject, base.transform.GetPosition(), building.Orientation);
			if (flag)
			{
				notifier.Remove(invalidLocation);
			}
			else
			{
				notifier.Add(invalidLocation, string.Empty);
			}
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag, this);
			bool flag2 = digs_complete && flag && fetchList == null;
			if (flag2 && buildChore == null)
			{
				ChoreType build = Db.Get().ChoreTypes.Build;
				Tag[] chore_tags = choreTags;
				buildChore = new WorkChore<Constructable>(build, this, null, chore_tags, true, UpdateBuildState, UpdateBuildState, UpdateBuildState, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				UpdateBuildState(buildChore);
			}
			else if (!flag2 && buildChore != null)
			{
				buildChore.Cancel("Need to dig");
				buildChore = null;
			}
		}
	}

	private void OnFetchListComplete()
	{
		fetchList = null;
		PlaceDiggables();
		ClearMaterialNeeds();
	}

	private void ClearMaterialNeeds()
	{
		if (!materialNeedsCleared)
		{
			Recipe.Ingredient[] allIngredients = Recipe.GetAllIngredients(SelectedElementsTags);
			foreach (Recipe.Ingredient ingredient in allIngredients)
			{
				MaterialNeeds.Instance.UpdateNeed(ingredient.tag, 0f - ingredient.amount);
			}
			materialNeedsCleared = true;
		}
	}

	private void OnSolidChangedOrDigDestroyed(object data)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && !finished)
		{
			PlaceDiggables();
		}
	}

	private void UpdateBuildState(Chore chore)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (chore.InProgress())
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstruction, null);
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstructionNoWorker, null);
		}
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		if (ids != null)
		{
			selectedElements = new Element[ids.Length];
			for (int i = 0; i < ids.Length; i++)
			{
				selectedElements[i] = ElementLoader.FindElementByHash((SimHashes)ids[i]);
			}
			if (selectedElementsTags == null)
			{
				selectedElementsTags = new Tag[ids.Length];
				for (int j = 0; j < ids.Length; j++)
				{
					selectedElementsTags[j] = ElementLoader.FindElementByHash((SimHashes)ids[j]).tag;
				}
			}
			for (int k = 0; k < selectedElements.Length; k++)
			{
			}
		}
	}

	private void OnReachableChanged(object data)
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if ((bool)data)
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, false);
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				KAnimControllerBase kAnimControllerBase = component;
				Game.LocationColours build = Game.Instance.uiColours.Build;
				kAnimControllerBase.TintColour = build.validLocation;
			}
		}
		else
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, this);
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				KAnimControllerBase kAnimControllerBase2 = component;
				Game.LocationColours build2 = Game.Instance.uiColours.Build;
				kAnimControllerBase2.TintColour = build2.unreachable;
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "icon_cancel";
		string text = UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME;
		System.Action on_click = OnPressCancel;
		string tooltipText = UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
	}

	private void OnPressCancel()
	{
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnCancel(object data = null)
	{
		DetailsScreen.Instance.Show(false);
		ClearMaterialNeeds();
	}
}
