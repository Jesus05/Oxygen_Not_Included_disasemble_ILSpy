#define UNITY_ASSERTIONS
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class Light2D : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public Color Color = Color.white;

	public float Range = 5f;

	public float Angle;

	public int Lux = 1000;

	public Vector2 Direction;

	public Vector2 Offset;

	public bool drawOverlay;

	public Color overlayColour;

	public LightShape shape;

	private int cell = Grid.InvalidCell;

	public MaterialPropertyBlock materialPropertyBlock;

	private bool isRegistered;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle liquidPartitionerEntry;

	private LightGridManager.LightGridEmitter emitter;

	private List<int> litCells = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<Light2D> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Light2D>(delegate(Light2D component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public float IntensityAnimation
	{
		get;
		set;
	}

	protected override void OnPrefabInit()
	{
		Subscribe(-592767678, OnOperationalChangedDelegate);
		IntensityAnimation = 1f;
	}

	protected override void OnCmpEnable()
	{
		materialPropertyBlock = new MaterialPropertyBlock();
		base.OnCmpEnable();
		Components.Light2Ds.Add(this);
		if (base.isSpawned)
		{
			Refresh();
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "Light2D.OnCmpEnable");
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Refresh();
	}

	protected override void OnCmpDisable()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChanged);
		Components.Light2Ds.Remove(this);
		base.OnCmpDisable();
		Refresh();
	}

	protected override void OnCleanUp()
	{
		UnregisterLight();
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref liquidPartitionerEntry);
	}

	private void OnCellChanged()
	{
		GetComponent<Light2D>().Refresh();
	}

	private void UnregisterLight()
	{
		if (isRegistered && Grid.IsValidCell(cell))
		{
			GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref liquidPartitionerEntry);
			isRegistered = false;
		}
		if (emitter != null)
		{
			emitter.Remove();
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		UnregisterLight();
		Operational component = GetComponent<Operational>();
		if ((!((Object)component != (Object)null) || component.IsOperational) && base.isActiveAndEnabled)
		{
			Vector3 position = base.transform.GetPosition();
			position = new Vector3(position.x + Offset.x, position.y + Offset.y, position.z);
			int num = Grid.PosToCell(position);
			if (Grid.IsValidCell(num))
			{
				Vector2I vector2I = Grid.CellToXY(num);
				int num2 = (int)Range;
				if (shape == LightShape.Circle)
				{
					Vector2I vector2I2 = new Vector2I(vector2I.x - num2, vector2I.y - num2);
					solidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I2.x, vector2I2.y, 2 * num2, 2 * num2, GameScenePartitioner.Instance.solidChangedLayer, TriggerRefresh);
					liquidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I2.x, vector2I2.y, 2 * num2, 2 * num2, GameScenePartitioner.Instance.liquidChangedLayer, TriggerRefresh);
				}
				else if (shape == LightShape.Cone)
				{
					Vector2I vector2I3 = new Vector2I(vector2I.x - num2, vector2I.y - num2);
					solidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I3.x, vector2I3.y, 2 * num2, num2, GameScenePartitioner.Instance.solidChangedLayer, TriggerRefresh);
					liquidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I3.x, vector2I3.y, 2 * num2, num2, GameScenePartitioner.Instance.liquidChangedLayer, TriggerRefresh);
				}
				else
				{
					UnityEngine.Debug.Assert(false);
				}
				cell = num;
				litCells.Clear();
				emitter = new LightGridManager.LightGridEmitter(cell, litCells, Lux, Range, Color, shape, 0.5f);
				emitter.Add();
				isRegistered = true;
			}
		}
	}

	private void TriggerRefresh(object data)
	{
		Refresh();
	}

	private void OnOperationalChanged(object data)
	{
		base.enabled = GetComponent<Operational>().IsOperational;
		Refresh();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, Range), UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT, Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
