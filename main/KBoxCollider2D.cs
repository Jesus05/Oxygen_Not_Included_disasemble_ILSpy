using UnityEngine;

public class KBoxCollider2D : KCollider2D
{
	[SerializeField]
	private Vector2 _size;

	public Vector2 size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			MarkDirty(false);
		}
	}

	public override Bounds bounds
	{
		get
		{
			Vector3 position = base.transform.GetPosition();
			Vector2 offset = base.offset;
			float x = offset.x;
			Vector2 offset2 = base.offset;
			Vector3 center = position + new Vector3(x, offset2.y, 0f);
			return new Bounds(center, new Vector3(_size.x, _size.y, 0f));
		}
	}

	public override Extents GetExtents()
	{
		Vector3 position = base.transform.GetPosition();
		Vector2 offset = base.offset;
		float x = offset.x;
		Vector2 offset2 = base.offset;
		Vector3 vector = position + new Vector3(x, offset2.y, 0f);
		Vector2 vector2 = size * 0.9999f;
		Vector2 vector3 = new Vector2(vector.x - vector2.x * 0.5f, vector.y - vector2.y * 0.5f);
		Vector2 vector4 = new Vector2(vector.x + vector2.x * 0.5f, vector.y + vector2.y * 0.5f);
		Vector2I vector2I = new Vector2I((int)vector3.x, (int)vector3.y);
		Vector2I vector2I2 = new Vector2I((int)vector4.x, (int)vector4.y);
		int width = vector2I2.x - vector2I.x + 1;
		int height = vector2I2.y - vector2I.y + 1;
		return new Extents(vector2I.x, vector2I.y, width, height);
	}

	public override bool Intersects(Vector2 intersect_pos)
	{
		Vector3 position = base.transform.GetPosition();
		Vector2 offset = base.offset;
		float x = offset.x;
		Vector2 offset2 = base.offset;
		Vector3 vector = position + new Vector3(x, offset2.y, 0f);
		float x2 = vector.x;
		Vector2 size = this.size;
		float x3 = x2 - size.x * 0.5f;
		float y = vector.y;
		Vector2 size2 = this.size;
		Vector2 vector2 = new Vector2(x3, y - size2.y * 0.5f);
		float x4 = vector.x;
		Vector2 size3 = this.size;
		float x5 = x4 + size3.x * 0.5f;
		float y2 = vector.y;
		Vector2 size4 = this.size;
		Vector2 vector3 = new Vector2(x5, y2 + size4.y * 0.5f);
		return intersect_pos.x >= vector2.x && intersect_pos.x <= vector3.x && intersect_pos.y >= vector2.y && intersect_pos.y <= vector3.y;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(bounds.center, new Vector3(_size.x, _size.y, 0f));
	}
}
