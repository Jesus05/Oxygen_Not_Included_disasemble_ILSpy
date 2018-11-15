using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class River : Path
	{
		public string element
		{
			get;
			set;
		}

		public string backgroundElement
		{
			get;
			set;
		}

		public float widthCenter
		{
			get;
			set;
		}

		public float widthBorder
		{
			get;
			set;
		}

		public float temperature
		{
			get;
			set;
		}

		public float maxMass
		{
			get;
			set;
		}

		public float flowIn
		{
			get;
			set;
		}

		public float flowOut
		{
			get;
			set;
		}

		public River()
		{
			pathElements = new List<Segment>();
		}

		public River(Node t0, Node t1, string element = "Water", string backgroundElement = "Granite", float temperature = 373f, float maxMass = 2000f, float flowIn = 1000f, float flowOut = 100f, float widthCenter = 1.5f, float widthBorder = 1.5f)
			: this()
		{
			this.element = element;
			this.backgroundElement = backgroundElement;
			AddSection(t0, t1);
			this.temperature = temperature;
			this.maxMass = maxMass;
			this.flowIn = flowIn;
			this.flowOut = flowOut;
			this.widthCenter = widthCenter;
			this.widthBorder = widthBorder;
		}

		public River(River other, bool copySections = true)
		{
			if (copySections)
			{
				pathElements = new List<Segment>(other.pathElements);
			}
			element = element;
			backgroundElement = backgroundElement;
			temperature = other.temperature;
			maxMass = other.maxMass;
			flowIn = other.flowIn;
			flowOut = other.flowOut;
			widthCenter = other.widthCenter;
			widthBorder = other.widthBorder;
		}

		public void AddSection(Node t0, Node t1)
		{
			pathElements.Add(new Segment(t0.position, t1.position));
		}

		public Vector2 SourcePosition()
		{
			Segment segment = pathElements[0];
			return segment.e0;
		}

		public Vector2 SinkPosition()
		{
			Segment segment = pathElements[pathElements.Count - 1];
			return segment.e1;
		}
	}
}
