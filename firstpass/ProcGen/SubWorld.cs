using KSerialization.Converters;
using System.Collections.Generic;
using VoronoiTree;

namespace ProcGen
{
	public class SubWorld : SampleDescriber
	{
		public enum ZoneType
		{
			FrozenWastes,
			CrystalCaverns,
			BoggyMarsh,
			Sandstone,
			ToxicJungle,
			MagmaCore,
			OilField,
			Space
		}

		public float pdWeight;

		public string biomeNoise
		{
			get;
			protected set;
		}

		public string overrideNoise
		{
			get;
			protected set;
		}

		public string densityNoise
		{
			get;
			protected set;
		}

		[StringEnumConverter]
		public Temperature.Range temperatureRange
		{
			get;
			protected set;
		}

		public Feature centralFeature
		{
			get;
			protected set;
		}

		public List<Feature> features
		{
			get;
			protected set;
		}

		public Override overrides
		{
			get;
			protected set;
		}

		public List<string> tags
		{
			get;
			protected set;
		}

		public int minChildCount
		{
			get;
			protected set;
		}

		public List<WeightedBiome> biomes
		{
			get;
			protected set;
		}

		public Dictionary<string, string[]> pointsOfInterest
		{
			get;
			protected set;
		}

		public Dictionary<string, int> featureTemplates
		{
			get;
			protected set;
		}

		public int iterations
		{
			get;
			protected set;
		}

		public float minEnergy
		{
			get;
			protected set;
		}

		public ZoneType zoneType
		{
			get;
			private set;
		}

		public List<SampleDescriber> samplers
		{
			get;
			private set;
		}

		public SubWorld()
		{
			minChildCount = 2;
			features = new List<Feature>();
			tags = new List<string>();
			biomes = new List<WeightedBiome>();
			samplers = new List<SampleDescriber>();
			featureTemplates = new Dictionary<string, int>();
		}

		public Node AddCenteralFeature(Tree node, Graph graph, TagSet newTags)
		{
			if (centralFeature != null)
			{
				Node node2 = graph.AddNode(centralFeature.type);
				node2.SetPosition(node.site.poly.Centroid());
				VoronoiTree.Node node3 = node.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiTree.Node.NodeType.Internal);
				node3.tags = new TagSet(newTags);
				node3.AddTag(new Tag(centralFeature.type));
				node3.AddTag(WorldGenTags.Feature);
				node3.AddTag(WorldGenTags.CenteralFeature);
				for (int i = 0; i < centralFeature.tags.Count; i++)
				{
					node3.AddTag(new Tag(centralFeature.tags[i]));
				}
				return node2;
			}
			return null;
		}

		public void GenerateStartArea(Tree node, Graph graph)
		{
		}
	}
}
