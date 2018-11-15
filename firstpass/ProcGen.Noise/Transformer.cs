using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Tranformer;
using System;

namespace ProcGen.Noise
{
	public class Transformer : NoiseBase
	{
		public enum TransformerType
		{
			_UNSET_,
			Displace,
			Turbulence,
			RotatePoint
		}

		public TransformerType transformerType
		{
			get;
			set;
		}

		public float power
		{
			get;
			set;
		}

		public Vector2f rotation
		{
			get;
			set;
		}

		public Transformer()
		{
			transformerType = TransformerType.Displace;
			power = 1f;
			rotation = new Vector2f(0, 0);
		}

		public override Type GetObjectType()
		{
			return typeof(Transformer);
		}

		public IModule3D CreateModule()
		{
			if (transformerType == TransformerType.Turbulence)
			{
				Turbulence turbulence = new Turbulence();
				turbulence.Power = power;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				RotatePoint rotatePoint = new RotatePoint();
				RotatePoint rotatePoint2 = rotatePoint;
				Vector2f rotation = this.rotation;
				rotatePoint2.XAngle = rotation.x;
				RotatePoint rotatePoint3 = rotatePoint;
				Vector2f rotation2 = this.rotation;
				rotatePoint3.YAngle = rotation2.y;
				rotatePoint.ZAngle = 0f;
				return rotatePoint;
			}
			return new Displace();
		}

		public IModule3D CreateModule(IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (transformerType == TransformerType.Turbulence)
			{
				return new Turbulence(sourceModule, xModule, yModule, zModule, power);
			}
			if (transformerType == TransformerType.RotatePoint)
			{
				Vector2f rotation = this.rotation;
				float x = rotation.x;
				Vector2f rotation2 = this.rotation;
				return new RotatePoint(sourceModule, x, rotation2.y, 0f);
			}
			return new Displace(sourceModule, xModule, yModule, zModule);
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (transformerType == TransformerType.Turbulence)
			{
				Turbulence turbulence = target as Turbulence;
				turbulence.SourceModule = sourceModule;
				turbulence.XDistortModule = xModule;
				turbulence.YDistortModule = yModule;
				turbulence.ZDistortModule = zModule;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				RotatePoint rotatePoint = target as RotatePoint;
				rotatePoint.SourceModule = sourceModule;
			}
			else
			{
				Displace displace = target as Displace;
				displace.SourceModule = sourceModule;
				displace.XDisplaceModule = xModule;
				displace.YDisplaceModule = yModule;
				displace.ZDisplaceModule = zModule;
			}
		}
	}
}
