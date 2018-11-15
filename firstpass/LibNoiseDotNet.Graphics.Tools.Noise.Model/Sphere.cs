namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Sphere : AbstractModel
	{
		public Sphere()
		{
		}

		public Sphere(IModule3D module)
			: base(module)
		{
		}

		public float GetValue(float lat, float lon)
		{
			float x = 0f;
			float y = 0f;
			float z = 0f;
			Libnoise.LatLonToXYZ(lat, lon, ref x, ref y, ref z);
			return ((IModule3D)_sourceModule).GetValue(x, y, z);
		}
	}
}
