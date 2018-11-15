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

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			Initialize();
			None = Add("None", string.Empty, Color.grey);
			Food = Add("Food", string.Empty, new Color(1f, 0.8862745f, 0.5176471f));
			Sleep = Add("Sleep", string.Empty, new Color(0.6392157f, 1f, 0.5176471f));
			Recreation = Add("Recreation", string.Empty, new Color(0.258823544f, 0.6431373f, 0.956862748f));
			Bathroom = Add("Bathroom", string.Empty, new Color(0.5176471f, 1f, 0.956862748f));
			Hospital = Add("Hospital", string.Empty, new Color(1f, 0.5176471f, 0.5568628f));
			Industrial = Add("Industrial", string.Empty, new Color(0.956862748f, 0.772549033f, 0.258823544f));
			Agricultural = Add("Agricultural", string.Empty, new Color(0.8039216f, 0.9490196f, 0.282352954f));
		}

		private RoomTypeCategory Add(string id, string name, Color color)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, color);
			Add(roomTypeCategory);
			return roomTypeCategory;
		}
	}
}
