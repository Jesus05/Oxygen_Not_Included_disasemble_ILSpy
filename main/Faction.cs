using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Faction
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct FactionIDComparer : IEqualityComparer<FactionManager.FactionID>
	{
		public bool Equals(FactionManager.FactionID x, FactionManager.FactionID y)
		{
			return x == y;
		}

		public int GetHashCode(FactionManager.FactionID obj)
		{
			return (int)obj;
		}
	}

	public HashSet<FactionAlignment> Members = new HashSet<FactionAlignment>();

	public FactionManager.FactionID ID;

	public Dictionary<FactionManager.FactionID, FactionManager.Disposition> Dispositions = new Dictionary<FactionManager.FactionID, FactionManager.Disposition>(default(FactionIDComparer));

	public Faction(FactionManager.FactionID faction)
	{
		ID = faction;
		ConfigureAlignments(faction);
	}

	public HashSet<FactionAlignment> HostileTo()
	{
		HashSet<FactionAlignment> hashSet = new HashSet<FactionAlignment>();
		foreach (KeyValuePair<FactionManager.FactionID, FactionManager.Disposition> disposition in Dispositions)
		{
			if (disposition.Value == FactionManager.Disposition.Attack)
			{
				hashSet.UnionWith(FactionManager.Instance.GetFaction(disposition.Key).Members);
			}
		}
		return hashSet;
	}

	private void ConfigureAlignments(FactionManager.FactionID faction)
	{
		switch (faction)
		{
		case FactionManager.FactionID.Duplicant:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Assist);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Assist);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Neutral);
			break;
		case FactionManager.FactionID.Friendly:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Assist);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Assist);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Neutral);
			break;
		case FactionManager.FactionID.Hostile:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Attack);
			break;
		case FactionManager.FactionID.Predator:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Attack);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Attack);
			break;
		case FactionManager.FactionID.Prey:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Neutral);
			break;
		case FactionManager.FactionID.Pest:
			Dispositions.Add(FactionManager.FactionID.Duplicant, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Friendly, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Hostile, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Predator, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Prey, FactionManager.Disposition.Neutral);
			Dispositions.Add(FactionManager.FactionID.Pest, FactionManager.Disposition.Neutral);
			break;
		}
	}
}
