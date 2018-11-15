using UnityEngine;

public class NativeAnimBatchLoader : MonoBehaviour
{
	public bool performTimeUpdate;

	public bool performUpdate;

	public bool performRender;

	public bool setTimeScale;

	public bool destroySelf;

	public bool generateObjects;

	public GameObject[] enableObjects;

	private void Awake()
	{
		KAnimBatchManager.DestroyInstance();
		KAnimGroupFile.DestroyInstance();
		KGlobalAnimParser.DestroyInstance();
		KAnimBatchManager.CreateInstance();
		KGlobalAnimParser.CreateInstance();
		KAnimGroupFile.GetGroupFile().LoadAll();
		KAnimBatchManager.Instance().CompleteInit();
	}

	private void Start()
	{
		if (generateObjects)
		{
			for (int i = 0; i < enableObjects.Length; i++)
			{
				if ((Object)enableObjects[i] != (Object)null)
				{
					enableObjects[i].GetComponent<KBatchedAnimController>().visibilityType = KAnimControllerBase.VisibilityType.Always;
					enableObjects[i].SetActive(true);
				}
			}
		}
		if (setTimeScale)
		{
			Time.timeScale = 1f;
		}
		if (destroySelf)
		{
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		if (!destroySelf)
		{
			if (performUpdate)
			{
				KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
				KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
			}
			if (performRender)
			{
				KAnimBatchManager.Instance().Render();
			}
		}
	}
}
