using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Segment
	{
		public Vector2 e0;

		public Vector2 e1;

		public Segment(Vector2 e0, Vector2 e1)
		{
			this.e0 = e0;
			this.e1 = e1;
		}

		public List<Segment> Stagger(SeededRandom rnd, float maxDistance = 10f, float staggerRange = 3f)
		{
			List<Segment> list = new List<Segment>();
			Vector2 a = e1 - e0;
			Vector2 vector = e0;
			Vector2 vector2 = e1;
			float magnitude = a.magnitude;
			float num = magnitude / maxDistance;
			Vector2 normalized = new Vector2(0f - a.y, a.x).normalized;
			for (int i = 0; (float)i < num; i++)
			{
				vector2 = e0 + a * (1f / num) * (float)i + normalized * rnd.RandomRange(0f - staggerRange, staggerRange);
				list.Add(new Segment(vector, vector2));
				vector = vector2;
			}
			list.Add(new Segment(vector2, e1));
			return list;
		}
	}
}
