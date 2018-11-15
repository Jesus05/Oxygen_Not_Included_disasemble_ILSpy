using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Diggable : Workable
{
	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle unstableEntry;

	private MeshRenderer childRenderer;

	private bool isReachable;

	private Element originalDigElement;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[SerializeField]
	public HashedString choreTypeIdHash;

	[SerializeField]
	public Tag[] choreTags;

	[SerializeField]
	public Material[] materials;

	[SerializeField]
	public MeshRenderer materialDisplay;

	private bool isDigComplete;

	private static List<Tuple<string, Tag>> lasersForHardness = new List<Tuple<string, Tag>>
	{
		new Tuple<string, Tag>("dig", "fx_dig_splash"),
		new Tuple<string, Tag>("specialistdig", "fx_dig_splash")
	};

	private int handle;

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public Chore chore;

	public bool Reachable => isReachable;

	private Diggable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
		readyForRoleWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresRolePerk;
		faceTargetWhenWorking = true;
		Subscribe(-1432940121, OnReachableChangedDelegate);
		attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		multitoolContext = "dig";
		multitoolHitEffectTag = "fx_dig_splash";
		workingPstComplete = HashedString.Invalid;
		workingPstFailed = HashedString.Invalid;
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		originalDigElement = Grid.Element[num];
		KSelectable component = GetComponent<KSelectable>();
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig, null);
		UpdateColor(isReachable);
		Grid.Objects[num, 7] = base.gameObject;
		ChoreType chore_type = Db.Get().ChoreTypes.Dig;
		if (choreTypeIdHash.IsValid)
		{
			chore_type = Db.Get().ChoreTypes.GetByHash(choreTypeIdHash);
		}
		chore = new WorkChore<Diggable>(chore_type, this, null, choreTags, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		SetWorkTime(float.PositiveInfinity);
		partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", base.gameObject, Grid.PosToCell(this), GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		OnSolidChanged(null);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		handle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		Components.Diggables.Add(this);
		UpdateBuildableDiggables(num);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		AnimInfo result = default(AnimInfo);
		if (overrideAnims != null && overrideAnims.Length > 0)
		{
			result.overrideAnims = overrideAnims;
		}
		if (multitoolContext.IsValid && multitoolHitEffectTag.IsValid)
		{
			result.smi = new MultitoolController.Instance(this, worker, multitoolContext, Assets.GetPrefab(multitoolHitEffectTag));
		}
		return result;
	}

	public static void UpdateBuildableDiggables(int cell)
	{
		Queue<GameUtil.FloodFillInfo> floodFillNext = GameUtil.FloodFillNext;
		floodFillNext.Clear();
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellLeft(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellRight(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellAbove(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellBelow(cell),
			depth = 0
		});
		UpdateBuildableDiggables(floodFillNext);
		floodFillNext.Clear();
	}

	public static void UpdateBuildableDiggables(Queue<GameUtil.FloodFillInfo> queue)
	{
		List<Diggable> adjacentDiggables = new List<Diggable>();
		bool any_buildables = false;
		GameUtil.FloodFillConditional(queue, delegate(int visited_cell)
		{
			bool result = false;
			if (IsCellBuildable(visited_cell))
			{
				result = true;
				any_buildables = true;
			}
			else
			{
				GameObject gameObject = Grid.Objects[visited_cell, 7];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					result = true;
					adjacentDiggables.Add(gameObject.GetComponent<Diggable>());
				}
			}
			return result;
		}, GameUtil.FloodFillVisited, null, 10000);
		GameUtil.FloodFillVisited.Clear();
		if (any_buildables)
		{
			foreach (Diggable item in adjacentDiggables)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && Array.IndexOf(item.choreTags, GameTags.ChoreTypes.Building) < 0)
				{
					Array.Resize(ref item.choreTags, item.choreTags.Length + 1);
					item.choreTags[item.choreTags.Length - 1] = GameTags.ChoreTypes.Building;
				}
			}
		}
		else
		{
			foreach (Diggable item2 in adjacentDiggables)
			{
				if (!((UnityEngine.Object)item2 == (UnityEngine.Object)null))
				{
					int num = Array.IndexOf(item2.choreTags, GameTags.ChoreTypes.Building);
					if (num >= 0)
					{
						item2.choreTags[num] = item2.choreTags[item2.choreTags.Length - 1];
						Array.Resize(ref item2.choreTags, item2.choreTags.Length - 1);
					}
				}
			}
		}
		queue.Clear();
	}

	private static bool IsCellBuildable(int cell)
	{
		bool result = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			Constructable component = gameObject.GetComponent<Constructable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				result = true;
			}
		}
		return result;
	}

	private IEnumerator PeriodicUnstableFallingRecheck()
	{
		yield return (object)new WaitForSeconds(2f);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(JuniorMiner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(Miner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(SeniorMiner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	private void OnSolidChanged(object data)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && !((UnityEngine.Object)base.gameObject == (UnityEngine.Object)null))
		{
			GameScenePartitioner.Instance.Free(ref unstableEntry);
			int num = Grid.PosToCell(this);
			int num2 = -1;
			UpdateColor(isReachable);
			if (Grid.Element[num].hardness >= 150)
			{
				bool flag = false;
				foreach (Chore.PreconditionInstance precondition in chore.GetPreconditions())
				{
					Chore.PreconditionInstance current = precondition;
					if (current.id == ChorePreconditions.instance.HasRolePerk.id)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanDigNearlyImpenetrable);
				}
				requiredRolePerk = RoleManager.rolePerks.CanDigNearlyImpenetrable.id;
				materialDisplay.sharedMaterial = materials[2];
			}
			else if (Grid.Element[num].hardness >= 50)
			{
				bool flag2 = false;
				foreach (Chore.PreconditionInstance precondition2 in chore.GetPreconditions())
				{
					Chore.PreconditionInstance current2 = precondition2;
					if (current2.id == ChorePreconditions.instance.HasRolePerk.id)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanDigVeryFirm);
				}
				requiredRolePerk = RoleManager.rolePerks.CanDigVeryFirm.id;
				materialDisplay.sharedMaterial = materials[1];
			}
			else
			{
				requiredRolePerk = HashedString.Invalid;
				chore.GetPreconditions().Remove(chore.GetPreconditions().Find((Chore.PreconditionInstance o) => o.id == ChorePreconditions.instance.HasRolePerk.id));
			}
			UpdateStatusItem(null);
			bool flag3 = false;
			if (!Grid.Solid[num])
			{
				num2 = GetUnstableCellAbove(num);
				if (num2 == -1)
				{
					flag3 = true;
				}
				else
				{
					StartCoroutine("PeriodicUnstableFallingRecheck");
				}
			}
			else if (Grid.Foundation[num])
			{
				flag3 = true;
			}
			if (flag3)
			{
				isDigComplete = true;
				if (chore == null || !chore.InProgress())
				{
					Util.KDestroyGameObject(base.gameObject);
				}
				else
				{
					GetComponentInChildren<MeshRenderer>().enabled = false;
				}
			}
			else if (num2 != -1)
			{
				Extents extents = default(Extents);
				Grid.CellToXY(num, out extents.x, out extents.y);
				extents.width = 1;
				extents.height = (num2 - num + Grid.WidthInCells - 1) / Grid.WidthInCells + 1;
				unstableEntry = GameScenePartitioner.Instance.Add("Diggable.OnSolidChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
			}
		}
	}

	public Element GetTargetElement()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Grid.Element[num];
	}

	public override string GetConversationTopic()
	{
		return originalDigElement.tag.Name;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int cell = Grid.PosToCell(this);
		DoDigTick(cell, dt);
		return isDigComplete;
	}

	protected override void OnStopWork(Worker worker)
	{
		if (isDigComplete)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public static void DoDigTick(int cell, float dt)
	{
		float num = (float)(int)Grid.Element[cell].hardness;
		if (num != 255f)
		{
			Element element = ElementLoader.FindElementByHash(SimHashes.Ice);
			float num2 = num / (float)(int)element.hardness;
			float num3 = Mathf.Min(Grid.Mass[cell], 400f) / 400f;
			float num4 = 4f * num3;
			float num5 = num4 + num2 * num4;
			float amount = dt / num5;
			WorldDamage.Instance.ApplyDamage(cell, amount, -1, -1, null, null);
		}
	}

	public static Diggable GetDiggable(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 7];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			return gameObject.GetComponent<Diggable>();
		}
		return null;
	}

	public static bool IsDiggable(int cell)
	{
		if (Grid.Solid[cell])
		{
			return !Grid.Foundation[cell];
		}
		return GetUnstableCellAbove(cell) != Grid.InvalidCell;
	}

	private static int GetUnstableCellAbove(int cell)
	{
		Vector2I cellXY = Grid.CellToXY(cell);
		UnstableGroundManager component = World.Instance.GetComponent<UnstableGroundManager>();
		List<int> cellsContainingFallingAbove = component.GetCellsContainingFallingAbove(cellXY);
		if (cellsContainingFallingAbove.Contains(cell))
		{
			return cell;
		}
		int num = Grid.CellAbove(cell);
		while (Grid.IsValidCell(num))
		{
			if (Grid.Foundation[num])
			{
				return Grid.InvalidCell;
			}
			if (Grid.Solid[num])
			{
				if (Grid.Element[num].IsUnstable)
				{
					return num;
				}
				return Grid.InvalidCell;
			}
			if (cellsContainingFallingAbove.Contains(num))
			{
				return num;
			}
			num = Grid.CellAbove(num);
		}
		return Grid.InvalidCell;
	}

	public static bool RequiresTool(Element e)
	{
		return false;
	}

	public static bool Undiggable(Element e)
	{
		return e.id == SimHashes.Unobtanium;
	}

	private void OnReachableChanged(object data)
	{
		if ((UnityEngine.Object)childRenderer == (UnityEngine.Object)null)
		{
			childRenderer = GetComponentInChildren<MeshRenderer>();
		}
		Material material = childRenderer.material;
		isReachable = (bool)data;
		Color color = material.color;
		Game.LocationColours dig = Game.Instance.uiColours.Dig;
		if (!(color == dig.invalidLocation))
		{
			UpdateColor(isReachable);
			KSelectable component = GetComponent<KSelectable>();
			if (isReachable)
			{
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, false);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
				GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
				}, null, null);
			}
		}
	}

	private void UpdateColor(bool reachable)
	{
		if ((UnityEngine.Object)childRenderer != (UnityEngine.Object)null)
		{
			Material material = childRenderer.material;
			if (RequiresTool(Grid.Element[Grid.PosToCell(base.gameObject)]) || Undiggable(Grid.Element[Grid.PosToCell(base.gameObject)]))
			{
				Material material2 = material;
				Game.LocationColours dig = Game.Instance.uiColours.Dig;
				material2.color = dig.invalidLocation;
			}
			else if (Grid.Element[Grid.PosToCell(base.gameObject)].hardness >= 50)
			{
				if (reachable)
				{
					Material material3 = material;
					Game.LocationColours dig2 = Game.Instance.uiColours.Dig;
					material3.color = dig2.validLocation;
				}
				else
				{
					Material material4 = material;
					Game.LocationColours dig3 = Game.Instance.uiColours.Dig;
					material4.color = dig3.unreachable;
				}
				multitoolContext = lasersForHardness[1].first;
				multitoolHitEffectTag = lasersForHardness[1].second;
			}
			else
			{
				if (reachable)
				{
					Material material5 = material;
					Game.LocationColours dig4 = Game.Instance.uiColours.Dig;
					material5.color = dig4.validLocation;
				}
				else
				{
					Material material6 = material;
					Game.LocationColours dig5 = Game.Instance.uiColours.Dig;
					material6.color = dig5.unreachable;
				}
				multitoolContext = lasersForHardness[0].first;
				multitoolHitEffectTag = lasersForHardness[0].second;
			}
		}
	}

	public override float GetPercentComplete()
	{
		return Grid.Damage[Grid.PosToCell(this)];
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		GameScenePartitioner.Instance.Free(ref unstableEntry);
		Game.Instance.Unsubscribe(handle);
		int cell = Grid.PosToCell(this);
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.digDestroyedLayer, null);
		Components.Diggables.Remove(this);
		UpdateBuildableDiggables(cell);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "icon_cancel";
		string text = UI.USERMENUACTIONS.CANCELDIG.NAME;
		System.Action on_click = OnCancel;
		string tooltipText = UI.USERMENUACTIONS.CANCELDIG.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
	}
}
