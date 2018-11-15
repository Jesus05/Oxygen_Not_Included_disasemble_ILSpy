using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Primitive;
using System;

namespace ProcGen.Noise
{
	public class Primitive : NoiseBase
	{
		public NoisePrimitive primative
		{
			get;
			set;
		}

		public NoiseQuality quality
		{
			get;
			set;
		}

		public int seed
		{
			get;
			set;
		}

		public float offset
		{
			get;
			set;
		}

		public Primitive()
		{
			primative = NoisePrimitive.ImprovedPerlin;
			quality = NoiseQuality.Best;
			seed = 0;
			offset = 1f;
		}

		public Primitive(Primitive src)
		{
			primative = src.primative;
			quality = src.quality;
			seed = src.seed;
			offset = src.offset;
		}

		public override Type GetObjectType()
		{
			return typeof(Primitive);
		}

		public IModule3D CreateModule(int globalSeed)
		{
			PrimitiveModule primitiveModule = null;
			switch (primative)
			{
			case NoisePrimitive.Constant:
				primitiveModule = new Constant(offset);
				break;
			case NoisePrimitive.Cylinders:
				primitiveModule = new Cylinders(offset);
				break;
			case NoisePrimitive.Spheres:
				primitiveModule = new Spheres(offset);
				break;
			case NoisePrimitive.BevinsGradient:
				primitiveModule = new BevinsGradient();
				break;
			case NoisePrimitive.BevinsValue:
				primitiveModule = new BevinsValue();
				break;
			case NoisePrimitive.ImprovedPerlin:
				primitiveModule = new ImprovedPerlin();
				break;
			case NoisePrimitive.SimplexPerlin:
				primitiveModule = new SimplexPerlin();
				break;
			}
			primitiveModule.Quality = quality;
			primitiveModule.Seed = globalSeed + seed;
			return (IModule3D)primitiveModule;
		}
	}
}
