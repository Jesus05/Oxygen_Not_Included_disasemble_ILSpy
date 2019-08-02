using System;
using UnityEngine;
using UnityEngine.UI;

public class UICurvePath : KMonoBehaviour
{
	public Transform startPoint;

	public Transform endPoint;

	public Transform controlPointStart;

	public Transform controlPointEnd;

	public Image sprite;

	public bool loop = true;

	public bool animateScale;

	public Vector3 initialScale;

	private float startDelay;

	public float initialAlpha = 0.5f;

	public float moveSpeed = 0.1f;

	private float tick;

	private Vector3 A;

	private Vector3 B;

	private Vector3 C;

	private Vector3 D;

	protected override void OnSpawn()
	{
		Init();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		OnResize();
		startDelay = (float)UnityEngine.Random.Range(0, 8);
	}

	private void OnResize()
	{
		A = startPoint.position;
		B = controlPointStart.position;
		C = controlPointEnd.position;
		D = endPoint.position;
	}

	protected override void OnCleanUp()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
		base.OnCleanUp();
	}

	private void Update()
	{
		startDelay -= Time.unscaledDeltaTime;
		sprite.gameObject.SetActive(startDelay < 0f);
		if (!(startDelay > 0f))
		{
			tick += Time.unscaledDeltaTime * moveSpeed;
			sprite.transform.position = DeCasteljausAlgorithm(tick);
			Image img = sprite;
			Color color = sprite.color;
			img.SetAlpha(Mathf.Min(color.a + tick / 2f, 1f));
			if (animateScale)
			{
				Vector3 localScale = sprite.transform.localScale;
				float num = Mathf.Min(localScale.x + Time.unscaledDeltaTime * moveSpeed, 1f);
				sprite.transform.localScale = new Vector3(num, num, 1f);
			}
			if (loop && tick > 1f)
			{
				Init();
			}
		}
	}

	private void Init()
	{
		sprite.transform.position = startPoint.position;
		tick = 0f;
		if (animateScale)
		{
			sprite.transform.localScale = initialScale;
		}
		sprite.SetAlpha(initialAlpha);
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			A = startPoint.position;
			B = controlPointStart.position;
			C = controlPointEnd.position;
			D = endPoint.position;
		}
		Gizmos.color = Color.white;
		Vector3 a = A;
		float num = 0.02f;
		int num2 = Mathf.FloorToInt(1f / num);
		for (int i = 1; i <= num2; i++)
		{
			float t = (float)i * num;
			Vector3 vector = DeCasteljausAlgorithm(t);
			a = vector;
		}
		Gizmos.color = Color.green;
	}

	private Vector3 DeCasteljausAlgorithm(float t)
	{
		float d = 1f - t;
		Vector3 a = d * A + t * B;
		Vector3 a2 = d * B + t * C;
		Vector3 a3 = d * C + t * D;
		Vector3 a4 = d * a + t * a2;
		Vector3 a5 = d * a2 + t * a3;
		return d * a4 + t * a5;
	}
}
