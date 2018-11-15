namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public interface IMap2D<T>
	{
		int Width
		{
			get;
		}

		int Height
		{
			get;
		}

		T BorderValue
		{
			get;
			set;
		}

		T GetValue(int x, int y);

		void SetValue(int x, int y, T value);

		void SetSize(int width, int height);

		void Reset();

		void Clear(T value);

		void Clear();

		void MinMax(out T min, out T max);
	}
}
