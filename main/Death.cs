public class Death : Resource
{
	public string preAnim;

	public string loopAnim;

	public string sound;

	public string description;

	public Death(string id, ResourceSet parent, string name, string description, string pre_anim, string loop_anim)
		: base(id, parent, name)
	{
		preAnim = pre_anim;
		loopAnim = loop_anim;
		this.description = description;
	}
}
