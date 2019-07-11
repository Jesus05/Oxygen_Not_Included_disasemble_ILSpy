#define UNITY_ASSERTIONS
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class Light2D : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public enum RefreshResult
	{
		None,
		Removed,
		Updated
	}

	private bool dirty_shape;

	private bool dirty_position;

	[SerializeField]
	private LightGridManager.LightGridEmitter.State pending_emitter_state = LightGridManager.LightGridEmitter.State.DEFAULT;

	public float Angle;

	public Vector2 Direction;

	[SerializeField]
	private Vector2 _offset;

	public bool drawOverlay;

	public Color overlayColour;

	public MaterialPropertyBlock materialPropertyBlock;

	private HandleVector<int>.Handle solidPartitionerEntry = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle liquidPartitionerEntry = HandleVector<int>.InvalidHandle;

	private static readonly EventSystem.IntraObjectHandler<Light2D> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Light2D>(delegate(Light2D light, object data)
	{
		light.enabled = (bool)data;
	});

	public LightShape shape
	{
		get
		{
			return pending_emitter_state.shape;
		}
		set
		{
			pending_emitter_state.shape = MaybeDirty(pending_emitter_state.shape, value, ref dirty_shape);
		}
	}

	public LightGridManager.LightGridEmitter emitter
	{
		get;
		private set;
	}

	public Color Color
	{
		get
		{
			return pending_emitter_state.colour;
		}
		set
		{
			pending_emitter_state.colour = value;
		}
	}

	public int Lux
	{
		get
		{
			return pending_emitter_state.intensity;
		}
		set
		{
			pending_emitter_state.intensity = value;
		}
	}

	public float Range
	{
		get
		{
			return pending_emitter_state.radius;
		}
		set
		{
			pending_emitter_state.radius = MaybeDirty(pending_emitter_state.radius, value, ref dirty_shape);
		}
	}

	private int origin
	{
		get
		{
			return pending_emitter_state.origin;
		}
		set
		{
			pending_emitter_state.origin = MaybeDirty(pending_emitter_state.origin, value, ref dirty_position);
		}
	}

	public float IntensityAnimation
	{
		get;
		set;
	}

	public Vector2 Offset
	{
		get
		{
			return _offset;
		}
		set
		{
			if (_offset != value)
			{
				_offset = value;
				origin = Grid.PosToCell(base.transform.GetPosition() + (Vector3)_offset);
			}
		}
	}

	private bool isRegistered => solidPartitionerEntry != HandleVector<int>.InvalidHandle;

	public Light2D()
	{
		emitter = new LightGridManager.LightGridEmitter();
		Range = 5f;
		Lux = 1000;
	}

	private T MaybeDirty<T>(T old_value, T new_value, ref bool dirty)
	{
		if (EqualityComparer<T>.Default.Equals(old_value, new_value))
		{
			return old_value;
		}
		dirty = true;
		return new_value;
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
			AddToScenePartitioner();
			emitter.Refresh(pending_emitter_state, true);
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			Game.Instance.roomProber.UpdateRoom(roomOfGameObject.cavity);
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnMoved, "Light2D.OnMoved");
	}

	protected override void OnCmpDisable()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnMoved);
		Components.Light2Ds.Remove(this);
		base.OnCmpDisable();
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			Game.Instance.roomProber.UpdateRoom(roomOfGameObject.cavity);
		}
		FullRemove();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		origin = Grid.PosToCell(base.transform.GetPosition() + (Vector3)Offset);
		if (base.isActiveAndEnabled)
		{
			AddToScenePartitioner();
			emitter.Refresh(pending_emitter_state, true);
		}
	}

	protected override void OnCleanUp()
	{
		FullRemove();
	}

	private void OnMoved()
	{
		if (base.isSpawned)
		{
			FullRefresh();
		}
	}

	private HandleVector<int>.Handle AddToLayer(Vector2I xy_min, int width, int height, ScenePartitionerLayer layer)
	{
		return GameScenePartitioner.Instance.Add("Light2D", base.gameObject, xy_min.x, xy_min.y, width, height, layer, OnWorldChanged);
	}

	private void AddToScenePartitioner()
	{
		Vector2I vector2I = Grid.CellToXY(origin);
		int num = (int)Range;
		Vector2I xy_min = new Vector2I(vector2I.x - num, vector2I.y - num);
		UnityEngine.Debug.Assert(shape == LightShape.Circle || shape == LightShape.Cone);
		int width = 2 * num;
		int height = (shape != 0) ? num : (2 * num);
		solidPartitionerEntry = AddToLayer(xy_min, width, height, GameScenePartitioner.Instance.solidChangedLayer);
		liquidPartitionerEntry = AddToLayer(xy_min, width, height, GameScenePartitioner.Instance.liquidChangedLayer);
	}

	private void RemoveFromScenePartitioner()
	{
		if (isRegistered)
		{
			GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref liquidPartitionerEntry);
		}
	}

	private void MoveInScenePartitioner()
	{
		GameScenePartitioner.Instance.UpdatePosition(solidPartitionerEntry, origin);
		GameScenePartitioner.Instance.UpdatePosition(liquidPartitionerEntry, origin);
	}

	[ContextMenu("Refresh")]
	public void FullRefresh()
	{
		if (base.isSpawned && base.isActiveAndEnabled)
		{
			DebugUtil.DevAssert(isRegistered, "shouldn't be refreshing if we aren't spawned and enabled");
			RefreshShapeAndPosition();
			emitter.Refresh(pending_emitter_state, true);
		}
	}

	public void FullRemove()
	{
		RemoveFromScenePartitioner();
		emitter.RemoveFromGrid();
	}

	public RefreshResult RefreshShapeAndPosition()
	{
		if (base.isSpawned)
		{
			if (base.isActiveAndEnabled)
			{
				int num = Grid.PosToCell(base.transform.GetPosition() + (Vector3)Offset);
				if (Grid.IsValidCell(num))
				{
					origin = num;
					if (dirty_shape)
					{
						RemoveFromScenePartitioner();
						AddToScenePartitioner();
					}
					else if (dirty_position)
					{
						MoveInScenePartitioner();
					}
					dirty_shape = false;
					dirty_position = false;
					return RefreshResult.Updated;
				}
				FullRemove();
				return RefreshResult.Removed;
			}
			FullRemove();
			return RefreshResult.Removed;
		}
		return RefreshResult.None;
	}

	private void OnWorldChanged(object data)
	{
		FullRefresh();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, Range), UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT, Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
