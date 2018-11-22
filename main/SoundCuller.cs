using UnityEngine;

public struct SoundCuller
{
	public class Tuning : TuningData<Tuning>
	{
		public float extraYRange;
	}

	private Vector2 min;

	private Vector2 max;

	private Vector2 cameraPos;

	private float zoomScaler;

	public bool IsAudible(Vector2 pos)
	{
		return min.LessEqual(pos) && pos.LessEqual(max);
	}

	public bool IsAudibleNoCameraScaling(Vector2 pos, float falloff_distance_sq)
	{
		float num = (pos.x - cameraPos.x) * (pos.x - cameraPos.x) + (pos.y - cameraPos.y) * (pos.y - cameraPos.y);
		return num < falloff_distance_sq;
	}

	public bool IsAudible(Vector2 pos, float falloff_distance_sq)
	{
		pos = GetVerticallyScaledPosition(pos);
		return IsAudibleNoCameraScaling(pos, falloff_distance_sq);
	}

	public bool IsAudible(Vector2 pos, string sound_path)
	{
		if (!string.IsNullOrEmpty(sound_path))
		{
			SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(sound_path);
			return IsAudible(pos, soundEventDescription.falloffDistanceSq);
		}
		return false;
	}

	public Vector3 GetVerticallyScaledPosition(Vector2 pos)
	{
		float num = 0f;
		float num2 = 1f;
		if (pos.y > max.y)
		{
			num = Mathf.Abs(pos.y - max.y);
		}
		else if (pos.y < min.y)
		{
			num = Mathf.Abs(pos.y - min.y);
			num2 = -1f;
		}
		else
		{
			num = 0f;
		}
		float extraYRange = TuningData<Tuning>.Get().extraYRange;
		num = ((!(num < extraYRange)) ? extraYRange : num);
		float num3 = num * num / (4f * zoomScaler);
		num3 *= num2;
		return new Vector3(pos.x, pos.y + num3, 0f);
	}

	public static SoundCuller CreateCuller()
	{
		SoundCuller result = default(SoundCuller);
		Camera main = Camera.main;
		Camera camera = main;
		Vector3 position = Camera.main.transform.GetPosition();
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(1f, 1f, position.z));
		Camera camera2 = main;
		Vector3 position2 = Camera.main.transform.GetPosition();
		Vector3 vector2 = camera2.ViewportToWorldPoint(new Vector3(0f, 0f, position2.z));
		result.min = new Vector3(vector2.x, vector2.y, 0f);
		result.max = new Vector3(vector.x, vector.y, 0f);
		result.cameraPos = main.transform.GetPosition();
		Audio audio = Audio.Get();
		float orthographicSize = CameraController.Instance.cameras[0].orthographicSize;
		float num = orthographicSize / (audio.listenerReferenceZ - audio.listenerMinZ);
		num = (result.zoomScaler = ((!(num <= 0f)) ? 1f : 2f));
		return result;
	}
}
