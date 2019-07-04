using FMOD;
using FMOD.Studio;
using System;
using System.IO;
using UnityEngine;

namespace FMODUnity
{
	public static class RuntimeUtils
	{
		public const string LogFileName = "fmod.log";

		private const string BankExtension = ".bank";

		public static VECTOR ToFMODVector(this Vector3 vec)
		{
			VECTOR result = default(VECTOR);
			result.x = vec.x;
			result.y = vec.y;
			result.z = vec.z;
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(this Vector3 pos)
		{
			ATTRIBUTES_3D result = default(ATTRIBUTES_3D);
			result.forward = Vector3.forward.ToFMODVector();
			result.up = Vector3.up.ToFMODVector();
			result.position = pos.ToFMODVector();
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(this Transform transform)
		{
			ATTRIBUTES_3D result = default(ATTRIBUTES_3D);
			result.forward = transform.forward.ToFMODVector();
			result.up = transform.up.ToFMODVector();
			result.position = transform.position.ToFMODVector();
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody rigidbody = null)
		{
			ATTRIBUTES_3D result = transform.To3DAttributes();
			if ((bool)rigidbody)
			{
				result.velocity = rigidbody.velocity.ToFMODVector();
			}
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody rigidbody = null)
		{
			ATTRIBUTES_3D result = go.transform.To3DAttributes();
			if ((bool)rigidbody)
			{
				result.velocity = rigidbody.velocity.ToFMODVector();
			}
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody2D rigidbody)
		{
			ATTRIBUTES_3D result = transform.To3DAttributes();
			if ((bool)rigidbody)
			{
				Vector2 velocity = rigidbody.velocity;
				VECTOR velocity2 = default(VECTOR);
				velocity2.x = velocity.x;
				Vector2 velocity3 = rigidbody.velocity;
				velocity2.y = velocity3.y;
				velocity2.z = 0f;
				result.velocity = velocity2;
			}
			return result;
		}

		public static ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody2D rigidbody)
		{
			ATTRIBUTES_3D result = go.transform.To3DAttributes();
			if ((bool)rigidbody)
			{
				Vector2 velocity = rigidbody.velocity;
				VECTOR velocity2 = default(VECTOR);
				velocity2.x = velocity.x;
				Vector2 velocity3 = rigidbody.velocity;
				velocity2.y = velocity3.y;
				velocity2.z = 0f;
				result.velocity = velocity2;
			}
			return result;
		}

		internal static FMODPlatform GetCurrentPlatform()
		{
			return FMODPlatform.Windows;
		}

		internal static string GetBankPath(string bankName)
		{
			string streamingAssetsPath = Application.streamingAssetsPath;
			if (!(Path.GetExtension(bankName) != ".bank"))
			{
				return $"{streamingAssetsPath}/{bankName}";
			}
			return $"{streamingAssetsPath}/{bankName}.bank";
		}

		internal static string GetPluginPath(string pluginName)
		{
			string str = pluginName + ".dll";
			string str2 = Application.dataPath + "/Plugins/";
			return str2 + str;
		}

		public static void EnforceLibraryOrder()
		{
			Memory.GetStats(out int _, out int _);
			FMOD.Studio.Util.ParseID("", out Guid _);
		}
	}
}
