using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DestinationSelectPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject asteroidPrefab;

	[SerializeField]
	private KButtonDrag dragTarget;

	[SerializeField]
	private MultiToggle leftArrowButton;

	[SerializeField]
	private MultiToggle rightArrowButton;

	[SerializeField]
	private RectTransform asteroidContainer;

	[SerializeField]
	private float asteroidFocusScale = 2f;

	[SerializeField]
	private float asteroidXSeparation = 240f;

	[SerializeField]
	private float focusScaleSpeed = 0.5f;

	[SerializeField]
	private float centeringSpeed = 0.5f;

	private float offset = 0f;

	private int selectedIndex = -1;

	private List<DestinationAsteroid2> asteroids = new List<DestinationAsteroid2>();

	private int numAsteroids;

	private List<string> worldNames;

	private Dictionary<string, ColonyDestinationAsteroidData> asteroidData = new Dictionary<string, ColonyDestinationAsteroidData>();

	private Vector2 dragStartPos;

	private Vector2 dragLastPos;

	private bool isDragging = false;

	private float min => asteroidContainer.rect.x + offset;

	private float max => min + asteroidContainer.rect.width;

	public event Action<ColonyDestinationAsteroidData> OnAsteroidClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		dragTarget.onBeginDrag += BeginDrag;
		dragTarget.onDrag += Drag;
		dragTarget.onEndDrag += EndDrag;
		MultiToggle multiToggle = leftArrowButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(ClickLeft));
		MultiToggle multiToggle2 = rightArrowButton;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, new System.Action(ClickRight));
	}

	private void BeginDrag()
	{
		dragStartPos = Input.mousePosition;
		dragLastPos = dragStartPos;
		isDragging = true;
		KFMOD.PlayOneShot(GlobalAssets.GetSound("DestinationSelect_Scroll_Start", false));
	}

	private void Drag()
	{
		Vector2 vector = Input.mousePosition;
		float num = vector.x - dragLastPos.x;
		dragLastPos = vector;
		offset += num;
		int num2 = selectedIndex;
		selectedIndex = Mathf.RoundToInt((0f - offset) / asteroidXSeparation);
		selectedIndex = Mathf.Clamp(selectedIndex, 0, worldNames.Count - 1);
		if (num2 != selectedIndex)
		{
			this.OnAsteroidClicked(asteroidData[worldNames[selectedIndex]]);
			KFMOD.PlayOneShot(GlobalAssets.GetSound("DestinationSelect_Scroll", false));
		}
	}

	private void EndDrag()
	{
		Drag();
		isDragging = false;
		KFMOD.PlayOneShot(GlobalAssets.GetSound("DestinationSelect_Scroll_Stop", false));
	}

	private void ClickLeft()
	{
		selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, worldNames.Count - 1);
		this.OnAsteroidClicked(asteroidData[worldNames[selectedIndex]]);
	}

	private void ClickRight()
	{
		selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, worldNames.Count - 1);
		this.OnAsteroidClicked(asteroidData[worldNames[selectedIndex]]);
	}

	protected override void OnSpawn()
	{
		WorldGen.LoadSettings();
		worldNames = SettingsCache.worlds.GetNames();
		foreach (string worldName in worldNames)
		{
			ColonyDestinationAsteroidData value = new ColonyDestinationAsteroidData(worldName, 0);
			asteroidData[worldName] = value;
		}
		worldNames.Sort(delegate(string a, string b)
		{
			ColonyDestinationAsteroidData colonyDestinationAsteroidData = asteroidData[a];
			ColonyDestinationAsteroidData colonyDestinationAsteroidData2 = asteroidData[b];
			return colonyDestinationAsteroidData.difficulty.CompareTo(colonyDestinationAsteroidData2.difficulty);
		});
	}

	private void Update()
	{
		if (!isDragging)
		{
			float num = offset + (float)selectedIndex * asteroidXSeparation;
			float value = 0f;
			if (num != 0f)
			{
				value = 0f - num;
			}
			value = Mathf.Clamp(value, (0f - asteroidXSeparation) * 2f, asteroidXSeparation * 2f);
			if (value != 0f)
			{
				float num2 = centeringSpeed * Time.unscaledDeltaTime;
				float num3 = value * centeringSpeed * Time.unscaledDeltaTime;
				if (num3 > 0f && num3 < num2)
				{
					num3 = Mathf.Min(num2, value);
				}
				else if (num3 < 0f && num3 > 0f - num2)
				{
					num3 = Mathf.Max(0f - num2, value);
				}
				offset += num3;
			}
		}
		Vector2 min = asteroidContainer.rect.min;
		float x = min.x;
		Vector2 max = asteroidContainer.rect.max;
		float x2 = max.x;
		offset = Mathf.Clamp(offset, (float)(-(worldNames.Count - 1)) * asteroidXSeparation + x, x2);
		RePlaceAsteroids();
	}

	[ContextMenu("RePlaceAsteroids")]
	public void RePlaceAsteroids()
	{
		BeginAsteroidDrawing();
		for (int i = 0; i < worldNames.Count; i++)
		{
			if (i != selectedIndex)
			{
				float num = offset + (float)i * asteroidXSeparation;
				if (!(num + offset + asteroidXSeparation < min) && !(num + offset - asteroidXSeparation > max))
				{
					DestinationAsteroid2 asteroid = GetAsteroid(worldNames[i], 1f);
					asteroid.transform.SetLocalPosition(new Vector3(num, 0f, 0f));
					if (numAsteroids > 100)
					{
						break;
					}
				}
			}
		}
		float x = offset + (float)selectedIndex * asteroidXSeparation;
		DestinationAsteroid2 asteroid2 = GetAsteroid(worldNames[selectedIndex], asteroidFocusScale);
		asteroid2.transform.SetLocalPosition(new Vector3(x, 0f, 0f));
		EndAsteroidDrawing();
	}

	private void BeginAsteroidDrawing()
	{
		numAsteroids = 0;
	}

	private DestinationAsteroid2 GetAsteroid(string name, float scale)
	{
		DestinationAsteroid2 destinationAsteroid;
		if (numAsteroids < asteroids.Count)
		{
			destinationAsteroid = asteroids[numAsteroids];
		}
		else
		{
			destinationAsteroid = Util.KInstantiateUI<DestinationAsteroid2>(asteroidPrefab, asteroidContainer.gameObject, false);
			destinationAsteroid.OnClicked += this.OnAsteroidClicked;
			asteroids.Add(destinationAsteroid);
		}
		asteroidData[name].TargetScale = scale;
		asteroidData[name].Scale += (asteroidData[name].TargetScale - asteroidData[name].Scale) * focusScaleSpeed * Time.unscaledDeltaTime;
		destinationAsteroid.transform.localScale = Vector3.one * asteroidData[name].Scale;
		destinationAsteroid.SetAsteroid(asteroidData[name]);
		numAsteroids++;
		return destinationAsteroid;
	}

	private void EndAsteroidDrawing()
	{
		for (int i = 0; i < asteroids.Count; i++)
		{
			asteroids[i].gameObject.SetActive(i < numAsteroids);
		}
	}

	public ColonyDestinationAsteroidData SelectAsteroid(string name, int seed)
	{
		selectedIndex = worldNames.IndexOf(name);
		asteroidData[name].ReInitialize(seed);
		return asteroidData[name];
	}

	public void ScrollLeft()
	{
		int index = Mathf.Max(selectedIndex - 1, 0);
		this.OnAsteroidClicked(asteroidData[worldNames[index]]);
	}

	public void ScrollRight()
	{
		int index = Mathf.Min(selectedIndex + 1, worldNames.Count - 1);
		this.OnAsteroidClicked(asteroidData[worldNames[index]]);
	}
}
