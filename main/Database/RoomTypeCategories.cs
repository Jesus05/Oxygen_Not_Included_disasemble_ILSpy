using UnityEngine;

namespace Database
{
	public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
	{
		public RoomTypeCategory None;

		public RoomTypeCategory Food;

		public RoomTypeCategory Sleep;

		public RoomTypeCategory Recreation;

		public RoomTypeCategory Bathroom;

		public RoomTypeCategory Hospital;

		public RoomTypeCategory Industrial;

		public RoomTypeCategory Agricultural;

		public RoomTypeCategory Park;

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			Initialize();
			None = Add("None", "", Color.grey);
			Food = Add("Food", "", new Color(1f, 0.8862745f, 0.5176471f));
			Sleep = Add("Sleep", "", new Color(0.6392157f, 1f, 0.5176471f));
			Recreation = Add("Recreation", "", new Color(0.258823544f, 0.6431373f, 0.956862748f));
			Bathroom = Add("Bathroom", "", new Color(0.5176471f, 1f, 0.956862748f));
			Hospital = Add("Hospital", "", new Color(1f, 0.5176471f, 0.5568628f));
			Industrial = Add("Industrial", "", new Color(0.956862748f, 0.772549033f, 0.258823544f));
			Agricultural = Add("Agricultural", "", new Color(0.8039216f, 0.9490196f, 0.282352954f));
			Park = Add("Park", "", new Color(0.6745098f, 1f, 0.7411765f));
		}

		private RoomTypeCategory Add(string id, string name, Color color)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, color);
			Add(roomTypeCategory);
			return roomTypeCategory;
		}
	}
}
