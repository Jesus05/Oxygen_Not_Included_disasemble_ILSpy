using Klei;
using KSerialization;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;

public class Global : MonoBehaviour
{
	public SpriteAtlas[] forcedAtlasInitializationList;

	public GameObject modErrorsPrefab;

	public GameObject globalCanvas;

	private GameInputManager mInputManager;

	private AnimEventManager mAnimEventManager;

	public ModManager modManager;

	public LayeredFileSystem layeredFileSystem;

	public StandardFileSystem standardFS;

	public ZipFileSystem worldGenZipFS;

	private bool gotKleiUserID = false;

	private Thread mainThread;

	public static readonly string LanguagePackKey = "LanguagePack";

	public static readonly string LanguageCodeKey = "LanguageCode";

	public static Global Instance
	{
		get;
		private set;
	}

	public static BindingEntry[] GenerateDefaultBindings()
	{
		List<BindingEntry> list = new List<BindingEntry>();
		list.Add(new BindingEntry(null, GamepadButton.NumButtons, KKeyCode.Escape, Modifier.None, Action.Escape, false, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, Action.PanUp, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, Action.PanDown, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, Action.PanLeft, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, Action.PanRight, true, false));
		list.Add(new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, Action.RotateBuilding, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, Action.ManagePeople, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, Action.ManageConsumables, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, Action.ManageVitals, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, Action.ManageResearch, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, Action.ManageReport, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, Action.ManageCodex, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, Action.ManageRoles, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Period, Modifier.None, Action.ManageSchedule, true, false));
		list.Add(new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, Action.ManageStarmap, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, Action.Dig, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, Action.Mop, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, Action.Clear, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, Action.Disinfect, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, Action.Attack, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, Action.Capture, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, Action.Harvest, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Insert, Modifier.None, Action.EmptyPipe, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, Action.Prioritize, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.Alt, Action.ToggleScreenshotMode, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, Action.BuildingCancel, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, Action.BuildingDeconstruct, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Tab, Modifier.None, Action.CycleSpeed, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, Action.CameraHome, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.None, Action.MouseLeft, false, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.Shift, Action.ShiftMouseLeft, false, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse1, Modifier.None, Action.MouseRight, false, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse2, Modifier.None, Action.MouseMiddle, false, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.None, Action.Plan1, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.None, Action.Plan2, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.None, Action.Plan3, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.None, Action.Plan4, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.None, Action.Plan5, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.None, Action.Plan6, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.None, Action.Plan7, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.None, Action.Plan8, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.None, Action.Plan9, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.None, Action.Plan10, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.None, Action.Plan11, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.None, Action.Plan12, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, Action.CopyBuilding, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollUp, Modifier.None, Action.ZoomIn, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollDown, Modifier.None, Action.ZoomOut, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.None, Action.Overlay1, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.None, Action.Overlay2, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.None, Action.Overlay3, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.None, Action.Overlay4, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F5, Modifier.None, Action.Overlay5, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F6, Modifier.None, Action.Overlay6, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F7, Modifier.None, Action.Overlay7, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F8, Modifier.None, Action.Overlay8, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F9, Modifier.None, Action.Overlay9, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F10, Modifier.None, Action.Overlay10, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F11, Modifier.None, Action.Overlay11, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Shift, Action.Overlay12, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Shift, Action.Overlay13, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Shift, Action.Overlay14, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadPlus, Modifier.None, Action.SpeedUp, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadMinus, Modifier.None, Action.SlowDown, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Space, Modifier.None, Action.TogglePause, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Ctrl, Action.SetUserNav1, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Ctrl, Action.SetUserNav2, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Ctrl, Action.SetUserNav3, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Ctrl, Action.SetUserNav4, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Ctrl, Action.SetUserNav5, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Ctrl, Action.SetUserNav6, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Ctrl, Action.SetUserNav7, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Ctrl, Action.SetUserNav8, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Ctrl, Action.SetUserNav9, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Ctrl, Action.SetUserNav10, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Shift, Action.GotoUserNav1, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Shift, Action.GotoUserNav2, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Shift, Action.GotoUserNav3, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Shift, Action.GotoUserNav4, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Shift, Action.GotoUserNav5, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Shift, Action.GotoUserNav6, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Shift, Action.GotoUserNav7, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Shift, Action.GotoUserNav8, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Shift, Action.GotoUserNav9, true, false));
		list.Add(new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Shift, Action.GotoUserNav10, true, false));
		list.Add(new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Slash, Modifier.None, Action.ToggleOpen, true, false));
		list.Add(new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, Action.ToggleEnabled, true, false));
		list.Add(new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Backslash, Modifier.None, Action.BuildingUtility1, true, false));
		list.Add(new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.LeftBracket, Modifier.None, Action.BuildingUtility2, true, false));
		list.Add(new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.RightBracket, Modifier.None, Action.BuildingUtility3, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.LeftAlt, Modifier.Alt, Action.AlternateView, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.RightAlt, Modifier.Alt, Action.AlternateView, true, false));
		list.Add(new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.LeftShift, Modifier.Shift, Action.DragStraight, true, false));
		list.Add(new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.RightShift, Modifier.Shift, Action.DragStraight, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Ctrl, Action.DebugFocus, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.U, Modifier.Ctrl, Action.DebugUltraTestMode, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Alt, Action.DebugToggleUI, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Alt, Action.DebugCollectGarbage, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, Modifier.Alt, Action.DebugInvincible, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Alt, Action.DebugForceLightEverywhere, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Shift, Action.DebugVisualTest, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Shift, Action.DebugElementTest, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Shift, Action.DebugRiverTest, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Shift, Action.DebugTileTest, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.N, Modifier.Alt, Action.DebugRefreshNavCell, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Ctrl, Action.DebugGotoTarget, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.S, Modifier.Ctrl, Action.DebugSelectMaterial, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.M, Modifier.Ctrl, Action.DebugToggleMusic, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.None, Action.DebugToggle, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Alt, Action.DebugTeleport, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Ctrl, Action.DebugSpawnMinion, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Ctrl, Action.DebugPlace, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Ctrl, Action.DebugInstantBuildMode, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F5, Modifier.Ctrl, Action.DebugSlowTestMode, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Ctrl, Action.DebugDig, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F8, Modifier.Ctrl, Action.DebugExplosion, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F9, Modifier.Ctrl, Action.DebugDiscoverAllElements, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Alt, Action.DebugToggleSelectInEditor, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.P, Modifier.Alt, Action.DebugPathFinding, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Alt, Action.DebugSuperSpeed, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Alt, Action.DebugGameStep, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Alt, Action.DebugSimStep, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.X, Modifier.Alt, Action.DebugNotification, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Alt, Action.DebugNotificationMessage, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.None, Action.ToggleProfiler, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.Alt, Action.ToggleChromeProfiler, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Ctrl, Action.DebugDumpSceneParitionerLeakData, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Ctrl, Action.DebugTriggerException, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, (Modifier)6, Action.DebugTriggerError, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Ctrl, Action.DebugDumpGCRoots, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, (Modifier)3, Action.DebugDumpGarbageReferences, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Ctrl, Action.DebugDumpEventData, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, (Modifier)3, Action.DebugCrashSim, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Alt, Action.DebugNextCall, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Alt, Action.SreenShot1x, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Alt, Action.SreenShot2x, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Alt, Action.SreenShot8x, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Alt, Action.SreenShot32x, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Alt, Action.DebugLockCursor, true, false));
		list.Add(new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Alt, Action.DebugTogglePersonalPriorityComparison, true, false));
		list.Add(new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, Action.DialogSubmit, false, false));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, Action.BuildMenuKeyA, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, Action.BuildMenuKeyB, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, Action.BuildMenuKeyC, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, Action.BuildMenuKeyD, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, Action.BuildMenuKeyE, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, Action.BuildMenuKeyF, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, Action.BuildMenuKeyG, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, Action.BuildMenuKeyH, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, Action.BuildMenuKeyI, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, Action.BuildMenuKeyJ, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, Action.BuildMenuKeyK, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, Action.BuildMenuKeyL, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, Action.BuildMenuKeyM, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, Action.BuildMenuKeyN, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, Action.BuildMenuKeyO, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, Action.BuildMenuKeyP, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Q, Modifier.None, Action.BuildMenuKeyQ, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, Action.BuildMenuKeyR, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, Action.BuildMenuKeyS, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, Action.BuildMenuKeyT, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, Action.BuildMenuKeyU, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, Action.BuildMenuKeyV, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, Action.BuildMenuKeyW, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, Action.BuildMenuKeyX, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, Action.BuildMenuKeyY, false, true));
		list.Add(new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, Action.BuildMenuKeyZ, false, true));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.B, Modifier.Shift, Action.SandboxBrush, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.N, Modifier.Shift, Action.SandboxSprinkle, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.F, Modifier.Shift, Action.SandboxFlood, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.K, Modifier.Shift, Action.SandboxSample, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.H, Modifier.Shift, Action.SandboxHeatGun, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.C, Modifier.Shift, Action.SandboxClearFloor, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.X, Modifier.Shift, Action.SandboxDestroy, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.E, Modifier.Shift, Action.SandboxSpawnEntity, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.S, Modifier.Shift, Action.ToggleSandboxTools, true, false));
		list.Add(new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.R, Modifier.Shift, Action.SandboxReveal, true, false));
		List<BindingEntry> list2 = list;
		IList<BuildMenu.DisplayInfo> list3 = (IList<BuildMenu.DisplayInfo>)BuildMenu.OrderedBuildings.data;
		foreach (BuildMenu.DisplayInfo item in list3)
		{
			AddBindings(HashedString.Invalid, item, list2);
		}
		return list2.ToArray();
	}

	private static void AddBindings(HashedString parent_category, BuildMenu.DisplayInfo display_info, List<BindingEntry> bindings)
	{
		if (display_info.data != null)
		{
			Type type = display_info.data.GetType();
			if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
			{
				IList<BuildMenu.DisplayInfo> list = (IList<BuildMenu.DisplayInfo>)display_info.data;
				foreach (BuildMenu.DisplayInfo item2 in list)
				{
					AddBindings(display_info.category, item2, bindings);
				}
			}
			else if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(type))
			{
				string str = HashCache.Get().Get(parent_category);
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				string group = textInfo.ToTitleCase(str) + " Menu";
				BindingEntry item = new BindingEntry(group, GamepadButton.NumButtons, display_info.keyCode, Modifier.None, display_info.hotkey, true, true);
				bindings.Add(item);
			}
		}
	}

	private void Awake()
	{
		globalCanvas = GameObject.Find("Canvas");
		UnityEngine.Object.DontDestroyOnLoad(globalCanvas.gameObject);
		OutputSystemInfo();
		Instance = this;
		if (forcedAtlasInitializationList != null)
		{
			SpriteAtlas[] array = forcedAtlasInitializationList;
			foreach (SpriteAtlas spriteAtlas in array)
			{
				int spriteCount = spriteAtlas.spriteCount;
				Sprite[] array2 = new Sprite[spriteCount];
				spriteAtlas.GetSprites(array2);
				Sprite[] array3 = array2;
				foreach (Sprite sprite in array3)
				{
					Texture2D texture = sprite.texture;
					if ((UnityEngine.Object)texture != (UnityEngine.Object)null)
					{
						texture.filterMode = FilterMode.Bilinear;
						texture.anisoLevel = 4;
						texture.mipMapBias = 0f;
					}
				}
			}
		}
		LayeredFileSystem.CreateInstance();
		layeredFileSystem = LayeredFileSystem.instance;
		standardFS = new StandardFileSystem();
		layeredFileSystem.AddFileSystem(standardFS);
		Singleton<StateMachineUpdater>.CreateInstance();
		Singleton<StateMachineManager>.CreateInstance();
		modManager = new ModManager();
		modManager.Start();
		Manager.Initialize();
		mInputManager = new GameInputManager(GenerateDefaultBindings());
		Audio.Get();
		KAnimBatchManager.CreateInstance();
		Singleton<SoundEventVolumeCache>.CreateInstance();
		mAnimEventManager = new AnimEventManager();
		Singleton<KBatchedAnimUpdater>.CreateInstance();
		DistributionPlatform.Initialize();
		Localization.Initialize(false);
		KProfiler.main_thread = Thread.CurrentThread;
		RestoreLegacyMetricsSetting();
		if (DistributionPlatform.Initialized)
		{
			if (!KPrivacyPrefs.instance.disableDataCollection)
			{
				Debug.Log("Logged into " + DistributionPlatform.Inst.Name + " with ID:" + DistributionPlatform.Inst.LocalUser.Id + ", NAME:" + DistributionPlatform.Inst.LocalUser.Name, null);
				ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(OnGetUserIdKey);
			}
		}
		else
		{
			Debug.LogWarning("Can't init " + DistributionPlatform.Inst.Name + " distribution platform...", null);
			OnGetUserIdKey();
		}
		GlobalResources.Instance();
	}

	private void RestoreLegacyMetricsSetting()
	{
		if (KPlayerPrefs.GetInt("ENABLE_METRICS", 1) == 0)
		{
			KPlayerPrefs.DeleteKey("ENABLE_METRICS");
			KPlayerPrefs.Save();
			KPrivacyPrefs.instance.disableDataCollection = true;
			KPrivacyPrefs.Save();
		}
	}

	public GameInputManager GetInputManager()
	{
		return mInputManager;
	}

	public AnimEventManager GetAnimEventManager()
	{
		if (!App.IsExiting)
		{
			return mAnimEventManager;
		}
		return null;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (mInputManager != null)
		{
			mInputManager.OnApplicationFocus(focus);
		}
	}

	private void OnGetUserIdKey()
	{
		gotKleiUserID = true;
	}

	private void Update()
	{
		mInputManager.Update();
		if (mAnimEventManager != null)
		{
			mAnimEventManager.Update();
		}
		if (DistributionPlatform.Initialized && (UnityEngine.Object)SteamUGCService.Instance == (UnityEngine.Object)null)
		{
			SteamUGCService.Initialize();
			modManager.RegisterUGCEventHandlers(SteamUGCService.Instance);
		}
		if (gotKleiUserID)
		{
			gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(SetONIStaticSessionVariables, SetONIDynamicSessionVariables);
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
	}

	private void SetONIStaticSessionVariables()
	{
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", "preview");
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", 297993u);
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, KPlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey).ToString());
		}
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey).ToString());
		}
		if (SteamManager.Initialized)
		{
			PublishedFileId_t installed;
			string installedLanguageCode = LanguageOptionsScreen.GetInstalledLanguageCode(out installed);
			if (installed != PublishedFileId_t.Invalid)
			{
				ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(LanguagePackKey, installed.m_PublishedFileId);
			}
			if (!string.IsNullOrEmpty(installedLanguageCode))
			{
				ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(LanguageCodeKey, installedLanguageCode);
			}
		}
	}

	private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
	{
		if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null && (UnityEngine.Object)GameClock.Instance != (UnityEngine.Object)null)
		{
			data.Add("GameTimeSeconds", (int)GameClock.Instance.GetTime());
		}
	}

	private void LateUpdate()
	{
		Singleton<KBatchedAnimUpdater>.Instance.LateUpdate();
	}

	private void OnDestroy()
	{
		if (modManager != null)
		{
			modManager.Shutdown();
		}
		Instance = null;
		if (mAnimEventManager != null)
		{
			mAnimEventManager.FreeResources();
		}
		Singleton<KBatchedAnimUpdater>.DestroyInstance();
	}

	private void OnApplicationQuit()
	{
		KGlobalAnimParser.DestroyInstance();
		ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
	}

	private void OutputSystemInfo()
	{
		try
		{
			Console.WriteLine("SYSTEM INFO:");
			Dictionary<string, object> hardwareStats = KleiMetrics.GetHardwareStats();
			foreach (KeyValuePair<string, object> item in hardwareStats)
			{
				try
				{
					Console.WriteLine($"    {item.Key.ToString()}={item.Value.ToString()}");
				}
				catch
				{
				}
			}
			Console.WriteLine(string.Format("    {0}={1}", "System Language", Application.systemLanguage.ToString()));
		}
		catch
		{
		}
	}
}
