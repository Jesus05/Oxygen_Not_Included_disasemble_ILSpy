using System.Collections.Generic;

[SkipSaveFileSerialization]
public class ChoreProvider : KMonoBehaviour
{
	public List<Chore> chores = new List<Chore>();

	public string Name
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Name = base.name;
	}

	public virtual Chore AddChore(Chore chore)
	{
		chore.provider = this;
		chores.Add(chore);
		return chore;
	}

	public virtual Chore RemoveChore(Chore chore)
	{
		if (chore != null)
		{
			chores.Remove(chore);
			return chore;
		}
		return null;
	}

	public virtual void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		for (int i = 0; i < chores.Count; i++)
		{
			Chore chore = chores[i];
			chore.CollectChores(consumer_state, succeeded, failed_contexts, false);
		}
	}
}
