using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;

public static class Util
{
	private static HashSet<char> defaultInvalidUserInputChars = new HashSet<char>(Path.GetInvalidPathChars());

	private static HashSet<char> additionalInvalidUserInputChars = new HashSet<char>(new char[9]
	{
		'<',
		'>',
		':',
		'"',
		'/',
		'?',
		'*',
		'\\',
		'!'
	});

	private static System.Random random = new System.Random();

	private static string defaultRootFolder = Application.persistentDataPath;

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static void InitializeComponent(Component cmp)
	{
		if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
		{
			KMonoBehaviour kMonoBehaviour = cmp as KMonoBehaviour;
			if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
			{
				kMonoBehaviour.InitializeComponent();
			}
		}
	}

	public static void SpawnComponent(Component cmp)
	{
		if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
		{
			KMonoBehaviour kMonoBehaviour = cmp as KMonoBehaviour;
			if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
			{
				kMonoBehaviour.Spawn();
			}
		}
	}

	public static Component FindComponent(this Component cmp, string targetName)
	{
		return cmp.gameObject.FindComponent(targetName);
	}

	public static Component FindComponent(this GameObject go, string targetName)
	{
		Component component = go.GetComponent(targetName);
		InitializeComponent(component);
		return component;
	}

	public static T FindComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.FindComponent<T>();
	}

	public static T FindComponent<T>(this GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		InitializeComponent(component);
		return component;
	}

	public static T FindOrAddUnityComponent<T>(this Component cmp) where T : Component
	{
		return cmp.gameObject.FindOrAddUnityComponent<T>();
	}

	public static T FindOrAddUnityComponent<T>(this GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<T>();
		}
		return val;
	}

	public static Component RequireComponent(this Component cmp, string name)
	{
		return cmp.gameObject.RequireComponent(name);
	}

	public static Component RequireComponent(this GameObject go, string name)
	{
		Component component = go.GetComponent(name);
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogErrorFormat(go, "{0} '{1}' requires a component of type {2}!", go.GetType().ToString(), go.name, name);
			return null;
		}
		InitializeComponent(component);
		return component;
	}

	public static T RequireComponent<T>(this Component cmp) where T : Component
	{
		T component = cmp.gameObject.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogErrorFormat(cmp.gameObject, "{0} '{1}' requires a component of type {2} as requested by {3}!", cmp.gameObject.GetType().ToString(), cmp.gameObject.name, typeof(T).ToString(), cmp.GetType().ToString());
			return (T)null;
		}
		InitializeComponent(component);
		return component;
	}

	public static T RequireComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogErrorFormat(gameObject, "{0} '{1}' requires a component of type {2}!", gameObject.GetType().ToString(), gameObject.name, typeof(T).ToString());
			return (T)null;
		}
		InitializeComponent(component);
		return component;
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		SetLayer(go.transform, layer);
	}

	public static void SetLayer(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		for (int i = 0; i < t.childCount; i++)
		{
			SetLayer(t.GetChild(i), layer);
		}
	}

	public static void KDestroyGameObject(Component original)
	{
		KDestroyGameObject(original.gameObject);
	}

	public static void KDestroyGameObject(GameObject original)
	{
		original.DeleteObject();
	}

	public static T FindOrAddComponent<T>(this Component cmp) where T : Component
	{
		return cmp.gameObject.FindOrAddComponent<T>();
	}

	public static T FindOrAddComponent<T>(this GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<T>();
			KMonoBehaviour kMonoBehaviour = val as KMonoBehaviour;
			if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null && !KMonoBehaviour.isPoolPreInit && !kMonoBehaviour.IsInitialized())
			{
				Debug.LogErrorFormat("Could not find component " + typeof(T).ToString() + " on object " + go.ToString());
			}
		}
		else
		{
			InitializeComponent(val);
		}
		return val;
	}

	public static void PreInit(this GameObject go)
	{
		KMonoBehaviour.isPoolPreInit = true;
		KMonoBehaviour[] components = go.GetComponents<KMonoBehaviour>();
		foreach (KMonoBehaviour kMonoBehaviour in components)
		{
			kMonoBehaviour.InitializeComponent();
		}
		KMonoBehaviour.isPoolPreInit = false;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position)
	{
		return KInstantiate(original, position, Quaternion.identity, null, null, true, 0);
	}

	public static GameObject KInstantiate(Component original, GameObject parent = null, string name = null)
	{
		return KInstantiate(original.gameObject, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject original, GameObject parent = null, string name = null)
	{
		return KInstantiate(original, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Quaternion rotation, GameObject parent = null, string name = null, bool initialize_id = true, int gameLayer = 0)
	{
		if (App.IsExiting)
		{
			return null;
		}
		GameObject gameObject = null;
		if ((UnityEngine.Object)original == (UnityEngine.Object)null)
		{
			DebugUtil.LogWarningArgs("Missing prefab");
		}
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)original.GetComponent<RectTransform>() != (UnityEngine.Object)null && (UnityEngine.Object)parent != (UnityEngine.Object)null)
			{
				gameObject = UnityEngine.Object.Instantiate(original, position, rotation);
				gameObject.transform.SetParent(parent.transform, true);
			}
			else
			{
				Transform parent2 = null;
				if ((UnityEngine.Object)parent != (UnityEngine.Object)null)
				{
					parent2 = parent.transform;
				}
				gameObject = UnityEngine.Object.Instantiate(original, position, rotation, parent2);
			}
			if (gameLayer != 0)
			{
				gameObject.SetLayerRecursively(gameLayer);
			}
		}
		if (name != null)
		{
			gameObject.name = name;
		}
		else
		{
			gameObject.name = original.name;
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			if (initialize_id)
			{
				component.InstanceID = KPrefabID.GetUniqueID();
				KPrefabIDTracker.Get().Register(component);
			}
			KPrefabID component2 = original.GetComponent<KPrefabID>();
			component.CopyTags(component2);
			component.CopyInitFunctions(component2);
			component.RunInstantiateFn();
		}
		return gameObject;
	}

	public static T KInstantiateUI<T>(GameObject original, GameObject parent = null, bool force_active = false) where T : Component
	{
		GameObject gameObject = KInstantiateUI(original, parent, force_active);
		return gameObject.GetComponent<T>();
	}

	public static GameObject KInstantiateUI(GameObject original, GameObject parent = null, bool force_active = false)
	{
		if (App.IsExiting)
		{
			return null;
		}
		GameObject gameObject = null;
		if ((UnityEngine.Object)original == (UnityEngine.Object)null)
		{
			DebugUtil.LogWarningArgs("Missing prefab");
		}
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			gameObject = UnityEngine.Object.Instantiate(original, (!((UnityEngine.Object)parent != (UnityEngine.Object)null)) ? null : parent.transform, false);
		}
		gameObject.name = original.name;
		if (force_active)
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public static GameObject NewGameObject(GameObject parent, string name)
	{
		GameObject gameObject = new GameObject();
		if ((UnityEngine.Object)parent != (UnityEngine.Object)null)
		{
			gameObject.transform.parent = parent.transform;
		}
		if (name != null)
		{
			gameObject.name = name;
		}
		return gameObject;
	}

	public static T UpdateComponentRequirement<T>(this GameObject go, bool required = true) where T : Component
	{
		T val = go.GetComponent(typeof(T)) as T;
		if (!required && (UnityEngine.Object)val != (UnityEngine.Object)null)
		{
			UnityEngine.Object.DestroyImmediate(val, true);
			val = (T)null;
		}
		else if (required && (UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = (go.AddComponent(typeof(T)) as T);
		}
		return val;
	}

	public static string FormatTwoDecimalPlace(float value)
	{
		return $"{value:0.00}";
	}

	public static string FormatOneDecimalPlace(float value)
	{
		return $"{value:0.0}";
	}

	public static string FormatWholeNumber(float value)
	{
		return $"{value:0}";
	}

	public static bool IsInputCharacterValid(char _char, bool isPath = false)
	{
		if (defaultInvalidUserInputChars.Contains(_char))
		{
			return false;
		}
		if (!isPath && additionalInvalidUserInputChars.Contains(_char))
		{
			return false;
		}
		return true;
	}

	public static void ScrubInputField(TMP_InputField inputField, bool isPath = false)
	{
		for (int num = inputField.text.Length - 1; num >= 0; num--)
		{
			if (num < inputField.text.Length && !IsInputCharacterValid(inputField.text[num], isPath))
			{
				inputField.text = inputField.text.Remove(num, 1);
			}
		}
	}

	public static string StripTextFormatting(string original)
	{
		return Regex.Replace(original, "<[^>]*>([^<]*)<[^>]*>", "$1");
	}

	public static void Reset(Transform transform)
	{
		transform.SetLocalPosition(Vector3.zero);
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static float GaussianRandom(float mu = 0f, float sigma = 1f)
	{
		double num = random.NextDouble();
		double num2 = random.NextDouble();
		double num3 = (double)(Mathf.Sqrt(-2f * Mathf.Log((float)num)) * Mathf.Sin(6.28318548f * (float)num2));
		double num4 = (double)mu + (double)sigma * num3;
		return (float)num4;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		list.ShuffleSeeded(random);
	}

	public static Bounds GetBounds(GameObject go)
	{
		Bounds bounds = default(Bounds);
		bool first = true;
		GetBounds(go, ref bounds, ref first);
		return bounds;
	}

	private static void GetBounds(GameObject go, ref Bounds bounds, ref bool first)
	{
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				if (first)
				{
					bounds = component.bounds;
					first = false;
				}
				else
				{
					bounds.Encapsulate(component.bounds);
				}
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				Transform child = go.transform.GetChild(i);
				GetBounds(child.gameObject, ref bounds, ref first);
			}
		}
	}

	public static bool IsOnLeftSideOfScreen(Vector3 position)
	{
		return position.x < (float)Screen.width;
	}

	public static void Write(this BinaryWriter writer, Vector2 v)
	{
		writer.Write(v.x);
		writer.Write(v.y);
	}

	public static void Write(this BinaryWriter writer, Vector3 v)
	{
		writer.Write(v.x);
		writer.Write(v.y);
		writer.Write(v.z);
	}

	public static Vector2 ReadVector2(this BinaryReader reader)
	{
		Vector2 result = default(Vector2);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		return result;
	}

	public static Vector3 ReadVector3(this BinaryReader reader)
	{
		Vector3 result = default(Vector3);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		result.z = reader.ReadSingle();
		return result;
	}

	public static void Write(this BinaryWriter writer, Quaternion q)
	{
		writer.Write(q.x);
		writer.Write(q.y);
		writer.Write(q.z);
		writer.Write(q.w);
	}

	public static Quaternion ReadQuaternion(this BinaryReader reader)
	{
		Quaternion result = default(Quaternion);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		result.z = reader.ReadSingle();
		result.w = reader.ReadSingle();
		return result;
	}

	public static Color ColorFromHex(string hex)
	{
		int num = Convert.ToInt32(hex, 16);
		float r = 1f;
		float g = 1f;
		float b = 1f;
		float a = 1f;
		if (hex.Length == 6)
		{
			r = (float)((num >> 16) & 0xFF);
			r /= 255f;
			g = (float)((num >> 8) & 0xFF);
			g /= 255f;
			b = (float)(num & 0xFF);
			b /= 255f;
		}
		else if (hex.Length == 8)
		{
			r = (float)((num >> 24) & 0xFF);
			r /= 255f;
			g = (float)((num >> 16) & 0xFF);
			g /= 255f;
			b = (float)((num >> 8) & 0xFF);
			b /= 255f;
			a = (float)(num & 0xFF);
			a /= 255f;
		}
		return new Color(r, g, b, a);
	}

	public static string ToHexString(this Color c)
	{
		return $"{(int)(c.r * 255f):X2}{(int)(c.g * 255f):X2}{(int)(c.b * 255f):X2}{(int)(c.a * 255f):X2}";
	}

	public static void Signal(this System.Action action)
	{
		action?.Invoke();
	}

	public static void Signal<T>(this Action<T> action, T parameter)
	{
		action?.Invoke(parameter);
	}

	public static RectTransform rectTransform(this GameObject go)
	{
		return go.GetComponent<RectTransform>();
	}

	public static RectTransform rectTransform(this Component cmp)
	{
		return cmp.GetComponent<RectTransform>();
	}

	public static T[] Append<T>(this T[] array, T item)
	{
		T[] array2 = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		array2[array.Length] = item;
		return array2;
	}

	public static string GetKleiRootPath()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return Path.Combine(folderPath, "Klei");
		}
		return defaultRootFolder;
	}

	public static string GetTitleFolderName()
	{
		return "OxygenNotIncluded";
	}

	public static string GetRetiredColoniesFolderName()
	{
		return "RetiredColonies";
	}

	public static string RootFolder()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			return Path.Combine(GetKleiRootPath(), GetTitleFolderName());
		}
		return GetKleiRootPath();
	}

	public static string CacheFolder()
	{
		return Path.Combine(defaultRootFolder, "cache");
	}

	public static Transform FindTransformRecursive(Transform node, string name)
	{
		if (node.name == name)
		{
			return node;
		}
		for (int i = 0; i < node.childCount; i++)
		{
			Transform transform = FindTransformRecursive(node.GetChild(i), name);
			if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
			{
				return transform;
			}
		}
		return null;
	}

	public static Vector3 ReadVector3(this IReader reader)
	{
		Vector3 result = default(Vector3);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		result.z = reader.ReadSingle();
		return result;
	}

	public static Quaternion ReadQuaternion(this IReader reader)
	{
		Quaternion result = default(Quaternion);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		result.z = reader.ReadSingle();
		result.w = reader.ReadSingle();
		return result;
	}

	public static T GetRandom<T>(this T[] tArray)
	{
		return tArray[UnityEngine.Random.Range(0, tArray.Length)];
	}

	public static T GetRandom<T>(this List<T> tList)
	{
		return tList[UnityEngine.Random.Range(0, tList.Count)];
	}

	public static float RandomVariance(float center, float plusminus)
	{
		return center + UnityEngine.Random.Range(0f - plusminus, plusminus);
	}

	public static bool IsNullOrWhiteSpace(this string str)
	{
		return string.IsNullOrEmpty(str) || str == " ";
	}

	public static void ApplyInvariantCultureToThread(Thread thread)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			thread.CurrentCulture = CultureInfo.InvariantCulture;
		}
	}
}
