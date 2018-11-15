namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Invert : ModifierModule, IModule3D, IModule
	{
		public Invert()
		{
		}

		public Invert(IModule source)
			: base(source)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return 0f - ((IModule3D)_sourceModule).GetValue(x, y, z);
		}
	}
}
