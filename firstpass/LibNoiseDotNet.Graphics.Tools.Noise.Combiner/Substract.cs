namespace LibNoiseDotNet.Graphics.Tools.Noise.Combiner
{
	public class Substract : CombinerModule, IModule3D, IModule
	{
		public Substract()
		{
		}

		public Substract(IModule left, IModule right)
			: base(left, right)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)_leftModule).GetValue(x, y, z) - ((IModule3D)_rightModule).GetValue(x, y, z);
		}
	}
}
