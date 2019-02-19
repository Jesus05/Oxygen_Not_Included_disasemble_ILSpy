namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Line : AbstractModel
	{
		protected struct Position
		{
			public float x;

			public float y;

			public float z;

			public Position(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}

		protected bool _attenuate = true;

		protected Position _startPosition = new Position(0f, 0f, 0f);

		protected Position _endPosition = new Position(0f, 0f, 0f);

		public bool Attenuate
		{
			get
			{
				return _attenuate;
			}
			set
			{
				_attenuate = value;
			}
		}

		public Line()
		{
		}

		public Line(IModule module)
			: base(module)
		{
		}

		public void SetStartPoint(float x, float y, float z)
		{
			_startPosition.x = x;
			_startPosition.y = y;
			_startPosition.z = z;
		}

		public void SetEndPoint(float x, float y, float z)
		{
			_endPosition.x = x;
			_endPosition.y = y;
			_endPosition.z = z;
		}

		public float GetValue(float p)
		{
			float x = (_endPosition.x - _startPosition.x) * p + _startPosition.x;
			float y = (_endPosition.y - _startPosition.y) * p + _startPosition.y;
			float z = (_endPosition.z - _startPosition.z) * p + _startPosition.z;
			float value = ((IModule3D)_sourceModule).GetValue(x, y, z);
			if (_attenuate)
			{
				return p * (1f - p) * 4f * value;
			}
			return value;
		}
	}
}
