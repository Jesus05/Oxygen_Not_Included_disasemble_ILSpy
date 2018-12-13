using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[DebuggerDisplay("{name} visible={visible} suspendUpdates={suspendUpdates} moving={moving}")]
public class KBatchedAnimController : KAnimControllerBase, KAnimConverter.IAnimConverter
{
	[NonSerialized]
	protected bool _forceRebuild;

	private Vector3 lastPos = Vector3.zero;

	private Vector2I lastChunkXY = KBatchedAnimUpdater.INVALID_CHUNK_ID;

	private KAnimBatch batch;

	public float animScale = 0.005f;

	private bool suspendUpdates;

	private bool visibilityListenerRegistered;

	private bool moving;

	private SymbolOverrideController symbolOverrideController;

	private int symbolOverrideControllerVersion;

	[NonSerialized]
	public KBatchedAnimUpdater.RegistrationState updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Unregistered;

	public Grid.SceneLayer sceneLayer;

	private RectTransform rt;

	private Vector3 screenOffset = new Vector3(0f, 0f, 0f);

	public Matrix2x3 navMatrix = Matrix2x3.identity;

	private CanvasScaler scaler;

	public bool setScaleFromAnim = true;

	public Vector2 animOverrideSize = Vector2.one;

	private Canvas rootCanvas;

	public bool isMovable;

	[CompilerGenerated]
	private static Action<Transform, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<Transform, bool> _003C_003Ef__mg_0024cache1;

	protected bool forceRebuild
	{
		get
		{
			return _forceRebuild;
		}
		set
		{
			_forceRebuild = value;
		}
	}

	public HashedString batchGroupID
	{
		get;
		private set;
	}

	public KBatchedAnimController()
	{
		batchInstanceData = new KBatchedAnimInstanceData(this);
	}

	public int GetCurrentFrameIndex()
	{
		return curAnimFrameIdx;
	}

	public KBatchedAnimInstanceData GetBatchInstanceData()
	{
		return batchInstanceData;
	}

	public bool IsActive()
	{
		return base.isActiveAndEnabled && _enabled;
	}

	public bool IsVisible()
	{
		return isVisible;
	}

