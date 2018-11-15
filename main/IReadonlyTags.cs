public interface IReadonlyTags
{
	bool HasTag(string tag);

	bool HasTag(int hashtag);

	bool HasTags(int[] tags);
}
