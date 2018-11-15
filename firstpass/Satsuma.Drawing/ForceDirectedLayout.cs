using System;
using System.Collections.Generic;

namespace Satsuma.Drawing
{
	public sealed class ForceDirectedLayout
	{
		public const double DefaultStartingTemperature = 0.2;

		public const double DefaultMinimumTemperature = 0.01;

		public const double DefaultTemperatureAttenuation = 0.95;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Dictionary<Node, PointD> NodePositions
		{
			get;
			private set;
		}

		public Func<double, double> SpringForce
		{
			get;
			set;
		}

		public Func<double, double> ElectricForce
		{
			get;
			set;
		}

		public Func<PointD, PointD> ExternalForce
		{
			get;
			set;
		}

		public double Temperature
		{
			get;
			set;
		}

		public double TemperatureAttenuation
		{
			get;
			set;
		}

		public ForceDirectedLayout(IGraph graph, Func<Node, PointD> initialPositions = null, int seed = -1)
		{
			Graph = graph;
			NodePositions = new Dictionary<Node, PointD>();
			SpringForce = ((double d) => 2.0 * Math.Log(d));
			ElectricForce = ((double d) => 1.0 / (d * d));
			ExternalForce = null;
			TemperatureAttenuation = 0.95;
			Initialize(initialPositions, seed);
		}

		public void Initialize(Func<Node, PointD> initialPositions = null, int seed = -1)
		{
			if (initialPositions == null)
			{
				Random r;
				if (seed == -1)
				{
					r = new Random();
				}
				else
				{
					r = new Random(seed);
				}
				initialPositions = ((Node node) => new PointD(r.NextDouble(), r.NextDouble()));
			}
			foreach (Node item in Graph.Nodes())
			{
				NodePositions[item] = initialPositions(item);
			}
			Temperature = 0.2;
		}

		public void Step()
		{
			Dictionary<Node, PointD> dictionary = new Dictionary<Node, PointD>();
			foreach (Node item in Graph.Nodes())
			{
				PointD arg = NodePositions[item];
				double num = 0.0;
				double num2 = 0.0;
				foreach (Arc item2 in Graph.Arcs(item, ArcFilter.All))
				{
					PointD other = NodePositions[Graph.Other(item2, item)];
					double num3 = arg.Distance(other);
					double num4 = Temperature * SpringForce(num3);
					num += (other.X - arg.X) / num3 * num4;
					num2 += (other.Y - arg.Y) / num3 * num4;
				}
				foreach (Node item3 in Graph.Nodes())
				{
					if (!(item3 == item))
					{
						PointD other2 = NodePositions[item3];
						double num5 = arg.Distance(other2);
						double num6 = Temperature * ElectricForce(num5);
						num += (arg.X - other2.X) / num5 * num6;
						num2 += (arg.Y - other2.Y) / num5 * num6;
					}
				}
				if (ExternalForce != null)
				{
					PointD pointD = ExternalForce(arg);
					num += Temperature * pointD.X;
					num2 += Temperature * pointD.Y;
				}
				dictionary[item] = new PointD(num, num2);
			}
			foreach (Node item4 in Graph.Nodes())
			{
				Dictionary<Node, PointD> nodePositions;
				Node key;
				(nodePositions = NodePositions)[key = item4] = nodePositions[key] + dictionary[item4];
			}
			Temperature *= TemperatureAttenuation;
		}

		public void Run(double minimumTemperature = 0.01)
		{
			while (Temperature > minimumTemperature)
			{
				Step();
			}
		}
	}
}
