using UnityEngine;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Scale2d : ModifierModule, IModule3D, IModule
	{
		public const float DEFAULT_SCALE = 1f;

		protected Vector2 _scale = Vector2.one * 1f;

		public Vector2 Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
			}
		}

		public Scale2d()
		{
		}

		public Scale2d(IModule source)
			: base(source)
		{
		}

		public Scale2d(IModule source, Vector2 scale)
			: base(source)
		{
			_scale = scale;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)_sourceModule).GetValue(x * _scale.x, y, z * _scale.y);
		}
	}
}
