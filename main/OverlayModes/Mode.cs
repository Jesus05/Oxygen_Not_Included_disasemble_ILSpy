using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public abstract class Mode
	{
		private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();

		public static void Clear()
		{
			workingTargets.Clear();
		}

		public abstract SimViewMode ViewMode();

		public virtual void Enable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Disable()
		{
		}

		public abstract string GetSoundName();

		public void RegisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister += OnSaveLoadRootRegistered;
			saveManager.onUnregister += OnSaveLoadRootUnregistered;
		}

		public void UnregisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister -= OnSaveLoadRootRegistered;
			saveManager.onUnregister -= OnSaveLoadRootUnregistered;
		}

		protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
		{
		}

		protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
		{
		}

		protected void ProcessExistingSaveLoadRoots()
		{
			foreach (KeyValuePair<Tag, List<SaveLoadRoot>> list in SaveLoader.Instance.saveManager.GetLists())
			{
				foreach (SaveLoadRoot item in list.Value)
				{
					OnSaveLoadRootRegistered(item);
				}
			}
		}

		protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
			UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (Tag item in (IEnumerable<Tag>)tags)
			{
				List<SaveLoadRoot> value = null;
				if (lists.TryGetValue(item, out value))
				{
					foreach (SaveLoadRoot item2 in value)
					{
						T component = item2.GetComponent<T>();
						if (component != null)
						{
							uniformGrid.Add(component);
						}
					}
				}
			}
			return uniformGrid;
		}

		protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						ResetDisplayValues(component);
					}
				}
			}
		}

		protected static void ResetDisplayValues(KBatchedAnimController controller)
		{
			controller.SetLayer(0);
			controller.HighlightColour = Color.clear;
			controller.TintColour = Color.white;
			controller.SetLayer(controller.GetComponent<KPrefabID>().defaultLayer);
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, Vector2I min, Vector2I max, Action<T> on_removed = null) where T : KMonoBehaviour
		{
			ClearOutsideViewObjects(targets, min, max, null, delegate(T cmp)
			{
				if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
				{
					KBatchedAnimController component = ((Component)cmp).GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						ResetDisplayValues(component);
					}
					if (on_removed != null)
					{
						on_removed(cmp);
					}
				}
			});
			workingTargets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, Vector2I vis_min, Vector2I vis_max, ICollection<Tag> item_ids, Action<T> on_remove) where T : KMonoBehaviour
		{
			workingTargets.Clear();
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					Vector2I vector2I = Grid.PosToXY(current.transform.GetPosition());
					if (!(vis_min <= vector2I) || !(vector2I <= vis_max))
					{
						workingTargets.Add((KMonoBehaviour)current);
					}
					else
					{
						KPrefabID component = current.GetComponent<KPrefabID>();
						if (item_ids != null && !item_ids.Contains(component.PrefabTag))
						{
							workingTargets.Add((KMonoBehaviour)current);
						}
					}
				}
			}
			foreach (T workingTarget in workingTargets)
			{
				if (!((UnityEngine.Object)workingTarget == (UnityEngine.Object)null))
				{
					on_remove?.Invoke(workingTarget);
					targets.Remove(workingTarget);
				}
			}
			workingTargets.Clear();
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null, Func<T, bool> special_clear_condition = null) where T : IUniformGridObject
		{
			ClearOutsideViewObjects(targets, working_targets, vis_min, vis_max, delegate(T cmp)
			{
				if (cmp != null && on_removed != null)
				{
					on_removed(cmp);
				}
			});
			if (special_clear_condition != null)
			{
				working_targets.Clear();
				foreach (T item in (IEnumerable<T>)targets)
				{
					if (special_clear_condition(item))
					{
						working_targets.Add(item);
					}
				}
				foreach (T item2 in (IEnumerable<T>)working_targets)
				{
					if (item2 != null)
					{
						if (on_removed != null)
						{
							on_removed(item2);
						}
						targets.Remove(item2);
					}
				}
				working_targets.Clear();
			}
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null) where T : IUniformGridObject
		{
			working_targets.Clear();
			foreach (T item in (IEnumerable<T>)targets)
			{
				if (item != null)
				{
					Vector2 vector = item.PosMin();
					Vector2 vector2 = item.PosMin();
					if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || (float)vis_max.x < vector.x || (float)vis_max.y < vector.y)
					{
						working_targets.Add(item);
					}
				}
			}
			foreach (T item2 in (IEnumerable<T>)working_targets)
			{
				if (item2 != null)
				{
					on_removed?.Invoke(item2);
					targets.Remove(item2);
				}
			}
			working_targets.Clear();
		}

		protected static float GetDefaultDepth(KMonoBehaviour cmp)
		{
			BuildingComplete component = cmp.GetComponent<BuildingComplete>();
			if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
			{
				return Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			}
			return Grid.GetLayerZ(component.Def.SceneLayer);
		}

		protected void UpdateHighlightTypeOverlay<T>(Vector2I min, Vector2I max, ICollection<T> targets, ICollection<Tag> item_ids, ColorHighlightCondition[] highlights, BringToFrontLayerSetting bringToFrontSetting, int layer) where T : KMonoBehaviour
		{
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					Vector3 position = current.transform.GetPosition();
					int cell = Grid.PosToCell(position);
					if (Grid.IsValidCell(cell) && Grid.IsVisible(cell) && min <= (Vector2)position && (Vector2)position <= max)
					{
						KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
						if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
						{
							int layer2 = 0;
							Color32 highlightColour = Color.clear;
							if (highlights != null)
							{
								foreach (ColorHighlightCondition colorHighlightCondition in highlights)
								{
									if (colorHighlightCondition.highlight_condition(current))
									{
										highlightColour = colorHighlightCondition.highlight_color(current);
										layer2 = layer;
										break;
									}
								}
							}
							switch (bringToFrontSetting)
							{
							case BringToFrontLayerSetting.Constant:
								component.SetLayer(layer);
								break;
							case BringToFrontLayerSetting.Conditional:
								component.SetLayer(layer2);
								break;
							}
							component.HighlightColour = highlightColour;
						}
					}
				}
			}
		}

		protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
		{
			Color32 highlightColour = Color.clear;
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.HighlightColour = highlightColour;
						component.SetLayer(0);
					}
				}
			}
			targets.Clear();
		}

		protected void AddTargetIfVisible<T>(T instance, Vector2I vis_min, Vector2I vis_max, ICollection<T> targets, int layer, Action<T> on_added = null, Func<KMonoBehaviour, bool> should_add = null) where T : IUniformGridObject
		{
			if (!instance.Equals(null))
			{
				Vector2 vector = instance.PosMin();
				Vector2 vector2 = instance.PosMax();
				if (!(vector2.x < (float)vis_min.x) && !(vector2.y < (float)vis_min.y) && !(vector.x > (float)vis_max.x) && !(vector.y > (float)vis_max.y) && !targets.Contains(instance))
				{
					bool flag = false;
					for (int i = (int)vector.y; (float)i <= vector2.y; i++)
					{
						for (int j = (int)vector.x; (float)j <= vector2.x; j++)
						{
							int num = Grid.XYToCell(j, i);
							if (Grid.Visible[num] > 20 || !PropertyTextures.IsFogOfWarEnabled)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						bool flag2 = true;
						KMonoBehaviour kMonoBehaviour = instance as KMonoBehaviour;
						if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null && should_add != null)
						{
							flag2 = should_add(kMonoBehaviour);
						}
						if (flag2)
						{
							if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
							{
								KBatchedAnimController component = kMonoBehaviour.GetComponent<KBatchedAnimController>();
								if ((UnityEngine.Object)component != (UnityEngine.Object)null)
								{
									component.SetLayer(layer);
								}
							}
							targets.Add(instance);
							on_added?.Invoke(instance);
						}
					}
				}
			}
		}
	}
}
