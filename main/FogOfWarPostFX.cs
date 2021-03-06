using UnityEngine;

public class FogOfWarPostFX : MonoBehaviour
{
	[SerializeField]
	private Shader shader;

	private Material material;

	private Camera myCamera;

	private void Awake()
	{
		base.enabled = SystemInfo.supportsImageEffects;
		if ((Object)shader != (Object)null)
		{
			material = new Material(shader);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		SetupUVs();
		Graphics.Blit(source, destination, material, 0);
	}

	private void SetupUVs()
	{
		if ((Object)myCamera == (Object)null)
		{
			myCamera = GetComponent<Camera>();
			if ((Object)myCamera == (Object)null)
			{
				return;
			}
		}
		Ray ray = myCamera.ViewportPointToRay(Vector3.zero);
		Vector3 origin = ray.origin;
		float z = origin.z;
		Vector3 direction = ray.direction;
		float distance = Mathf.Abs(z / direction.z);
		Vector3 point = ray.GetPoint(distance);
		Vector4 value = default(Vector4);
		value.x = point.x / Grid.WidthInMeters;
		value.y = point.y / Grid.HeightInMeters;
		ray = myCamera.ViewportPointToRay(Vector3.one);
		Vector3 origin2 = ray.origin;
		float z2 = origin2.z;
		Vector3 direction2 = ray.direction;
		distance = Mathf.Abs(z2 / direction2.z);
		point = ray.GetPoint(distance);
		value.z = point.x / Grid.WidthInMeters - value.x;
		value.w = point.y / Grid.HeightInMeters - value.y;
		material.SetVector("_UVOffsetScale", value);
	}
}
