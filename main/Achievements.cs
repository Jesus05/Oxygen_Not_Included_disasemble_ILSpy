public class Achievements : KMonoBehaviour
{
	public static string[] ACHIEVEMENT_IDS = new string[2]
	{
		"test_0",
		"test_1"
	};

	public void Unlock(string id)
	{
		Debug.LogFormat("UNLOCK ACHIEVEMENT {0}", id);
		if ((bool)SteamAchievementService.Instance)
		{
			SteamAchievementService.Instance.Unlock(id);
		}
	}
}
