public class EventBase : Resource
{
	public int hash;

	public EventBase(string id)
		: base(id, id)
	{
		hash = Hash.SDBMLower(id);
	}

	public virtual string GetDescription(EventInstanceBase ev)
	{
		return "";
	}
}
