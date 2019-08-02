namespace Database
{
	public class Expressions : ResourceSet<Expression>
	{
		public Expression Neutral;

		public Expression Happy;

		public Expression Uncomfortable;

		public Expression Cold;

		public Expression Hot;

		public Expression FullBladder;

		public Expression Tired;

		public Expression Hungry;

		public Expression Angry;

		public Expression Unhappy;

		public Expression RedAlert;

		public Expression Suffocate;

		public Expression RecoverBreath;

		public Expression Sick;

		public Expression SickSpores;

		public Expression Zombie;

		public Expression SickFierySkin;

		public Expression SickCold;

		public Expression Relief;

		public Expressions(ResourceSet parent)
			: base("Expressions", parent)
		{
			Faces faces = Db.Get().Faces;
			Angry = new Expression("Angry", this, faces.Angry);
			Suffocate = new Expression("Suffocate", this, faces.Suffocate);
			RecoverBreath = new Expression("RecoverBreath", this, faces.Uncomfortable);
			RedAlert = new Expression("RedAlert", this, faces.Hot);
			Hungry = new Expression("Hungry", this, faces.Hungry);
			SickSpores = new Expression("SickSpores", this, faces.SickSpores);
			Zombie = new Expression("Zombie", this, faces.Zombie);
			SickFierySkin = new Expression("SickFierySkin", this, faces.SickFierySkin);
			SickCold = new Expression("SickCold", this, faces.SickCold);
			Sick = new Expression("Sick", this, faces.Sick);
			Cold = new Expression("Cold", this, faces.Cold);
			Hot = new Expression("Hot", this, faces.Hot);
			FullBladder = new Expression("FullBladder", this, faces.Uncomfortable);
			Tired = new Expression("Tired", this, faces.Tired);
			Unhappy = new Expression("Unhappy", this, faces.Uncomfortable);
			Uncomfortable = new Expression("Uncomfortable", this, faces.Uncomfortable);
			Happy = new Expression("Happy", this, faces.Happy);
			Relief = new Expression("Relief", this, faces.Happy);
			Neutral = new Expression("Neutral", this, faces.Neutral);
			for (int num = Count - 1; num >= 0; num--)
			{
				resources[num].priority = 100 * (Count - num);
			}
		}
	}
}
