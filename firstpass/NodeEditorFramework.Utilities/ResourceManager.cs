using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class ResourceManager
	{
		public class MemoryTexture
		{
			public string path;

			public Texture2D texture;

			public string[] modifications;

			public MemoryTexture(string texPath, Texture2D tex, params string[] mods)
			{
				path = texPath;
				texture = tex;
				modifications = mods;
			}
		}

		private static List<MemoryTexture> loadedTextures = new List<MemoryTexture>();

		public static void SetDefaultResourcePath(string defaultResourcePath)
		{
		}

		public static string PreparePath(string path)
		{
			path = path.Replace(Application.dataPath, "Assets");
			if (path.Contains("Resources"))
			{
				path = path.Substring(path.LastIndexOf("Resources") + 10);
			}
			return path.Substring(0, path.LastIndexOf('.'));
		}

		public static T[] LoadResources<T>(string path) where T : UnityEngine.Object
		{
			path = PreparePath(path);
			throw new NotImplementedException("Currently it is not possible to load subAssets at runtime!");
		}

		public static T LoadResource<T>(string path) where T : UnityEngine.Object
		{
			path = PreparePath(path);
			return Resources.Load<T>(path);
		}

		public static Texture2D LoadTexture(string texPath)
		{
			if (!string.IsNullOrEmpty(texPath))
			{
				int num = loadedTextures.FindIndex((MemoryTexture memTex) => memTex.path == texPath);
				if (num != -1)
				{
					if (!((UnityEngine.Object)loadedTextures[num].texture == (UnityEngine.Object)null))
					{
						return loadedTextures[num].texture;
					}
					loadedTextures.RemoveAt(num);
				}
				Texture2D texture2D = LoadResource<Texture2D>(texPath);
				AddTextureToMemory(texPath, texture2D);
				return texture2D;
			}
			return null;
		}

		public static Texture2D GetTintedTexture(string texPath, Color col)
		{
			string text = "Tint:" + col.ToString();
			Texture2D texture2D = GetTexture(texPath, text);
			if ((UnityEngine.Object)texture2D == (UnityEngine.Object)null)
			{
				texture2D = LoadTexture(texPath);
				AddTextureToMemory(texPath, texture2D);
				texture2D = RTEditorGUI.Tint(texture2D, col);
				AddTextureToMemory(texPath, texture2D, text);
			}
			return texture2D;
		}

		public static void AddTextureToMemory(string texturePath, Texture2D texture, params string[] modifications)
		{
			if (!((UnityEngine.Object)texture == (UnityEngine.Object)null))
			{
				loadedTextures.Add(new MemoryTexture(texturePath, texture, modifications));
			}
		}

		public static MemoryTexture FindInMemory(Texture2D tex)
		{
			int num = loadedTextures.FindIndex((MemoryTexture memTex) => (UnityEngine.Object)memTex.texture == (UnityEngine.Object)tex);
			return (num == -1) ? null : loadedTextures[num];
		}

		public static bool HasInMemory(string texturePath, params string[] modifications)
		{
			int num = loadedTextures.FindIndex((MemoryTexture memTex) => memTex.path == texturePath);
			return num != -1 && EqualModifications(loadedTextures[num].modifications, modifications);
		}

		public static MemoryTexture GetMemoryTexture(string texturePath, params string[] modifications)
		{
			List<MemoryTexture> list = loadedTextures.FindAll((MemoryTexture memTex) => memTex.path == texturePath);
			if (list != null && list.Count != 0)
			{
				foreach (MemoryTexture item in list)
				{
					if (EqualModifications(item.modifications, modifications))
					{
						return item;
					}
				}
				return null;
			}
			return null;
		}

		public static Texture2D GetTexture(string texturePath, params string[] modifications)
		{
			return GetMemoryTexture(texturePath, modifications)?.texture;
		}

		private static bool EqualModifications(string[] modsA, string[] modsB)
		{
			return modsA.Length == modsB.Length && Array.TrueForAll(modsA, (string mod) => modsB.Count((string oMod) => mod == oMod) == modsA.Count((string oMod) => mod == oMod));
		}
	}
}
