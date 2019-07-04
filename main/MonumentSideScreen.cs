using System.Collections.Generic;
using UnityEngine;

public class MonumentSideScreen : SideScreenContent
{
	private MonumentPart target;

	public KButton debugVictoryButton;

	public KButton flipButton;

	public GameObject stateButtonPrefab;

	private List<GameObject> buttons = new List<GameObject>();

	[SerializeField]
	private RectTransform buttonContainer;

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<MonumentPart>() != (Object)null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		debugVictoryButton.onClick += delegate
		{
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Thriving.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Clothe8Dupes.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.EatkCalFromMeatByCycle100.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Build4NatureReserves.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.CoolBuildingTo6K.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.NoFarmTilesAndKCal.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.ReachedSpace.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.TameAGassyMoo.Id);
			GameScheduler.Instance.Schedule("ForceCheckAchievements", 0.1f, delegate
			{
				Game.Instance.Trigger(395452326, null);
			}, null, null);
		};
		debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && target.part == MonumentPart.Part.Top);
		flipButton.onClick += delegate
		{
			target.GetComponent<Rotatable>().Rotate();
		};
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.target = target.GetComponent<MonumentPart>();
		debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPart.Part.Top);
		GenerateStateButtons();
	}

	public void GenerateStateButtons()
	{
		for (int num = buttons.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(buttons[num]);
		}
		buttons.Clear();
		foreach (Tuple<string, string> selectableStatesAndSymbol in target.selectableStatesAndSymbols)
		{
			GameObject gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
			string targetState = selectableStatesAndSymbol.first;
			string second = selectableStatesAndSymbol.second;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				target.SetState(targetState);
			};
			buttons.Add(gameObject);
			KAnimFile animFile = target.GetComponent<KBatchedAnimController>().AnimFiles[0];
			gameObject.GetComponent<KButton>().fgImage.sprite = Def.GetUISpriteFromMultiObjectAnim(animFile, targetState, false, second);
		}
	}
}
