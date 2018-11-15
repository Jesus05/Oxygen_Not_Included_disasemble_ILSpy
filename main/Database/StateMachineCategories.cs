namespace Database
{
	public class StateMachineCategories : ResourceSet<StateMachine.Category>
	{
		public StateMachine.Category Ai;

		public StateMachine.Category Monitor;

		public StateMachine.Category Chore;

		public StateMachine.Category Misc;

		public StateMachineCategories()
		{
			Ai = Add(new StateMachine.Category("Ai"));
			Monitor = Add(new StateMachine.Category("Monitor"));
			Chore = Add(new StateMachine.Category("Chore"));
			Misc = Add(new StateMachine.Category("Misc"));
		}
	}
}
