using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Timelapser : KMonoBehaviour
{
	private bool screenshotActive;

	private bool screenshotPending;

	private bool previewScreenshot;

	private string previewSaveGamePath = string.Empty;

	private bool screenshotToday = true;

	private HashedString activeOverlay;

	private Camera freezeCamera;

	private RenderTexture bufferRenderTexture;

	private Vector3 camPosition;

	private float camSize;

	private bool debugScreenShot;

	private Vector2Int previewScreenshotResolution = new Vector2Int(Grid.WidthInCells * 2, Grid.HeightInCells * 2);

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

	private bool timelapseUserEnabled
	{
		get
		{
			Vector2I timelapseResolution = SaveGame.Instance.TimelapseResolution;
			return timelapseResolution.x > 0;
		}
	}

	protected override void OnPrefabInit()
	{
		RefreshRenderTextureSize(null);
		Game.Instance.Subscribe(75424175, RefreshRenderTextureSize);
		freezeCamera = CameraController.Instance.timelapseFreezeCamera;
		if (CycleTimeToScreenshot() > 0f)
		{
			OnNewDay(null);
		}
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
		if (previewScreenshot)
		{
			bufferRenderTexture = new RenderTexture(previewScreenshotResolution.x, previewScreenshotResolution.y, 32, RenderTextureFormat.ARGB32);
		}
		else if (timelapseUserEnabled)
		{
			Vector2I timelapseResolution = SaveGame.Instance.TimelapseResolution;
			int x = timelapseResolution.x;
			Vector2I timelapseResolution2 = SaveGame.Instance.TimelapseResolution;
			bufferRenderTexture = new RenderTexture(x, timelapseResolution2.y, 32, RenderTextureFormat.ARGB32);
		}
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
		if (screenshotToday && CycleTimeToScreenshot() <= 0f)
		{
			if (!timelapseUserEnabled)
			{
				screenshotToday = false;
			}
			else if (!PlayerController.Instance.IsDragging())
			{
				CameraController.Instance.ForcePanningState(false);
				screenshotToday = false;
				SaveScreenshot();
			}
		}
	}

	private float CycleTimeToScreenshot()
	{
		return 300f - GameClock.Instance.GetTime() % 600f;
	}

	private IEnumerator Render()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		yield return (object)wait;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void SaveScreenshot()
	{
		screenshotPending = true;
	}

	public void SaveColonyPreview(string saveFileName)
	{
		previewSaveGamePath = saveFileName;
		previewScreenshot = true;
		SaveScreenshot();
	}

	private void SetPostionAndOrtho()
	{
		float num = 0f;
		GameObject telepad = GameUtil.GetTelepad();
		if (!((UnityEngine.Object)telepad == (UnityEngine.Object)null))
		{
			Vector3 position = telepad.transform.GetPosition();
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				Vector3 position2 = item.transform.GetPosition();
				float num2 = (float)bufferRenderTexture.width / (float)bufferRenderTexture.height;
				Vector3 vector = position - position2;
				num = Mathf.Max(num, vector.x / num2, vector.y);
			}
			num += 10f;
			num = Mathf.Max(num, 18f);
			Camera overlayCamera = CameraController.Instance.overlayCamera;
			camSize = overlayCamera.orthographicSize;
			CameraController.Instance.SetOrthographicsSize(num);
			camPosition = CameraController.Instance.transform.position;
			CameraController instance = CameraController.Instance;
			Vector3 position3 = telepad.transform.position;
			float x = position3.x;
			Vector3 position4 = telepad.transform.position;
			float y = position4.y;
			Vector3 position5 = CameraController.Instance.transform.position;
			instance.SetPosition(new Vector3(x, y, position5.z));
			CameraController instance2 = CameraController.Instance;
			Vector3 position6 = telepad.transform.position;
			float x2 = position6.x;
			Vector3 position7 = telepad.transform.position;
			float y2 = position7.y;
			Vector3 position8 = CameraController.Instance.transform.position;
			instance2.SetTargetPos(new Vector3(x2, y2, position8.z), camSize, false);
		}
	}

	private void RenderAndPrint()
	{
		GameObject telepad = GameUtil.GetTelepad();
		if ((UnityEngine.Object)telepad == (UnityEngine.Object)null)
		{
			Debug.Log("No telepad present, aborting screenshot.");
		}
		else
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
		if (!previewScreenshot)
		{
			string text2 = Path.Combine(text, path);
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text2, path);
			DebugUtil.LogArgs("Saving screenshot to", text3);
			string format = "0000.##";
			text3 = text3 + "_cycle_" + GameClock.Instance.GetCycle().ToString(format);
			if (debugScreenShot)
			{
				string text4 = text3;
				text3 = text4 + "_" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
			}
			File.WriteAllBytes(text3 + ".png", bytes);
		}
		else
		{
			string path2 = previewSaveGamePath;
			path2 = Path.ChangeExtension(path2, ".png");
			DebugUtil.LogArgs("Saving screenshot to", path2);
			File.WriteAllBytes(path2, bytes);
		}
	}
}
