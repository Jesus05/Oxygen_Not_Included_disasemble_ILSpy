using System;

public struct BatchKey : IEquatable<BatchKey>
{
	private float _z;

	private int _layer;

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private Vector2I _idx;

	private int _hash;

	public float z => _z;

	public int layer => _layer;

	public HashedString groupID => _groupID;

	public Vector2I idx => _idx;

	public KAnimBatchGroup.MaterialType materialType => _materialType;

	public int hash => _hash;

	private BatchKey(KAnimConverter.IAnimConverter controller)
	{
		_layer = controller.GetLayer();
		_groupID = controller.GetBatchGroupID(false);
		_materialType = controller.GetMaterialType();
		_z = controller.GetZ();
		_idx = KAnimBatchManager.ControllerToChunkXY(controller);
		_hash = 0;
	}

	private BatchKey(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		this = new BatchKey(controller);
		_idx = idx;
	}

	private void CalculateHash()
	{
		_hash = (_z.GetHashCode() ^ _layer ^ (int)_materialType ^ _groupID.HashValue ^ _idx.GetHashCode());
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		BatchKey result = new BatchKey(controller, idx);
		result.CalculateHash();
		return result;
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller)
	{
		BatchKey result = new BatchKey(controller);
		result.CalculateHash();
		return result;
	}

	public bool Equals(BatchKey other)
	{
		return _z == other._z && _layer == other._layer && _materialType == other._materialType && _groupID == other._groupID && _idx == other._idx;
	}

	public override int GetHashCode()
	{
		return _hash;
	}

	public override string ToString()
	{
		object[] obj = new object[12]
		{
			"[",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		Vector2I idx = this.idx;
		obj[1] = idx.x;
		obj[2] = ",";
		Vector2I idx2 = this.idx;
		obj[3] = idx2.y;
		obj[4] = "] [";
		obj[5] = groupID.HashValue;
		obj[6] = "] [";
		obj[7] = layer;
		obj[8] = "] [";
		obj[9] = z;
		obj[10] = "]";
		obj[11] = materialType.ToString();
		return string.Concat(obj);
	}
}
