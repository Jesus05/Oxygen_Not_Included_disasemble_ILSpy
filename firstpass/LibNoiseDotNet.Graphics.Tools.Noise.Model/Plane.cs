namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Plane : AbstractModel
	{
		public Plane()
		{
		}

		public Plane(IModule module)
			: base(module)
		{
		}

		public float GetValue(float x, float z)
		{
			return ((IModule3D)_sourceModule).GetValue(x, 0f, z);
		}
	}
}