	public void SetSymbolScale(KAnimHashedString symbol_name, float scale)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(GetBatchGroupID(false)).GetSymbol(symbol_name);
		if (symbol != null)
		{
			base.symbolInstanceGpuData.SetSymbolScale(symbol.symbolIndexInSourceBuild, scale);
			SuspendUpdates(false);
			SetDirty();
		}
	}

	public void SetSymbolTint(KAnimHashedString symbol_name, Color color)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(GetBatchGroupID(false)).GetSymbol(symbol_name);
		if (symbol != null)
		{
			base.symbolInstanceGpuData.SetSymbolTint(symbol.symbolIndexInSourceBuild, color);
			SuspendUpdates(false);
			SetDirty();
		}
	}

	public Vector2I GetCellXY()
	{
		Vector3 positionIncludingOffset = base.PositionIncludingOffset;
		if (Grid.CellSizeInMeters == 0f)
		{
			return new Vector2I((int)positionIncludingOffset.x, (int)positionIncludingOffset.y);
		}
		return Grid.PosToXY(positionIncludingOffset);
	}

	public float GetZ()
	{
		Vector3 position = base.transform.GetPosition();
		return position.z;
	}

	public string GetName()
	{
		return base.name;
	}

	public override KAnim.Anim GetAnim(int index)
	{
		if (!batchGroupID.IsValid || !(batchGroupID != KAnimBatchManager.NO_BATCH))
		{
			Debug.LogError(base.name + " batch not ready", null);
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(batchGroupID);
		return batchGroupData.GetAnim(index);
	}

	private void Initialize()
	{
		if (batchGroupID.IsValid && batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			DeRegister();
			Register();
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving != moving)
		{
			moving = is_moving;
			SetDirty();
			ConfigureUpdateListener();
		}
	}

	private static void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		KBatchedAnimController component = transform.GetComponent<KBatchedAnimController>();
		component.OnMovementStateChanged(is_moving);
	}

	private void SetBatchGroup(KAnimFileData kafd)
	{
		DebugUtil.Assert(!this.batchGroupID.IsValid, "Should only be setting the batch group once.");
		DebugUtil.Assert(kafd != null, "Null anim data!! For", base.name);
		base.curBuild = kafd.build;
		DebugUtil.Assert(base.curBuild != null, "Null build for anim!! ", base.name, kafd.name);
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(base.curBuild.batchTag);
		HashedString batchGroupID = kafd.build.batchTag;
		if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
		{
			batchGroupID = group.swapTarget;
		}
		this.batchGroupID = batchGroupID;
		base.symbolInstanceGpuData = new SymbolInstanceGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).maxSymbolsPerBuild);
		base.symbolOverrideInfoGpuData = new SymbolOverrideInfoGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).symbolFrameInstances.Count);
		if (!this.batchGroupID.IsValid || this.batchGroupID == KAnimBatchManager.NO_BATCH)
		{
			Debug.LogError("Batch is not ready: " + base.name, null);
		}
	}

	public void LoadAnims()
	{
		if (!KAnimBatchManager.Instance().isReady)
		{
			Debug.LogError("KAnimBatchManager is not ready when loading anim:" + base.name, null);
		}
		if (animFiles.Length <= 0)
		{
			DebugUtil.Assert(false, "KBatchedAnimController has no anim files:" + base.name);
		}
		if ((UnityEngine.Object)animFiles[0].buildFile == (UnityEngine.Object)null)
		{
			Output.LogErrorWithObj(base.gameObject, $"First anim file needs to be the build file but {animFiles[0].animFile.name} doesn't have an associated build");
		}
		overrideAnims.Clear();
		anims.Clear();
		SetBatchGroup(animFiles[0].GetData());
		for (int i = 0; i < animFiles.Length; i++)
		{
			AddAnims(animFiles[i]);
		}
		forceRebuild = true;
		if (layering != null)
		{
			layering.HideSymbols();
		}
		if (usingNewSymbolOverrideSystem)
		{
			DebugUtil.Assert((UnityEngine.Object)GetComponent<SymbolOverrideController>() != (UnityEngine.Object)null);
		}
	}

	public void UpdateAnim(float dt)
	{
		if (batch != null && base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			if (batch != null && batch.group.maxGroupSize == 1)
			{
				float z = lastPos.z;
				Vector3 position = base.transform.GetPosition();
				if (z != position.z)
				{
					KAnimBatch kAnimBatch = batch;
					Vector3 position2 = base.transform.GetPosition();
					kAnimBatch.OverrideZ(position2.z);
				}
			}
			Vector3 vector = lastPos = base.PositionIncludingOffset;
			if (visibilityType != VisibilityType.Always)
			{
				Vector2I u = KAnimBatchManager.ControllerToChunkXY(this);
				if (u != lastChunkXY && lastChunkXY != KBatchedAnimUpdater.INVALID_CHUNK_ID)
				{
					DeRegister();
					Register();
				}
			}
			SetDirty();
		}
		if (!(batchGroupID == KAnimBatchManager.NO_BATCH) && IsActive() && (isVisible || forceRebuild))
		{
			if (!forceRebuild && (mode == KAnim.PlayMode.Paused || stopped || curAnim == null || (mode == KAnim.PlayMode.Once && curAnim != null && (base.elapsedTime > curAnim.totalTime || curAnim.totalTime <= 0f) && animQueue.Count == 0)))
			{
				SuspendUpdates(true);
			}
			curAnimFrameIdx = GetFrameIdx(base.elapsedTime, true);
			if (eventManagerHandle.IsValid() && aem != null)
			{
				float elapsedTime = aem.GetElapsedTime(eventManagerHandle);
				if ((int)((base.elapsedTime - elapsedTime) * 100f) != 0)
				{
					UpdateAnimEventSequenceTime();
				}
			}
			UpdateFrame(base.elapsedTime);
			if (!stopped && mode != KAnim.PlayMode.Paused)
			{
				SetElapsedTime(base.elapsedTime + dt * playSpeed);
			}
			forceRebuild = false;
		}
	}

	protected override void UpdateFrame(float t)
	{
		base.previousFrame = base.currentFrame;
		if (!stopped || forceRebuild)
		{
			if (curAnim != null && (mode == KAnim.PlayMode.Loop || elapsedTime <= GetDuration() || forceRebuild))
			{
				base.currentFrame = curAnim.GetFrameIdx(mode, elapsedTime);
				if (base.currentFrame != base.previousFrame || forceRebuild)
				{
					SetDirty();
				}
			}
			else
			{
				TriggerStop();
			}
			if (!stopped && mode == KAnim.PlayMode.Loop && base.currentFrame == 0)
			{
				AnimEnter(curAnim.hash);
			}
		}
		if (synchronizer != null)
		{
			synchronizer.SyncTime();
		}
	}

	public override void TriggerStop()
	{
		if (animQueue.Count > 0)
		{
			StartQueuedAnim();
		}
		else if (curAnim != null && mode == KAnim.PlayMode.Once)
		{
			base.currentFrame = curAnim.numFrames - 1;
			Stop();
			base.gameObject.Trigger(-1061186183, null);
			if (destroyOnAnimComplete)
			{
				DestroySelf();
			}
		}
	}

	public override void UpdateHidden()
	{
		for (int i = 0; i < base.curBuild.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = base.curBuild.symbols[i];
			bool is_visible = !hiddenSymbols.Contains(symbol.hash);
			base.symbolInstanceGpuData.SetVisible(i, is_visible);
		}
		SetDirty();
	}

	public int GetMaxVisible()
	{
		return maxSymbols;
	}

	public HashedString GetBatchGroupID(bool isEditorWindow = false)
	{
		return batchGroupID;
	}

	public int GetLayer()
	{
		return base.gameObject.layer;
	}

	public KAnimBatch GetBatch()
	{
		return batch;
	}

	public void SetBatch(KAnimBatch new_batch)
	{
		batch = new_batch;
		if (materialType == KAnimBatchGroup.MaterialType.UI)
		{
			KBatchedAnimCanvasRenderer kBatchedAnimCanvasRenderer = GetComponent<KBatchedAnimCanvasRenderer>();
			if ((UnityEngine.Object)kBatchedAnimCanvasRenderer == (UnityEngine.Object)null && new_batch != null)
			{
				kBatchedAnimCanvasRenderer = base.gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
			}
			if ((UnityEngine.Object)kBatchedAnimCanvasRenderer != (UnityEngine.Object)null)
			{
				kBatchedAnimCanvasRenderer.SetBatch(this);
			}
		}
	}

	public int GetCurrentNumFrames()
	{
		return (curAnim != null) ? curAnim.numFrames : 0;
	}

	public int GetFirstFrameIndex()
	{
		return (curAnim == null) ? (-1) : curAnim.firstFrameIdx;
	}

	private Canvas GetRootCanvas()
	{
		Canvas canvas = null;
		if ((UnityEngine.Object)rt == (UnityEngine.Object)null)
		{
			return null;
		}
		RectTransform component = rt.parent.GetComponent<RectTransform>();
		while ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			canvas = component.GetComponent<Canvas>();
			if ((UnityEngine.Object)canvas != (UnityEngine.Object)null && canvas.isRootCanvas)
			{
				return canvas;
			}
			component = component.parent.GetComponent<RectTransform>();
		}
		return null;
	}

	public override Matrix2x3 GetTransformMatrix()
	{
		Vector3 v = base.PositionIncludingOffset;
		v.z = 0f;
		Vector2 scale = new Vector2(animScale * animWidth, (0f - animScale) * animHeight);
		if (materialType == KAnimBatchGroup.MaterialType.UI)
		{
			rt = GetComponent<RectTransform>();
			if ((UnityEngine.Object)rootCanvas == (UnityEngine.Object)null)
			{
				rootCanvas = GetRootCanvas();
			}
			if ((UnityEngine.Object)scaler == (UnityEngine.Object)null && (UnityEngine.Object)rootCanvas != (UnityEngine.Object)null)
			{
				scaler = rootCanvas.GetComponent<CanvasScaler>();
			}
			if ((UnityEngine.Object)rootCanvas == (UnityEngine.Object)null)
			{
				screenOffset.x = (float)(Screen.width / 2);
				screenOffset.y = (float)(Screen.height / 2);
			}
			else
			{
				screenOffset.x = rootCanvas.rectTransform().rect.width / 2f;
				screenOffset.y = rootCanvas.rectTransform().rect.height / 2f;
			}
			float d = 1f;
			if ((UnityEngine.Object)scaler != (UnityEngine.Object)null)
			{
				d = 1f / scaler.scaleFactor;
			}
			v = (rt.localToWorldMatrix.MultiplyPoint(rt.pivot) + offset) * d - screenOffset;
			float num = animWidth * animScale;
			float num2 = animHeight * animScale;
			if (setScaleFromAnim && curAnim != null)
			{
				float num3 = num;
				Vector2 size = rt.rect.size;
				num = num3 * (size.x / curAnim.unScaledSize.x);
				float num4 = num2;
				Vector2 size2 = rt.rect.size;
				num2 = num4 * (size2.y / curAnim.unScaledSize.y);
			}
			else
			{
				float num5 = num;
				Vector2 size3 = rt.rect.size;
				num = num5 * (size3.x / animOverrideSize.x);
				float num6 = num2;
				Vector2 size4 = rt.rect.size;
				num2 = num6 * (size4.y / animOverrideSize.y);
			}
			Vector3 lossyScale = rt.lossyScale;
			float x = lossyScale.x * num;
			Vector3 lossyScale2 = rt.lossyScale;
			float y = (0f - lossyScale2.y) * num2;
			Vector3 lossyScale3 = rt.lossyScale;
			scale = new Vector3(x, y, lossyScale3.z);
			pivot = rt.pivot;
		}
		Matrix2x3 n = Matrix2x3.Scale(scale);
		Matrix2x3 n2 = Matrix2x3.Scale(new Vector2((!flipX) ? 1f : (-1f), (!flipY) ? 1f : (-1f)));
		if (rotation == 0f)
		{
			Matrix2x3 m = Matrix2x3.TRS(v, base.transform.rotation, base.transform.localScale);
			return m * n * navMatrix * n2;
		}
		Matrix2x3 n3 = Matrix2x3.Translate(-pivot);
		Matrix2x3 n4 = Matrix2x3.Rotate(rotation * 0.0174532924f);
		Matrix2x3 m2 = Matrix2x3.Translate(pivot);
		Matrix2x3 n5 = m2 * n4 * n3;
		Matrix2x3 m3 = Matrix2x3.TRS(v, base.transform.rotation, base.transform.localScale);
		return m3 * n5 * n * navMatrix * n2;
	}

	public override Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		if (curAnimFrameIdx != -1 && batch != null)
		{
			Matrix2x3 symbolLocalTransform = GetSymbolLocalTransform(symbol, out symbolVisible);
			if (symbolVisible)
			{
				Matrix4x4 lhs = GetTransformMatrix();
				return lhs * (Matrix4x4)symbolLocalTransform;
			}
		}
		symbolVisible = false;
		return default(Matrix4x4);
	}

	public override Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible)
	{
		if (curAnimFrameIdx != -1 && batch != null)
		{
			KAnim.Anim.Frame frame = batch.group.data.GetFrame(curAnimFrameIdx);
			if (frame != KAnim.Anim.Frame.InvalidFrame)
			{
				for (int i = 0; i < frame.numElements; i++)
				{
					int num = frame.firstElementIdx + i;
					if (num < batch.group.data.frameElements.Count)
					{
						KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[num];
						if (frameElement.symbol == symbol)
						{
							symbolVisible = true;
							return frameElement.transform;
						}
					}
				}
			}
		}
		symbolVisible = false;
		return Matrix2x3.identity;
	}

	public override void SetLayer(int layer)
	{
		if (layer != base.gameObject.layer)
		{
			base.SetLayer(layer);
			DeRegister();
			base.gameObject.layer = layer;
			Register();
		}
	}

	public override void SetDirty()
	{
		if (batch != null)
		{
			batch.SetDirty(this);
		}
	}

	protected override void OnStartQueuedAnim()
	{
		SuspendUpdates(false);
	}

	protected override void OnAwake()
	{
		LoadAnims();
		if (visibilityType == VisibilityType.Default)
		{
			visibilityType = ((materialType == KAnimBatchGroup.MaterialType.UI) ? VisibilityType.Always : visibilityType);
		}
		symbolOverrideController = GetComponent<SymbolOverrideController>();
		UpdateHidden();
		hasEnableRun = false;
	}

	protected override void OnStart()
	{
		if (batch == null)
		{
			Initialize();
		}
		if (visibilityType == VisibilityType.Always)
		{
			ConfigureUpdateListener();
		}
		CellChangeMonitor instance = Singleton<CellChangeMonitor>.Instance;
		if (instance != null)
		{
			instance.RegisterMovementStateChanged(base.transform, OnMovementStateChanged);
			moving = instance.IsMoving(base.transform);
		}
		symbolOverrideController = GetComponent<SymbolOverrideController>();
		SetDirty();
	}

	protected override void OnStop()
	{
		SetDirty();
	}

	private void OnEnable()
	{
		if (_enabled)
		{
			Enable();
		}
	}

	protected override void Enable()
	{
		if (!hasEnableRun)
		{
			hasEnableRun = true;
			if (batch == null)
			{
				Initialize();
			}
			SetDirty();
			SuspendUpdates(false);
			ConfigureVisibilityListener(true);
			if (!stopped && curAnim != null && mode != KAnim.PlayMode.Paused && !eventManagerHandle.IsValid())
			{
				StartAnimEventSequence();
			}
		}
	}

	private void OnDisable()
	{
		Disable();
	}

	protected override void Disable()
	{
		if (!App.IsExiting && !KMonoBehaviour.isLoadingScene && hasEnableRun)
		{
			hasEnableRun = false;
			SuspendUpdates(true);
			if (batch != null)
			{
				DeRegister();
			}
			ConfigureVisibilityListener(false);
			StopAnimEventSequence();
		}
	}

	protected override void OnDestroy()
	{
		if (!App.IsExiting)
		{
			Singleton<CellChangeMonitor>.Instance?.UnregisterMovementStateChanged(base.transform, OnMovementStateChanged);
			Singleton<KBatchedAnimUpdater>.Instance?.UpdateUnregister(this);
			isVisible = false;
			DeRegister();
			stopped = true;
			StopAnimEventSequence();
			batchInstanceData = null;
			batch = null;
			base.OnDestroy();
		}
	}

	public void SetBlendValue(float value)
	{
		batchInstanceData.SetBlend(value);
		SetDirty();
	}

	public bool ApplySymbolOverrides()
	{
		if ((UnityEngine.Object)symbolOverrideController != (UnityEngine.Object)null)
		{
			if (symbolOverrideControllerVersion != symbolOverrideController.version || symbolOverrideController.applySymbolOverridesEveryFrame)
			{
				symbolOverrideControllerVersion = symbolOverrideController.version;
				symbolOverrideController.ApplyOverrides();
			}
			symbolOverrideController.ApplyAtlases();
			return true;
		}
		return false;
	}

	public void SetSymbolOverride(int symbol_idx, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		DebugUtil.Assert(usingNewSymbolOverrideSystem, "KBatchedAnimController requires usingNewSymbolOverrideSystem to bet to true to enable symbol overrides.");
		base.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_idx, symbol_frame_instance);
	}

	protected override void Register()
	{
		if (IsActive() && batch == null && batchGroupID.IsValid && batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			lastChunkXY = KAnimBatchManager.ControllerToChunkXY(this);
			KAnimBatchManager.Instance().Register(this);
			forceRebuild = true;
			SetDirty();
		}
	}

	protected override void DeRegister()
	{
		if (batch != null)
		{
			batch.Deregister(this);
		}
	}

	private void ConfigureUpdateListener()
	{
		if ((IsActive() && !suspendUpdates && isVisible) || moving || visibilityType == VisibilityType.Always)
		{
			Singleton<KBatchedAnimUpdater>.Instance.UpdateRegister(this);
		}
		else
		{
			Singleton<KBatchedAnimUpdater>.Instance.UpdateUnregister(this);
		}
	}

	protected override void SuspendUpdates(bool suspend)
	{
		suspendUpdates = suspend;
		ConfigureUpdateListener();
	}

	public void SetVisiblity(bool is_visible)
	{
		if (is_visible != isVisible)
		{
			isVisible = is_visible;
			if (is_visible)
			{
				SuspendUpdates(false);
				SetDirty();
				UpdateAnimEventSequenceTime();
			}
			else
			{
				SuspendUpdates(true);
				SetDirty();
			}
		}
	}

	private void ConfigureVisibilityListener(bool enabled)
	{
		if (visibilityType != VisibilityType.Always)
		{
			if (enabled)
			{
				RegisterVisibilityListener();
			}
			else
			{
				UnregisterVisibilityListener();
			}
		}
	}

	protected override void RefreshVisibilityListener()
	{
		if (visibilityListenerRegistered)
		{
			ConfigureVisibilityListener(false);
			ConfigureVisibilityListener(true);
		}
	}

	private void RegisterVisibilityListener()
	{
		DebugUtil.Assert(!visibilityListenerRegistered);
		Singleton<KBatchedAnimUpdater>.Instance.VisibilityRegister(this);
		visibilityListenerRegistered = true;
	}

	private void UnregisterVisibilityListener()
	{
		DebugUtil.Assert(visibilityListenerRegistered);
		Singleton<KBatchedAnimUpdater>.Instance.VisibilityUnregister(this);
		visibilityListenerRegistered = false;
	}

	public void SetSceneLayer(Grid.SceneLayer layer)
	{
		float layerZ = Grid.GetLayerZ(layer);
		sceneLayer = layer;
		Vector3 position = base.transform.GetPosition();
		position.z = layerZ;
		base.transform.SetPosition(position);
		DeRegister();
		Register();
	}
}
