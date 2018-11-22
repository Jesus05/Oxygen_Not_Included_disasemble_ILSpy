using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class DecorProvider : KMonoBehaviour, IEffectDescriptor, IGameObjectEffectDescriptor
{
	private struct Splat
	{
		private DecorProvider provider;

		private Extents extents;

		private HandleVector<int>.Handle partitionerEntry;

		private HandleVector<int>.Handle solidChangedPartitionerEntry;

		public float decor
		{
			get;
			private set;
		}

		public Splat(DecorProvider provider)
		{
			this = default(Splat);
			AttributeInstance decor = provider.decor;
			this.decor = 0f;
			if (decor != null)
			{
				this.decor = decor.GetTotalValue();
			}
			if (provider.HasTag(GameTags.Stored))
			{
				this.decor = 0f;
			}
			int num = Grid.PosToCell(provider.gameObject);
			if (Grid.IsValidCell(num))
			{
				if (!Grid.Transparent[num] && Grid.Solid[num] && (UnityEngine.Object)provider.simCellOccupier == (UnityEngine.Object)null)
				{
					this.decor = 0f;
				}
				if (this.decor != 0f)
				{
					provider.cellCount = 0;
					this.provider = provider;
					int num2 = 5;
					AttributeInstance decorRadius = provider.decorRadius;
					if (decorRadius != null)
					{
						num2 = (int)decorRadius.GetTotalValue();
					}
					Orientation orientation = Orientation.Neutral;
					if ((bool)provider.rotatable)
					{
						orientation = provider.rotatable.GetOrientation();
					}
					OccupyArea occupyArea = provider.occupyArea;
					extents = occupyArea.GetExtents(orientation);
					extents.x = Mathf.Max(extents.x - num2, 0);
					extents.y = Mathf.Max(extents.y - num2, 0);
					extents.width = Mathf.Min(extents.width + num2 * 2, Grid.WidthInCells - 1);
					extents.height = Mathf.Min(extents.height + num2 * 2, Grid.HeightInCells - 1);
					partitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatCollectDecorProviders", provider.gameObject, extents, GameScenePartitioner.Instance.decorProviderLayer, provider.onCollectDecorProvidersCallback);
					solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatSolidCheck", provider.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, provider.refreshPartionerCallback);
					AddDecor();
				}
			}
		}

		public void Clear()
		{
			if (decor != 0f)
			{
				RemoveDecor();
				GameScenePartitioner.Instance.Free(ref partitionerEntry);
				GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
			}
		}

		private void AddDecor()
		{
			int cell = Grid.PosToCell(provider);
			int val = extents.x + extents.width;
			int val2 = extents.y + extents.height;
			int x = extents.x;
			int y = extents.y;
			int x2 = 0;
			int y2 = 0;
			Grid.CellToXY(cell, out x2, out y2);
			val = Math.Min(val, Grid.WidthInCells);
			val2 = Math.Min(val2, Grid.HeightInCells);
			x = Math.Max(0, x);
			y = Math.Max(0, y);
			for (int i = x; i < val; i++)
			{
				for (int j = y; j < val2; j++)
				{
					if (Grid.VisibilityTest(x2, y2, i, j, false))
					{
						int num = Grid.XYToCell(i, j);
						if (Grid.IsValidCell(num))
						{
							Grid.Decor[num] += decor;
							provider.cells[provider.cellCount++] = num;
						}
					}
				}
			}
		}

		private void RemoveDecor()
		{
			if (decor != 0f && !((UnityEngine.Object)provider == (UnityEngine.Object)null))
			{
				for (int i = 0; i < provider.cellCount; i++)
				{
					int num = provider.cells[i];
					if (Grid.IsValidCell(num))
					{
						Grid.Decor[num] -= decor;
					}
				}
			}
		}
	}

	public const string ID = "DecorProvider";

	private int width;

	private int height;

	private int previousDecor;

	public float baseRadius;

	public float baseDecor;

	public string overrideName;

	private HandleVector<int>.Handle partitionerEntry;

	public System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectDecorProvidersCallback;

	public AttributeInstance decor;

	public AttributeInstance decorRadius;

	private AttributeModifier baseDecorModifier;

	private AttributeModifier baseDecorRadiusModifier;

	public bool isMovable;

	[MyCmpReq]
	public OccupyArea occupyArea;

	[MyCmpGet]
	public Pickupable pickupable;

	[MyCmpGet]
	public Rotatable rotatable;

	[MyCmpGet]
	public SimCellOccupier simCellOccupier;

	private int[] cells = new int[512];

	private int cellCount;

	[MyCmpReq]
	private Modifiers modifiers;

	private Splat splat;

	public void Refresh()
	{
		splat.Clear();
		splat = new Splat(this);
		KPrefabID component = GetComponent<KPrefabID>();
		bool flag = component.HasTag(RoomConstraints.ConstraintTags.Decor20);
		bool flag2 = decor.GetTotalValue() >= 20f;
		if (flag != flag2)
		{
			if (flag2)
			{
				component.AddTag(RoomConstraints.ConstraintTags.Decor20);
			}
			else
			{
				component.RemoveTag(RoomConstraints.ConstraintTags.Decor20);
			}
			Game.Instance.roomProber.SolidChangedEvent(Grid.PosToCell(this), true);
		}
	}

	public float GetDecorForCell(int cell)
	{
		for (int i = 0; i < cellCount; i++)
		{
			if (cells[i] == cell)
			{
				return splat.decor;
			}
		}
		return 0f;
	}

	public void SetValues(EffectorValues values)
	{
		baseDecor = (float)values.amount;
		baseRadius = (float)values.radius;
		if (IsInitialized())
		{
			UpdateBaseDecorModifiers();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		decor = this.GetAttributes().Add(Db.Get().BuildingAttributes.Decor);
		decorRadius = this.GetAttributes().Add(Db.Get().BuildingAttributes.DecorRadius);
		UpdateBaseDecorModifiers();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		refreshCallback = Refresh;
		refreshPartionerCallback = delegate
		{
			Refresh();
		};
		onCollectDecorProvidersCallback = OnCollectDecorProviders;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		isMovable = ((UnityEngine.Object)component != (UnityEngine.Object)null && component.isMovable);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "DecorProvider.OnSpawn");
		AttributeInstance attributeInstance = decor;
		attributeInstance.OnDirty = (System.Action)Delegate.Combine(attributeInstance.OnDirty, refreshCallback);
		AttributeInstance attributeInstance2 = decorRadius;
		attributeInstance2.OnDirty = (System.Action)Delegate.Combine(attributeInstance2.OnDirty, refreshCallback);
		Refresh();
	}

	private void UpdateBaseDecorModifiers()
	{
		Attributes attributes = this.GetAttributes();
		if (baseDecorModifier != null)
		{
			attributes.Remove(baseDecorModifier);
			attributes.Remove(baseDecorRadiusModifier);
			baseDecorModifier = null;
			baseDecorRadiusModifier = null;
		}
		if (baseDecor != 0f)
		{
			baseDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, baseDecor, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			baseDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, baseRadius, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			attributes.Add(baseDecorModifier);
			attributes.Add(baseDecorRadiusModifier);
		}
	}

	private void OnCellChange()
	{
		Refresh();
	}

	private void OnCollectDecorProviders(object data)
	{
		List<DecorProvider> list = (List<DecorProvider>)data;
		list.Add(this);
	}

	public string GetName()
	{
		if (!string.IsNullOrEmpty(overrideName))
		{
			return overrideName;
		}
		return GetComponent<KSelectable>().GetName();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			AttributeInstance attributeInstance = decor;
			attributeInstance.OnDirty = (System.Action)Delegate.Remove(attributeInstance.OnDirty, refreshCallback);
			AttributeInstance attributeInstance2 = decorRadius;
			attributeInstance2.OnDirty = (System.Action)Delegate.Remove(attributeInstance2.OnDirty, refreshCallback);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		}
		splat.Clear();
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (decor != null && decorRadius != null)
		{
			float totalValue = decor.GetTotalValue();
			float totalValue2 = decorRadius.GetTotalValue();
			string arg = (!(baseDecor > 0f)) ? "consumed" : "produced";
			string str = (!(baseDecor > 0f)) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED;
			str = str + "\n\n" + decor.GetAttributeValueTooltip();
			string text = GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f);
			Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, arg, text, totalValue2), string.Format(str, text, totalValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(item);
		}
		else if (baseDecor != 0f)
		{
			string arg2 = (!(baseDecor >= 0f)) ? "consumed" : "produced";
			string format = (!(baseDecor >= 0f)) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED;
			string text2 = GameUtil.AddPositiveSign(baseDecor.ToString(), baseDecor > 0f);
			Descriptor item2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, arg2, text2, baseRadius), string.Format(format, text2, baseRadius), Descriptor.DescriptorType.Effect, false);
			list.Add(item2);
		}
		return list;
	}

	public static int GetLightDecorBonus(int cell)
	{
		if (Grid.LightIntensity[cell] <= 0)
		{
			return 0;
		}
		return DECOR.LIT_BONUS;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return GetEffectDescriptions();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}
}
