using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SelectToolHoverTextCard : HoverTextConfiguration
{
	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();

	public int recentNumberOfDisplayedSelectables;

	public int currentSelectedSelectableIndex = -1;

	private Sprite iconWarning;

	private Sprite iconDash;

	public static List<GameObject> highlightedObjects = new List<GameObject>();

	private static readonly List<Type> hiddenChoreConsumerTypes = new List<Type>
	{
		typeof(KSelectableHealthBar)
	};

	private int maskOverlay;

	private string cachedTemperatureString;

	private float cachedTemperature = -3.40282347E+38f;

	private List<KSelectable> overlayValidHoverObjects = new List<KSelectable>();

	private Dictionary<HashedString, Func<KSelectable, bool>> modeFilters = new Dictionary<HashedString, Func<KSelectable, bool>>
	{
		{
			OverlayModes.Oxygen.ID,
			ShouldShowOxygenOverlay
		},
		{
			OverlayModes.Light.ID,
			ShouldShowLightOverlay
		},
		{
			OverlayModes.GasConduits.ID,
			ShouldShowGasConduitOverlay
		},
		{
			OverlayModes.LiquidConduits.ID,
			ShouldShowLiquidConduitOverlay
		},
		{
			OverlayModes.SolidConveyor.ID,
			ShouldShowSolidConveyorOverlay
		},
		{
			OverlayModes.Power.ID,
			ShouldShowPowerOverlay
		},
		{
			OverlayModes.Logic.ID,
			ShouldShowLogicOverlay
		},
		{
			OverlayModes.Disease.ID,
			SelectToolHoverTextCard.ShowOverlayIfHasComponent<PrimaryElement>
		},
		{
			OverlayModes.Decor.ID,
			SelectToolHoverTextCard.ShowOverlayIfHasComponent<DecorProvider>
		},
		{
			OverlayModes.Crop.ID,
			ShouldShowCropOverlay
		},
		{
			OverlayModes.Temperature.ID,
			ShouldShowTemperatureOverlay
		}
	};

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache8;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cache9;

	[CompilerGenerated]
	private static Func<KSelectable, bool> _003C_003Ef__mg_0024cacheA;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		overlayFilterMap.Add(OverlayModes.Oxygen.ID, delegate
		{
			int num3 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num3].IsGas;
		});
		overlayFilterMap.Add(OverlayModes.GasConduits.ID, delegate
		{
			int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num2].IsGas;
		});
		overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, delegate
		{
			int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num].IsLiquid;
		});
		overlayFilterMap.Add(OverlayModes.Decor.ID, () => false);
		overlayFilterMap.Add(OverlayModes.Rooms.ID, () => false);
		overlayFilterMap.Add(OverlayModes.Logic.ID, () => false);
	}

	public override void ConfigureHoverScreen()
	{
		base.ConfigureHoverScreen();
		HoverTextScreen instance = HoverTextScreen.Instance;
		iconWarning = instance.GetSprite("iconWarning");
		iconDash = instance.GetSprite("dash");
		maskOverlay = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		if (item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening)
		{
			return true;
		}
		return false;
	}

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		if ((UnityEngine.Object)iconWarning == (UnityEngine.Object)null)
		{
			ConfigureHoverScreen();
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (!((UnityEngine.Object)OverlayScreen.Instance == (UnityEngine.Object)null) && Grid.IsValidCell(num))
		{
			HoverTextScreen instance = HoverTextScreen.Instance;
			HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
			overlayValidHoverObjects.Clear();
			foreach (KSelectable hoverObject in hoverObjects)
			{
				if (ShouldShowSelectableInCurrentOverlay(hoverObject))
				{
					overlayValidHoverObjects.Add(hoverObject);
				}
			}
			currentSelectedSelectableIndex = -1;
			if (highlightedObjects.Count > 0)
			{
				highlightedObjects.Clear();
			}
			HashedString mode = SimDebugView.Instance.GetMode();
			bool flag = mode == OverlayModes.Disease.ID;
			bool flag2 = true;
			if (Grid.ForceField[num])
			{
				flag2 = false;
			}
			bool flag3 = Grid.IsVisible(num);
			if (!flag3)
			{
				flag2 = false;
			}
			foreach (KeyValuePair<HashedString, Func<bool>> item in overlayFilterMap)
			{
				if (OverlayScreen.Instance.GetMode() == item.Key)
				{
					if (!item.Value())
					{
						flag2 = false;
					}
					break;
				}
			}
			string text = string.Empty;
			string empty = string.Empty;
			if (mode == OverlayModes.HeatFlow.ID)
			{
				if (!Grid.Solid[num] && flag3)
				{
					float thermalComfort = GameUtil.GetThermalComfort(num, 0f);
					float thermalComfort2 = GameUtil.GetThermalComfort(num, -0.0836800039f);
					float num2 = 0f;
					if (thermalComfort2 * 0.001f > -0.278933346f - num2 && thermalComfort2 * 0.001f < 0.278933346f + num2)
					{
						text = UI.OVERLAYS.HEATFLOW.NEUTRAL;
					}
					else if (thermalComfort2 <= ExternalTemperatureMonitor.GetExternalColdThreshold(null))
					{
						text = UI.OVERLAYS.HEATFLOW.COOLING;
					}
					else if (thermalComfort2 >= ExternalTemperatureMonitor.GetExternalWarmThreshold(null))
					{
						text = UI.OVERLAYS.HEATFLOW.HEATING;
					}
					float dtu_s = 1f * thermalComfort;
					text = text + " (" + GameUtil.GetFormattedHeatEnergyRate(dtu_s, GameUtil.HeatEnergyFormatterUnit.Automatic) + ")";
					hoverTextDrawer.BeginShadowBar(false);
					hoverTextDrawer.DrawText(UI.OVERLAYS.HEATFLOW.HOVERTITLE, Styles_Title.Standard);
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
					hoverTextDrawer.EndShadowBar();
				}
			}
			else if (mode == OverlayModes.Decor.ID)
			{
				List<DecorProvider> list = new List<DecorProvider>();
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviderLayer, list);
				float decorAtCell = GameUtil.GetDecorAtCell(num);
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HOVERTITLE, Styles_Title.Standard);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.TOTAL + GameUtil.GetFormattedDecor(decorAtCell, true), Styles_BodyText.Standard);
				if (!Grid.Solid[num] && flag3)
				{
					List<EffectorEntry> list2 = new List<EffectorEntry>();
					List<EffectorEntry> list3 = new List<EffectorEntry>();
					foreach (DecorProvider item2 in list)
					{
						float decorForCell = item2.GetDecorForCell(num);
						if (decorForCell != 0f)
						{
							string name = item2.GetName();
							KMonoBehaviour component = item2.GetComponent<KMonoBehaviour>();
							if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)component.gameObject != (UnityEngine.Object)null)
							{
								highlightedObjects.Add(component.gameObject);
							}
							bool flag4 = false;
							if (decorForCell > 0f)
							{
								for (int i = 0; i < list2.Count; i++)
								{
									EffectorEntry effectorEntry = list2[i];
									if (effectorEntry.name == name)
									{
										EffectorEntry value = list2[i];
										value.count++;
										value.value += decorForCell;
										list2[i] = value;
										flag4 = true;
										break;
									}
								}
								if (!flag4)
								{
									list2.Add(new EffectorEntry(name, decorForCell));
								}
							}
							else
							{
								for (int j = 0; j < list3.Count; j++)
								{
									EffectorEntry effectorEntry2 = list3[j];
									if (effectorEntry2.name == name)
									{
										EffectorEntry value2 = list3[j];
										value2.count++;
										value2.value += decorForCell;
										list3[j] = value2;
										flag4 = true;
										break;
									}
								}
								if (!flag4)
								{
									list3.Add(new EffectorEntry(name, decorForCell));
								}
							}
						}
					}
					int lightDecorBonus = DecorProvider.GetLightDecorBonus(num);
					if (lightDecorBonus > 0)
					{
						list2.Add(new EffectorEntry(UI.OVERLAYS.DECOR.LIGHTING, (float)lightDecorBonus));
					}
					list2.Sort((EffectorEntry x, EffectorEntry y) => y.value.CompareTo(x.value));
					if (list2.Count > 0)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_POSITIVE, Styles_BodyText.Standard);
					}
					foreach (EffectorEntry item3 in list2)
					{
						hoverTextDrawer.NewLine(18);
						hoverTextDrawer.DrawIcon(iconDash, 18);
						hoverTextDrawer.DrawText(item3.ToString(), Styles_BodyText.Standard);
					}
					list3.Sort((EffectorEntry x, EffectorEntry y) => Mathf.Abs(y.value).CompareTo(Mathf.Abs(x.value)));
					if (list3.Count > 0)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_NEGATIVE, Styles_BodyText.Standard);
					}
					foreach (EffectorEntry item4 in list3)
					{
						hoverTextDrawer.NewLine(18);
						hoverTextDrawer.DrawIcon(iconDash, 18);
						hoverTextDrawer.DrawText(item4.ToString(), Styles_BodyText.Standard);
					}
				}
				hoverTextDrawer.EndShadowBar();
			}
			else if (mode == OverlayModes.Rooms.ID)
			{
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
				if (cavityForCell != null)
				{
					Room room = cavityForCell.room;
					RoomType roomType = null;
					if (room != null)
					{
						roomType = room.roomType;
						empty = roomType.Name;
					}
					else
					{
						empty = UI.OVERLAYS.ROOMS.NOROOM.HEADER;
					}
					hoverTextDrawer.BeginShadowBar(false);
					hoverTextDrawer.DrawText(empty, Styles_Title.Standard);
					text = string.Empty;
					if (room != null)
					{
						string empty2 = string.Empty;
						empty2 = RoomDetails.EFFECT.resolve_string_function(room);
						string empty3 = string.Empty;
						empty3 = RoomDetails.ASSIGNED_TO.resolve_string_function(room);
						string empty4 = string.Empty;
						empty4 = RoomConstraints.RoomCriteriaString(room);
						string empty5 = string.Empty;
						empty5 = RoomDetails.EFFECTS.resolve_string_function(room);
						if (empty2 != string.Empty)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawText(empty2, Styles_BodyText.Standard);
						}
						if (empty3 != string.Empty && roomType != Db.Get().RoomTypes.Neutral)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawText(empty3, Styles_BodyText.Standard);
						}
						hoverTextDrawer.NewLine(22);
						hoverTextDrawer.DrawText(RoomDetails.RoomDetailString(room), Styles_BodyText.Standard);
						if (empty4 != string.Empty)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawText(empty4, Styles_BodyText.Standard);
						}
						if (empty5 != string.Empty)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawText(empty5, Styles_BodyText.Standard);
						}
					}
					else
					{
						string text2 = UI.OVERLAYS.ROOMS.NOROOM.DESC;
						int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
						if (cavityForCell.numCells > maxRoomSize)
						{
							text2 = text2 + "\n" + string.Format(UI.OVERLAYS.ROOMS.NOROOM.TOO_BIG, cavityForCell.numCells, maxRoomSize);
						}
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(text2, Styles_BodyText.Standard);
					}
					hoverTextDrawer.EndShadowBar();
				}
			}
			else if (mode == OverlayModes.Light.ID && flag3)
			{
				string text3 = text;
				text = text3 + string.Format(UI.OVERLAYS.LIGHTING.DESC, Grid.LightIntensity[num]) + " (" + GameUtil.GetLightDescription(Grid.LightIntensity[num]) + ")";
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(UI.OVERLAYS.LIGHTING.HOVERTITLE, Styles_Title.Standard);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
			int num3 = 0;
			ChoreConsumer choreConsumer = null;
			if ((UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)null)
			{
				choreConsumer = SelectTool.Instance.selected.GetComponent<ChoreConsumer>();
			}
			for (int k = 0; k < overlayValidHoverObjects.Count; k++)
			{
				if ((UnityEngine.Object)overlayValidHoverObjects[k] != (UnityEngine.Object)null && (UnityEngine.Object)overlayValidHoverObjects[k].GetComponent<CellSelectionObject>() == (UnityEngine.Object)null)
				{
					KSelectable kSelectable = overlayValidHoverObjects[k];
					if ((!((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null) || !(OverlayScreen.Instance.mode != OverlayModes.None.ID) || (kSelectable.gameObject.layer & maskOverlay) == 0) && flag3)
					{
						PrimaryElement component2 = kSelectable.GetComponent<PrimaryElement>();
						bool flag5 = (UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)overlayValidHoverObjects[k];
						if (flag5)
						{
							currentSelectedSelectableIndex = k;
						}
						num3++;
						hoverTextDrawer.BeginShadowBar(flag5);
						string text4 = GameUtil.GetUnitFormattedName(overlayValidHoverObjects[k].gameObject, true);
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && (UnityEngine.Object)kSelectable.GetComponent<Building>() != (UnityEngine.Object)null)
						{
							text4 = StringFormatter.Replace(StringFormatter.Replace(UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", text4), "{Element}", component2.Element.nameUpperCase);
						}
						hoverTextDrawer.DrawText(text4, Styles_Title.Standard);
						bool flag6 = false;
						string text5 = UI.OVERLAYS.DISEASE.NO_DISEASE;
						if (flag)
						{
							if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.DiseaseIdx != 255)
							{
								text5 = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, true);
							}
							flag6 = true;
							Storage component3 = kSelectable.GetComponent<Storage>();
							if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && component3.showInUI)
							{
								List<GameObject> items = component3.items;
								for (int l = 0; l < items.Count; l++)
								{
									GameObject gameObject = items[l];
									if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
									{
										PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
										if (component4.DiseaseIdx != 255)
										{
											text5 += string.Format(UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, gameObject.GetComponent<KSelectable>().GetProperName(), GameUtil.GetFormattedDisease(component4.DiseaseIdx, component4.DiseaseCount, true));
										}
									}
								}
							}
						}
						if (flag6)
						{
							StateMachineController component5 = kSelectable.GetComponent<StateMachineController>();
							if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
							{
								ImmuneSystemMonitor.Instance sMI = component5.GetSMI<ImmuneSystemMonitor.Instance>();
								if (sMI != null)
								{
									AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(kSelectable);
									float value3 = amountInstance.value;
									bool flag7 = sMI.sm.isLosingImmunity.Get(sMI);
									Color32 badColorBG = NotificationScreen.Instance.BadColorBG;
									badColorBG.a = byte.MaxValue;
									Color32 colour = (!flag7) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : badColorBG;
									string str = string.Format(UI.OVERLAYS.DISEASE.IMMUNITY, GameUtil.GetFormattedPercent(value3, GameUtil.TimeSlice.None));
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(iconDash, 18);
									hoverTextDrawer.DrawText(GameUtil.ColourizeString(colour, str), Styles_Values.Property.Standard);
								}
							}
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(iconDash, 18);
							hoverTextDrawer.DrawText(text5, Styles_Values.Property.Standard);
						}
						int num4 = 0;
						foreach (StatusItemGroup.Entry item5 in overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							StatusItemGroup.Entry current6 = item5;
							if (ShowStatusItemInCurrentOverlay(current6.item))
							{
								if (num4 >= maxNumberOfDisplayedSelectableWarnings)
								{
									break;
								}
								if (current6.category != null && current6.category.Id == "Main" && num4 < maxNumberOfDisplayedSelectableWarnings)
								{
									TextStyleSetting style = (!IsStatusItemWarning(current6)) ? Styles_BodyText.Standard : HoverTextStyleSettings[1];
									Sprite icon = (current6.item.sprite == null) ? iconWarning : current6.item.sprite.sprite;
									Color color = (!IsStatusItemWarning(current6)) ? Styles_BodyText.Standard.textColor : HoverTextStyleSettings[1].textColor;
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(icon, color, 18, 2);
									hoverTextDrawer.DrawText(current6.GetName(), style);
									num4++;
								}
							}
						}
						foreach (StatusItemGroup.Entry item6 in overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							StatusItemGroup.Entry current7 = item6;
							if (ShowStatusItemInCurrentOverlay(current7.item))
							{
								if (num4 >= maxNumberOfDisplayedSelectableWarnings)
								{
									break;
								}
								if ((current7.category == null || current7.category.Id != "Main") && num4 < maxNumberOfDisplayedSelectableWarnings)
								{
									TextStyleSetting style2 = (!IsStatusItemWarning(current7)) ? Styles_BodyText.Standard : HoverTextStyleSettings[1];
									Sprite icon2 = (current7.item.sprite == null) ? iconWarning : current7.item.sprite.sprite;
									Color color2 = (!IsStatusItemWarning(current7)) ? Styles_BodyText.Standard.textColor : HoverTextStyleSettings[1].textColor;
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(icon2, color2, 18, 2);
									hoverTextDrawer.DrawText(current7.GetName(), style2);
									num4++;
								}
							}
						}
						float temp = 0f;
						bool flag8 = true;
						bool flag9 = OverlayModes.Temperature.ID == SimDebugView.Instance.GetMode();
						if ((bool)kSelectable.GetComponent<Constructable>())
						{
							flag8 = false;
						}
						else if (flag9 && (bool)component2)
						{
							temp = component2.Temperature;
						}
						else if ((bool)kSelectable.GetComponent<Building>() && (bool)component2)
						{
							temp = component2.Temperature;
						}
						else if ((UnityEngine.Object)kSelectable.GetComponent<CellSelectionObject>() != (UnityEngine.Object)null)
						{
							temp = kSelectable.GetComponent<CellSelectionObject>().temperature;
						}
						else
						{
							flag8 = false;
						}
						if (mode != OverlayModes.None.ID && mode != OverlayModes.Temperature.ID)
						{
							flag8 = false;
						}
						if (flag8)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(iconDash, 18);
							hoverTextDrawer.DrawText(GameUtil.GetFormattedTemperature(temp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), Styles_BodyText.Standard);
						}
						BuildingComplete component6 = kSelectable.GetComponent<BuildingComplete>();
						if ((UnityEngine.Object)component6 != (UnityEngine.Object)null && component6.Def.IsFoundation)
						{
							flag2 = false;
						}
						if (mode == OverlayModes.Light.ID && (UnityEngine.Object)choreConsumer != (UnityEngine.Object)null)
						{
							bool flag10 = false;
							foreach (Type hiddenChoreConsumerType in hiddenChoreConsumerTypes)
							{
								if ((UnityEngine.Object)choreConsumer.gameObject.GetComponent(hiddenChoreConsumerType) != (UnityEngine.Object)null)
								{
									flag10 = true;
									break;
								}
							}
							if (!flag10)
							{
								choreConsumer.ShowHoverTextOnHoveredItem(kSelectable, hoverTextDrawer, this);
							}
						}
						hoverTextDrawer.EndShadowBar();
					}
				}
			}
			if (flag2)
			{
				CellSelectionObject cellSelectionObject = null;
				if ((UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)null)
				{
					cellSelectionObject = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
				}
				bool flag11 = (UnityEngine.Object)cellSelectionObject != (UnityEngine.Object)null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
				if (flag11)
				{
					currentSelectedSelectableIndex = recentNumberOfDisplayedSelectables - 1;
				}
				Element element = Grid.Element[num];
				hoverTextDrawer.BeginShadowBar(flag11);
				hoverTextDrawer.DrawText(element.nameUpperCase, Styles_Title.Standard);
				if (Grid.DiseaseCount[num] > 0 || flag)
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					hoverTextDrawer.DrawText(GameUtil.GetFormattedDisease(Grid.DiseaseIdx[num], Grid.DiseaseCount[num], true), Styles_Values.Property.Standard);
				}
				if (!element.IsVacuum)
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					hoverTextDrawer.DrawText(ElementLoader.elements[Grid.ElementIdx[num]].GetMaterialCategoryTag().ProperName(), Styles_BodyText.Standard);
				}
				string[] array = WorldInspector.MassStringsReadOnly(num);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(iconDash, 18);
				for (int m = 0; m < array.Length; m++)
				{
					if (m >= 3 || !element.IsVacuum)
					{
						hoverTextDrawer.DrawText(array[m], Styles_BodyText.Standard);
					}
				}
				if (!element.IsVacuum)
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					Element element2 = Grid.Element[num];
					string text6 = cachedTemperatureString;
					float num5 = Grid.Temperature[num];
					if (num5 != cachedTemperature)
					{
						cachedTemperature = num5;
						text6 = (cachedTemperatureString = GameUtil.GetFormattedTemperature(Grid.Temperature[num], GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
					}
					string text7 = (element2.specificHeatCapacity != 0f) ? text6 : "N/A";
					hoverTextDrawer.DrawText(text7, Styles_BodyText.Standard);
				}
				if (CellSelectionObject.IsExposedToSpace(num))
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					hoverTextDrawer.DrawText(MISC.STATUSITEMS.SPACE.NAME, Styles_BodyText.Standard);
				}
				if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(num))
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					hoverTextDrawer.DrawText(MISC.STATUSITEMS.BURIEDITEM.NAME, Styles_BodyText.Standard);
				}
				if (element.id == SimHashes.OxyRock)
				{
					float num6 = Grid.AccumulatedFlow[num] / 3f;
					string text8 = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME;
					text8 = text8.Replace("{FlowRate}", GameUtil.GetFormattedMass(num6, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawIcon(iconDash, 18);
					hoverTextDrawer.DrawText(text8, Styles_BodyText.Standard);
					if (num6 <= 0f)
					{
						GameUtil.IsEmissionBlocked(num, out bool all_not_gaseous, out bool all_over_pressure);
						string text9 = null;
						if (all_not_gaseous)
						{
							text9 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
						}
						else if (all_over_pressure)
						{
							text9 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
						}
						if (text9 != null)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(iconDash, 18);
							hoverTextDrawer.DrawText(text9, Styles_BodyText.Standard);
						}
					}
				}
				hoverTextDrawer.EndShadowBar();
			}
			else if (!flag3)
			{
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawIcon(iconWarning, 18);
				hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.UNKNOWN, Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
			recentNumberOfDisplayedSelectables = num3 + 1;
			hoverTextDrawer.EndDrawing();
		}
	}

	private bool ShowStatusItemInCurrentOverlay(StatusItem status)
	{
		if ((UnityEngine.Object)OverlayScreen.Instance == (UnityEngine.Object)null)
		{
			return false;
		}
		return (status.status_overlays & (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());
	}

	private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
	{
		bool result = true;
		if ((UnityEngine.Object)OverlayScreen.Instance == (UnityEngine.Object)null)
		{
			return result;
		}
		if ((UnityEngine.Object)selectable == (UnityEngine.Object)null)
		{
			return false;
		}
		if ((UnityEngine.Object)selectable.GetComponent<KPrefabID>() == (UnityEngine.Object)null)
		{
			return result;
		}
		HashedString mode = OverlayScreen.Instance.GetMode();
		if (modeFilters.TryGetValue(mode, out Func<KSelectable, bool> value))
		{
			result = value(selectable);
		}
		return result;
	}

	private static bool ShouldShowOxygenOverlay(KSelectable selectable)
	{
		return (UnityEngine.Object)selectable.GetComponent<AlgaeHabitat>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<Electrolyzer>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<AirFilter>() != (UnityEngine.Object)null;
	}

	private static bool ShouldShowLightOverlay(KSelectable selectable)
	{
		return (!((UnityEngine.Object)selectable.GetComponent<Light2D>() == (UnityEngine.Object)null)) ? true : false;
	}

	private static bool ShouldShowGasConduitOverlay(KSelectable selectable)
	{
		return ((UnityEngine.Object)selectable.GetComponent<Conduit>() != (UnityEngine.Object)null && selectable.GetComponent<Conduit>().type == ConduitType.Gas) || ((UnityEngine.Object)selectable.GetComponent<Filterable>() != (UnityEngine.Object)null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Gas) || ((UnityEngine.Object)selectable.GetComponent<Vent>() != (UnityEngine.Object)null && selectable.GetComponent<Vent>().conduitType == ConduitType.Gas) || ((UnityEngine.Object)selectable.GetComponent<Pump>() != (UnityEngine.Object)null && selectable.GetComponent<Pump>().conduitType == ConduitType.Gas) || ((UnityEngine.Object)selectable.GetComponent<ValveBase>() != (UnityEngine.Object)null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Gas);
	}

	private static bool ShouldShowLiquidConduitOverlay(KSelectable selectable)
	{
		return ((UnityEngine.Object)selectable.GetComponent<Conduit>() != (UnityEngine.Object)null && selectable.GetComponent<Conduit>().type == ConduitType.Liquid) || ((UnityEngine.Object)selectable.GetComponent<Filterable>() != (UnityEngine.Object)null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Liquid) || ((UnityEngine.Object)selectable.GetComponent<Vent>() != (UnityEngine.Object)null && selectable.GetComponent<Vent>().conduitType == ConduitType.Liquid) || ((UnityEngine.Object)selectable.GetComponent<Pump>() != (UnityEngine.Object)null && selectable.GetComponent<Pump>().conduitType == ConduitType.Liquid) || ((UnityEngine.Object)selectable.GetComponent<ValveBase>() != (UnityEngine.Object)null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Liquid);
	}

	private static bool ShouldShowPowerOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.WireIDs.Contains(prefabTag) || (UnityEngine.Object)selectable.GetComponent<Battery>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<PowerTransformer>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<EnergyConsumer>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<EnergyGenerator>() != (UnityEngine.Object)null;
	}

	private static bool ShouldShowTemperatureOverlay(KSelectable selectable)
	{
		return (UnityEngine.Object)selectable.GetComponent<PrimaryElement>() != (UnityEngine.Object)null;
	}

	private static bool ShouldShowLogicOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag) || (UnityEngine.Object)selectable.GetComponent<LogicPorts>() != (UnityEngine.Object)null;
	}

	private static bool ShouldShowSolidConveyorOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.SolidConveyorIDs.Contains(prefabTag);
	}

	private static bool HideInOverlay(KSelectable selectable)
	{
		return false;
	}

	private static bool ShowOverlayIfHasComponent<T>(KSelectable selectable)
	{
		return selectable.GetComponent<T>() != null;
	}

	private static bool ShouldShowCropOverlay(KSelectable selectable)
	{
		return (UnityEngine.Object)selectable.GetComponent<Uprootable>() != (UnityEngine.Object)null || (UnityEngine.Object)selectable.GetComponent<PlanterBox>() != (UnityEngine.Object)null;
	}
}
