using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace KSerialization
{
	public static class IOHelper
	{
		public static void WriteKleiString(this BinaryWriter writer, string str)
		{
			if (str != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				writer.Write(bytes.Length);
				writer.Write(bytes);
			}
			else
			{
				writer.Write(-1);
			}
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void WriteBoundaryTag(this BinaryWriter writer, object tag)
		{
			writer.Write((uint)tag);
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void CheckBoundaryTag(this IReader reader, object expected)
		{
			uint num = reader.ReadUInt32();
			if ((uint)expected != num)
			{
				Output.LogError($"Expected Tag {expected.ToString()}(0x{(uint)expected:X}) but got 0x{num:X} instead");
			}
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void Assert(bool condition)
		{
			DebugUtil.Assert(condition, "Assert!", string.Empty, string.Empty);
		}

		public static Vector2I ReadVector2I(this IReader reader)
		{
			Vector2I result = default(Vector2I);
			result.x = reader.ReadInt32();
			result.y = reader.ReadInt32();
			return result;
		}

		public static Vector2 ReadVector2(this IReader reader)
		{
			Vector2 result = default(Vector2);
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			return result;
		}

		public static Vector3 ReadVector3(this IReader reader)
		{
			Vector3 result = default(Vector3);
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			return result;
		}

		public static Color ReadColour(this IReader reader)
		{
			byte b = reader.ReadByte();
			byte b2 = reader.ReadByte();
			byte b3 = reader.ReadByte();
			byte b4 = reader.ReadByte();
			Color result = default(Color);
			result.r = (float)(int)b / 255f;
			result.g = (float)(int)b2 / 255f;
			result.b = (float)(int)b3 / 255f;
			result.a = (float)(int)b4 / 255f;
			return result;
		}
	}
}
