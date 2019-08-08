using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Timelapser : KMonoBehaviour
{
	private bool screenshotActive;

	private bool screenshotPending;

	private bool screenshotToday;

	private Camera freezeCamera;

	private RenderTexture bufferRenderTexture;

	private Vector3 camPosition;

	private float camSize;

	private bool debugScreenShot;

	private const int DEFAULT_SCREENSHOT_INTERVAL = 10;

	private int[] timelapseScreenshotCycles = new int[100]
	{
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11,
		12,
		13,
		14,
		15,
		16,
		17,
		18,
		19,
		20,
		21,
		22,
		23,
		24,
		25,
		26,
		27,
		28,
		29,
		30,
		31,
		32,
		33,
		34,
		35,
		36,
		37,
		38,
		39,
		40,
		41,
		42,
		43,
		44,
		45,
		46,
		47,
		48,
		49,
		50,
		55,
		60,
		65,
		70,
		75,
		80,
		85,
		90,
		95,
		100,
		110,
		120,
		130,
		140,
		150,
		160,
		170,
		180,
		190,
		200,
		210,
		220,
		230,
		240,
		250,
		260,
		270,
		280,
		290,
		200,
		310,
		320,
		330,
		340,
		350,
		360,
		370,
		380,
		390,
		400,
		410,
		420,
		430,
		440,
		450,
		460,
		470,
		480,
		490,
		500
	};

	public bool CapturingTimelapseScreenshot => screenshotActive;

	public Texture2D freezeTexture
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		RefreshRenderTextureSize(null);
		Game.Instance.Subscribe(75424175, RefreshRenderTextureSize);
		freezeCamera = CameraController.Instance.timelapseFreezeCamera;
		GameClock.Instance.Subscribe(631075836, OnNewDay);
		OnResize();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		StartCoroutine(Render());
	}

	private void OnResize()
	{
		if ((UnityEngine.Object)freezeTexture != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(freezeTexture);
		}
		freezeTexture = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.ARGB32, false);
	}

	private void RefreshRenderTextureSize(object data = null)
	{
		bufferRenderTexture = new RenderTexture(SaveGame.Instance.timelapseResolution.x, SaveGame.Instance.timelapseResolution.y, 32, RenderTextureFormat.ARGB32);
	}

	private void OnNewDay(object data = null)
	{
		int cycle = GameClock.Instance.GetCycle();
		if (cycle > timelapseScreenshotCycles[timelapseScreenshotCycles.Length - 1])
		{
			if (cycle % 10 == 0)
			{
				screenshotToday = true;
			}
		}
		else
		{
			for (int i = 0; i < timelapseScreenshotCycles.Length; i++)
			{
				if (cycle == timelapseScreenshotCycles[i])
				{
					screenshotToday = true;
				}
			}
		}
	}

	private void Update()
	{
		if (screenshotToday && GameClock.Instance.GetTime() % 600f > 300f && !PlayerController.Instance.IsDragging())
		{
			CameraController.Instance.ForcePanningState(false);
			screenshotToday = false;
			SaveScreenshot();
		}
	}

	private IEnumerator Render()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		yield return (object)wait;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void SaveScreenshot()
	{
		Debug.Log("Screenshot!");
		screenshotPending = true;
	}

	private void SetPostionAndOrtho()
	{
		int a = 0;
		GameObject telepad = GameUtil.GetTelepad();
		if (!((UnityEngine.Object)telepad == (UnityEngine.Object)null))
		{
			int cell_b = Grid.PosToCell(telepad);
			for (int i = 0; i < Grid.CellCount; i++)
			{
				if (Grid.Revealed[i])
				{
					a = Mathf.Max(a, Grid.GetCellDistance(i, cell_b));
				}
			}
			a = Mathf.Max(a, 18);
			Camera overlayCamera = CameraController.Instance.overlayCamera;
			camSize = overlayCamera.orthographicSize;
			CameraController.Instance.SetOrthographicsSize((float)a);
			camPosition = CameraController.Instance.transform.position;
			CameraController instance = CameraController.Instance;
			Vector3 position = telepad.transform.position;
			float x = position.x;
			Vector3 position2 = telepad.transform.position;
			float y = position2.y;
			Vector3 position3 = CameraController.Instance.transform.position;
			instance.SetPosition(new Vector3(x, y, position3.z));
			CameraController instance2 = CameraController.Instance;
			Vector3 position4 = telepad.transform.position;
			float x2 = position4.x;
			Vector3 position5 = telepad.transform.position;
			float y2 = position5.y;
			Vector3 position6 = CameraController.Instance.transform.position;
			instance2.SetTargetPos(new Vector3(x2, y2, position6.z), camSize, false);
		}
	}

	private void RenderAndPrint()
	{
		GameObject telepad = GameUtil.GetTelepad();
		if (!((UnityEngine.Object)telepad == (UnityEngine.Object)null))
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = bufferRenderTexture;
			CameraController instance = CameraController.Instance;
			Vector3 position = telepad.transform.position;
			float x = position.x;
			Vector3 position2 = telepad.transform.position;
			float y = position2.y;
			Vector3 position3 = CameraController.Instance.transform.position;
			instance.SetPosition(new Vector3(x, y, position3.z));
			CameraController.Instance.RenderForTimelapser(ref bufferRenderTexture);
			WriteToPng(bufferRenderTexture);
			CameraController.Instance.SetOrthographicsSize(camSize);
			CameraController.Instance.SetPosition(camPosition);
			CameraController.Instance.SetTargetPos(camPosition, camSize, false);
			RenderTexture.active = active;
			screenshotActive = false;
			debugScreenShot = false;
		}
	}

	public void WriteToPng(RenderTexture renderTex)
	{
		Texture2D texture2D = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTex.width, (float)renderTex.height), 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		UnityEngine.Object.Destroy(texture2D);
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string path = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);
		string text2 = Path.Combine(text, path);
		Debug.Log(text2);
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		string str = Path.Combine(text2, path);
		string format = "0000.##";
		str = str + "_cycle_" + GameClock.Instance.GetCycle().ToString(format);
		if (debugScreenShot)
		{
			string text3 = str;
			str = text3 + "_" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
		}
		File.WriteAllBytes(str + ".png", bytes);
	}
}
