namespace Database
{
	public class Faces : ResourceSet<Face>
	{
		public Face Neutral;

		public Face Happy;

		public Face Uncomfortable;

		public Face Cold;

		public Face Hot;

		public Face Tired;

		public Face Sleep;

		public Face Hungry;

		public Face Angry;

		public Face Suffocate;

		public Face Dead;

		public Face Sick;

		public Face SickSpores;

		public Face SickFierySkin;

		public Face SickCold;

		public Faces()
		{
			Neutral = Add(new Face("Neutral"));
			Happy = Add(new Face("Happy"));
			Uncomfortable = Add(new Face("Uncomfortable"));
			Cold = Add(new Face("Cold"));
			Hot = Add(new Face("Hot"));
			Tired = Add(new Face("Tired"));
			Sleep = Add(new Face("Sleep"));
			Hungry = Add(new Face("Hungry"));
			Angry = Add(new Face("Angry"));
			Suffocate = Add(new Face("Suffocate"));
			Sick = Add(new Face("Sick"));
			SickSpores = Add(new Face("Spores"));
			SickFierySkin = Add(new Face("Fiery"));
			SickCold = Add(new Face("Cold"));
			Dead = Add(new Face("Death"));
		}
	}
}
