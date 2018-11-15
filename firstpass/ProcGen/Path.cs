using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class Path
	{
		public List<Segment> pathElements;

		public Path()
		{
			pathElements = new List<Segment>();
		}

		public void AddSegment(Segment segment)
		{
			pathElements.Add(segment);
		}

		public void AddSegment(Vector2 e0, Vector2 e1)
		{
			pathElements.Add(new Segment(e0, e1));
		}

		public void Stagger(SeededRandom rnd, float maxDistance = 10f, float staggerRange = 3f)
		{
			List<Segment> list = new List<Segment>();
			for (int i = 0; i < pathElements.Count; i++)
			{
				list.AddRange(pathElements[i].Stagger(rnd, maxDistance, staggerRange));
			}
			pathElements = list;
		}
	}
}
