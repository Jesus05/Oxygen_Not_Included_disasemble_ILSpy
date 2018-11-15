using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen.Noise
{
	[Serializable]
	public class FloatList : NoiseBase
	{
		[SerializeField]
		public List<float> points
		{
			get;
			set;
		}

		public FloatList()
		{
			points = new List<float>();
		}

		public override Type GetObjectType()
		{
			return typeof(FloatList);
		}
	}
}
