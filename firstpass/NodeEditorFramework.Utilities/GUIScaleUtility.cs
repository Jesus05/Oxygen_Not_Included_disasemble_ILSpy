using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class GUIScaleUtility
	{
		private static bool compabilityMode;

		private static bool initiated;

		private static FieldInfo currentGUILayoutCache;

		private static FieldInfo currentTopLevelGroup;

		private static Func<Rect> GetTopRectDelegate;

		private static Func<Rect> topmostRectDelegate;

		private static List<List<Rect>> rectStackGroups;

		private static List<Matrix4x4> GUIMatrices;

		private static List<bool> adjustedGUILayout;

		public static Rect getTopRect => GetTopRectDelegate();

		public static Rect getTopRectScreenSpace => topmostRectDelegate();

		public static List<Rect> currentRectStack
		{
			get;
			private set;
		}

		public static Vector2 getCurrentScale => new Vector2(1f / GUI.matrix.GetColumn(0).magnitude, 1f / GUI.matrix.GetColumn(1).magnitude);

		public static void CheckInit()
		{
			if (!initiated)
			{
				Init();
			}
		}

		public static void Init()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(GUI));
			Type type = assembly.GetType("UnityEngine.GUIClip", true);
			PropertyInfo property = type.GetProperty("topmostRect", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method = type.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method2 = type.GetMethod("Clip", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[1]
			{
				typeof(Rect)
			}, new ParameterModifier[0]);
			if (type == null || property == null || method == null || method2 == null)
			{
				Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
				Debug.LogWarning(((type != null) ? "" : "GUIClipType is Null, ") + ((property != null) ? "" : "topmostRect is Null, ") + ((method != null) ? "" : "GetTopRect is Null, ") + ((method2 != null) ? "" : "ClipRect is Null, "));
				compabilityMode = true;
				initiated = true;
			}
			else
			{
				GetTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), method);
				topmostRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), property.GetGetMethod());
				if (GetTopRectDelegate == null || topmostRectDelegate == null)
				{
					Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
					Debug.LogWarning(((type != null) ? "" : "GUIClipType is Null, ") + ((property != null) ? "" : "topmostRect is Null, ") + ((method != null) ? "" : "GetTopRect is Null, ") + ((method2 != null) ? "" : "ClipRect is Null, "));
					compabilityMode = true;
					initiated = true;
				}
				else
				{
					currentRectStack = new List<Rect>();
					rectStackGroups = new List<List<Rect>>();
					GUIMatrices = new List<Matrix4x4>();
					adjustedGUILayout = new List<bool>();
					initiated = true;
				}
			}
		}

		public static Vector2 BeginScale(ref Rect rect, Vector2 zoomPivot, float zoom, bool adjustGUILayout)
		{
			Rect rect2;
			if (compabilityMode)
			{
				GUI.EndGroup();
				rect2 = rect;
			}
			else
			{
				BeginNoClip();
				rect2 = GUIToScaledSpace(rect);
			}
			rect = Scale(rect2, rect2.position + zoomPivot, new Vector2(zoom, zoom));
			GUI.BeginGroup(rect);
			rect.position = Vector2.zero;
			Vector2 vector = rect.center - rect2.size / 2f + zoomPivot;
			adjustedGUILayout.Add(adjustGUILayout);
			if (adjustGUILayout)
			{
				GUILayout.BeginHorizontal();
				Vector2 center = rect.center;
				float x = center.x;
				Vector2 size = rect2.size;
				GUILayout.Space(x - size.x + zoomPivot.x);
				GUILayout.BeginVertical();
				Vector2 center2 = rect.center;
				float y = center2.y;
				Vector2 size2 = rect2.size;
				GUILayout.Space(y - size2.y + zoomPivot.y);
			}
			GUIMatrices.Add(GUI.matrix);
			GUIUtility.ScaleAroundPivot(new Vector2(1f / zoom, 1f / zoom), vector);
			return vector;
		}

		public static void EndScale()
		{
			if (GUIMatrices.Count == 0 || adjustedGUILayout.Count == 0)
			{
				throw new UnityException("GUIScaleUtility: You are ending more scale regions than you are beginning!");
			}
			GUI.matrix = GUIMatrices[GUIMatrices.Count - 1];
			GUIMatrices.RemoveAt(GUIMatrices.Count - 1);
			if (adjustedGUILayout[adjustedGUILayout.Count - 1])
			{
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
			adjustedGUILayout.RemoveAt(adjustedGUILayout.Count - 1);
			GUI.EndGroup();
			if (compabilityMode)
			{
				if (!Application.isPlaying)
				{
					GUI.BeginClip(new Rect(0f, 23f, (float)Screen.width, (float)(Screen.height - 23)));
				}
				else
				{
					GUI.BeginClip(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
				}
			}
			else
			{
				RestoreClips();
			}
		}

		public static void BeginNoClip()
		{
			List<Rect> list = new List<Rect>();
			Rect getTopRect = GUIScaleUtility.getTopRect;
			while (getTopRect != new Rect(-10000f, -10000f, 40000f, 40000f))
			{
				list.Add(getTopRect);
				GUI.EndClip();
				getTopRect = GUIScaleUtility.getTopRect;
			}
			list.Reverse();
			rectStackGroups.Add(list);
			currentRectStack.AddRange(list);
		}

		public static void MoveClipsUp(int count)
		{
			List<Rect> list = new List<Rect>();
			Rect getTopRect = GUIScaleUtility.getTopRect;
			while (getTopRect != new Rect(-10000f, -10000f, 40000f, 40000f) && count > 0)
			{
				list.Add(getTopRect);
				GUI.EndClip();
				getTopRect = GUIScaleUtility.getTopRect;
				count--;
			}
			list.Reverse();
			rectStackGroups.Add(list);
			currentRectStack.AddRange(list);
		}

		public static void RestoreClips()
		{
			if (rectStackGroups.Count == 0)
			{
				Debug.LogError("GUIClipHierarchy: BeginNoClip/MoveClipsUp - RestoreClips count not balanced!");
			}
			else
			{
				List<Rect> list = rectStackGroups[rectStackGroups.Count - 1];
				for (int i = 0; i < list.Count; i++)
				{
					GUI.BeginClip(list[i]);
					currentRectStack.RemoveAt(currentRectStack.Count - 1);
				}
				rectStackGroups.RemoveAt(rectStackGroups.Count - 1);
			}
		}

		public static void BeginNewLayout()
		{
			if (!compabilityMode)
			{
				Rect getTopRect = GUIScaleUtility.getTopRect;
				if (getTopRect != new Rect(-10000f, -10000f, 40000f, 40000f))
				{
					GUILayout.BeginArea(new Rect(0f, 0f, getTopRect.width, getTopRect.height));
				}
				else
				{
					GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
				}
			}
		}

		public static void EndNewLayout()
		{
			if (!compabilityMode)
			{
				GUILayout.EndArea();
			}
		}

		public static void BeginIgnoreMatrix()
		{
			GUIMatrices.Add(GUI.matrix);
			GUI.matrix = Matrix4x4.identity;
		}

		public static void EndIgnoreMatrix()
		{
			if (GUIMatrices.Count == 0)
			{
				throw new UnityException("GUIScaleutility: You are ending more ignoreMatrices than you are beginning!");
			}
			GUI.matrix = GUIMatrices[GUIMatrices.Count - 1];
			GUIMatrices.RemoveAt(GUIMatrices.Count - 1);
		}

		public static Vector2 Scale(Vector2 pos, Vector2 pivot, Vector2 scale)
		{
			return Vector2.Scale(pos - pivot, scale) + pivot;
		}

		public static Rect Scale(Rect rect, Vector2 pivot, Vector2 scale)
		{
			rect.position = Vector2.Scale(rect.position - pivot, scale) + pivot;
			rect.size = Vector2.Scale(rect.size, scale);
			return rect;
		}

		public static Vector2 ScaledToGUISpace(Vector2 scaledPosition)
		{
			if (rectStackGroups != null && rectStackGroups.Count != 0)
			{
				List<Rect> list = rectStackGroups[rectStackGroups.Count - 1];
				for (int i = 0; i < list.Count; i++)
				{
					scaledPosition -= list[i].position;
				}
				return scaledPosition;
			}
			return scaledPosition;
		}

		public static Rect ScaledToGUISpace(Rect scaledRect)
		{
			if (rectStackGroups != null && rectStackGroups.Count != 0)
			{
				scaledRect.position = ScaledToGUISpace(scaledRect.position);
				return scaledRect;
			}
			return scaledRect;
		}

		public static Vector2 GUIToScaledSpace(Vector2 guiPosition)
		{
			if (rectStackGroups != null && rectStackGroups.Count != 0)
			{
				List<Rect> list = rectStackGroups[rectStackGroups.Count - 1];
				for (int i = 0; i < list.Count; i++)
				{
					guiPosition += list[i].position;
				}
				return guiPosition;
			}
			return guiPosition;
		}

		public static Rect GUIToScaledSpace(Rect guiRect)
		{
			if (rectStackGroups != null && rectStackGroups.Count != 0)
			{
				guiRect.position = GUIToScaledSpace(guiRect.position);
				return guiRect;
			}
			return guiRect;
		}

		public static Vector2 GUIToScreenSpace(Vector2 guiPosition)
		{
			return guiPosition + getTopRectScreenSpace.position;
		}

		public static Rect GUIToScreenSpace(Rect guiRect)
		{
			guiRect.position += getTopRectScreenSpace.position;
			return guiRect;
		}
	}
}
