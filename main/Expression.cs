using System.Diagnostics;

[DebuggerDisplay("{face.hash} {priority}")]
public class Expression : Resource
{
	public Face face;

	public int priority;

	public Expression(string id, ResourceSet parent, Face face)
		: base(id, parent, null)
	{
		this.face = face;
	}
}
