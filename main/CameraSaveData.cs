using UnityEngine;

public static class CameraSaveData
{
	public static bool valid;

	public static Vector3 position;

	public static Vector3 localScale;

	public static Quaternion rotation;

	public static float orthographicsSize;

	public static void Load(FastReader reader)
	{
		position = reader.ReadVector3();
		localScale = reader.ReadVector3();
		rotation = reader.ReadQuaternion();
		orthographicsSize = reader.ReadSingle();
		valid = true;
	}
}
