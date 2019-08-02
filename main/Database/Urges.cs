namespace Database
{
	public class Urges : ResourceSet<Urge>
	{
		public Urge BeIncapacitated;

		public Urge Sleep;

		public Urge Narcolepsy;

		public Urge Eat;

		public Urge WashHands;

		public Urge Shower;

		public Urge Pee;

		public Urge MoveToQuarantine;

		public Urge HealCritical;

		public Urge RecoverBreath;

		public Urge Emote;

		public Urge Feed;

		public Urge Doctor;

		public Urge Flee;

		public Urge Heal;

		public Urge PacifyIdle;

		public Urge PacifyEat;

		public Urge PacifySleep;

		public Urge PacifyRelocate;

		public Urge RestDueToDisease;

		public Urge EmoteHighPriority;

		public Urge Aggression;

		public Urge MoveToSafety;

		public Urge WarmUp;

		public Urge CoolDown;

		public Urge LearnSkill;

		public Urge EmoteIdle;

		public Urges()
		{
			HealCritical = Add(new Urge("HealCritical"));
			BeIncapacitated = Add(new Urge("BeIncapacitated"));
			PacifyEat = Add(new Urge("PacifyEat"));
			PacifySleep = Add(new Urge("PacifySleep"));
			PacifyIdle = Add(new Urge("PacifyIdle"));
			EmoteHighPriority = Add(new Urge("EmoteHighPriority"));
			RecoverBreath = Add(new Urge("RecoverBreath"));
			Aggression = Add(new Urge("Aggression"));
			MoveToQuarantine = Add(new Urge("MoveToQuarantine"));
			WashHands = Add(new Urge("WashHands"));
			Shower = Add(new Urge("Shower"));
			Eat = Add(new Urge("Eat"));
			Pee = Add(new Urge("Pee"));
			RestDueToDisease = Add(new Urge("RestDueToDisease"));
			Sleep = Add(new Urge("Sleep"));
			Narcolepsy = Add(new Urge("Narcolepsy"));
			Doctor = Add(new Urge("Doctor"));
			Heal = Add(new Urge("Heal"));
			Feed = Add(new Urge("Feed"));
			PacifyRelocate = Add(new Urge("PacifyRelocate"));
			Emote = Add(new Urge("Emote"));
			MoveToSafety = Add(new Urge("MoveToSafety"));
			WarmUp = Add(new Urge("WarmUp"));
			CoolDown = Add(new Urge("CoolDown"));
			LearnSkill = Add(new Urge("LearnSkill"));
			EmoteIdle = Add(new Urge("EmoteIdle"));
		}
	}
}
