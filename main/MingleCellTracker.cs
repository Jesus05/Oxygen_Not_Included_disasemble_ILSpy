using System.Collections.Generic;

public class MingleCellTracker : KMonoBehaviour, ISim1000ms
{
	public List<int> mingleCells = new List<int>();

	public void Sim1000ms(float dt)
	{
		mingleCells.Clear();
		RoomProber roomProber = Game.Instance.roomProber;
		MinionGroupProber minionGroupProber = MinionGroupProber.Get();
		foreach (Room room in roomProber.rooms)
		{
			if (room.roomType == Db.Get().RoomTypes.RecRoom)
			{
				for (int i = room.cavity.minY; i <= room.cavity.maxY; i++)
				{
					for (int j = room.cavity.minX; j <= room.cavity.maxX; j++)
					{
						int num = Grid.XYToCell(j, i);
						CavityInfo cavityForCell = roomProber.GetCavityForCell(num);
						if (cavityForCell == room.cavity && minionGroupProber.IsReachable(num) && !Grid.HasLadder[num] && !Grid.HasTube[num] && !Grid.IsLiquid(num) && Grid.Element[num].id == SimHashes.Oxygen)
						{
							mingleCells.Add(num);
						}
					}
				}
			}
		}
	}
}
