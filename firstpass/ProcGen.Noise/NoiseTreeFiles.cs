using Klei;
using LibNoiseDotNet.Graphics.Tools.Noise;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProcGen.Noise
{
	public class NoiseTreeFiles : YamlIO<NoiseTreeFiles>
	{
		public static string NOISE_FILE = "noise";

		private Dictionary<string, Tree> trees;

		public List<string> tree_files
		{
			get;
			set;
		}

		public NoiseTreeFiles()
		{
			trees = new Dictionary<string, Tree>();
			tree_files = new List<string>();
		}

		public static string GetPath()
		{
			return System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/" + NOISE_FILE + ".yaml");
		}

		public static string GetTreeFilePath(string filename)
		{
			return System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/noise/" + filename + ".yaml");
		}

		public void LoadAllTrees()
		{
			for (int i = 0; i < tree_files.Count; i++)
			{
				Tree tree = YamlIO<Tree>.LoadFile(GetTreeFilePath(tree_files[i]), null);
				if (tree != null)
				{
					trees.Add(tree_files[i], tree);
				}
			}
		}

		public Tree LoadTree(string name, string path)
		{
			if (name != null && name.Length > 0)
			{
				if (!trees.ContainsKey(name))
				{
					Tree tree = YamlIO<Tree>.LoadFile(path + name + ".yaml", null);
					if (tree != null)
					{
						trees.Add(name, tree);
					}
				}
				return trees[name];
			}
			return null;
		}

		public float GetZoomForTree(string name)
		{
			if (trees.ContainsKey(name))
			{
				return trees[name].settings.zoom;
			}
			return 1f;
		}

		public bool ShouldNormaliseTree(string name)
		{
			if (trees.ContainsKey(name))
			{
				return trees[name].settings.normalise;
			}
			return false;
		}

		public string[] GetTreeNames()
		{
			string[] array = new string[trees.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Tree> tree in trees)
			{
				array[num++] = tree.Key;
			}
			return array;
		}

		public Tree GetTree(string name, string path)
		{
			if (!trees.ContainsKey(name))
			{
				Tree tree = YamlIO<Tree>.LoadFile(path + "/" + name + ".yaml", null);
				if (tree == null)
				{
					return null;
				}
				trees.Add(name, tree);
			}
			return trees[name];
		}

		public Tree GetTree(string name)
		{
			if (trees.ContainsKey(name))
			{
				return trees[name];
			}
			return null;
		}

		public IModule3D BuildTree(string name, int globalSeed)
		{
			if (trees.ContainsKey(name))
			{
				return trees[name].BuildFinalModule(globalSeed);
			}
			return null;
		}
	}
}
