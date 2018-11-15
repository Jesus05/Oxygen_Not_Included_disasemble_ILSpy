namespace Database
{
	public class Shirts : ResourceSet<Shirt>
	{
		public Shirt Hot00;

		public Shirt Hot01;

		public Shirt Decor00;

		public Shirt Cold00;

		public Shirt Cold01;

		public Shirts()
		{
			Hot00 = Add(new Shirt("body_shirt_hot01"));
			Hot01 = Add(new Shirt("body_shirt_hot02"));
			Decor00 = Add(new Shirt("body_shirt_decor01"));
			Cold00 = Add(new Shirt("body_shirt_cold01"));
			Cold01 = Add(new Shirt("body_shirt_cold02"));
		}
	}
}
