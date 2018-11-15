using UnityEngine;

namespace Klei
{
	public class MeshCreator
	{
		public static void MakePlane(GameObject target, int width, int height, bool hide = true)
		{
			int num = width * height * 2;
			Vector3[] array = new Vector3[num];
			Vector2[] array2 = new Vector2[num];
			int[] array3 = new int[num];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int num2 = j * width + i;
					float num3 = 0.5f + (float)i;
					float num4 = 0.5f + (float)j;
					array[num2] = new Vector3(num3, num4, 0f);
					array2[num2] = new Vector2(num3 / (float)width, num4 / (float)height);
					array3[num2] = num2;
				}
			}
			Mesh mesh = null;
			MeshFilter component = target.GetComponent<MeshFilter>();
			if ((Object)component.sharedMesh == (Object)null)
			{
				mesh = new Mesh();
				mesh.name = "Klei.MeshCreator.Plane";
				component.sharedMesh = mesh;
			}
			else
			{
				component.sharedMesh.Clear();
				mesh = component.sharedMesh;
			}
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.SetIndices(array3, MeshTopology.Points, 0);
			if (hide)
			{
				mesh.hideFlags = HideFlags.HideInHierarchy;
			}
			mesh.RecalculateBounds();
		}
	}
}
