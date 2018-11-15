using UnityEngine;

namespace Rendering.World
{
	public struct Mask
	{
		private TextureAtlas atlas;

		private int texture_idx;

		private bool transpose;

		private bool flip_x;

		private bool flip_y;

		private int atlas_offset;

		private const int TILES_PER_SET = 4;

		public Vector2 UV0
		{
			get;
			private set;
		}

		public Vector2 UV1
		{
			get;
			private set;
		}

		public Vector2 UV2
		{
			get;
			private set;
		}

		public Vector2 UV3
		{
			get;
			private set;
		}

		public bool IsOpaque
		{
			get;
			private set;
		}

		public Mask(TextureAtlas atlas, int texture_idx, bool transpose, bool flip_x, bool flip_y, bool is_opaque)
		{
			this = default(Mask);
			this.atlas = atlas;
			this.texture_idx = texture_idx;
			this.transpose = transpose;
			this.flip_x = flip_x;
			this.flip_y = flip_y;
			atlas_offset = 0;
			IsOpaque = is_opaque;
			Refresh();
		}

		public void SetOffset(int offset)
		{
			atlas_offset = offset;
			Refresh();
		}

		public void Refresh()
		{
			int num = atlas_offset * 4 + atlas_offset;
			if (num + texture_idx >= atlas.items.Length)
			{
				num = 0;
			}
			Vector4 uvBox = atlas.items[num + texture_idx].uvBox;
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			if (transpose)
			{
				float x = uvBox.x;
				float x2 = uvBox.z;
				if (flip_x)
				{
					x = uvBox.z;
					x2 = uvBox.x;
				}
				zero.x = x;
				zero2.x = x;
				zero3.x = x2;
				zero4.x = x2;
				float y = uvBox.y;
				float y2 = uvBox.w;
				if (flip_y)
				{
					y = uvBox.w;
					y2 = uvBox.y;
				}
				zero.y = y;
				zero2.y = y2;
				zero3.y = y;
				zero4.y = y2;
			}
			else
			{
				float x3 = uvBox.x;
				float x4 = uvBox.z;
				if (flip_x)
				{
					x3 = uvBox.z;
					x4 = uvBox.x;
				}
				zero.x = x3;
				zero2.x = x4;
				zero3.x = x3;
				zero4.x = x4;
				float y3 = uvBox.y;
				float y4 = uvBox.w;
				if (flip_y)
				{
					y3 = uvBox.w;
					y4 = uvBox.y;
				}
				zero.y = y4;
				zero2.y = y4;
				zero3.y = y3;
				zero4.y = y3;
			}
			UV0 = zero;
			UV1 = zero2;
			UV2 = zero3;
			UV3 = zero4;
		}
	}
}
