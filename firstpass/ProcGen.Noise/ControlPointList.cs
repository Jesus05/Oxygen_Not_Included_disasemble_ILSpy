using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen.Noise
{
	[Serializable]
	public class ControlPointList : NoiseBase
	{
		public class Control
		{
			public float input
			{
				get;
				set;
			}

			public float output
			{
				get;
				set;
			}

			public Control()
			{
				input = 0f;
				output = 0f;
			}

			public Control(float i, float o)
			{
				input = i;
				output = o;
			}
		}

		[SerializeField]
		public List<Control> points
		{
			get;
			set;
		}

		public ControlPointList()
		{
			points = new List<Control>();
		}

		public override Type GetObjectType()
		{
			return typeof(ControlPointList);
		}

		public List<ControlPoint> GetControls()
		{
			List<ControlPoint> list = new List<ControlPoint>();
			for (int i = 0; i < points.Count; i++)
			{
				list.Add(new ControlPoint(points[i].input, points[i].output));
			}
			return list;
		}
	}
}
