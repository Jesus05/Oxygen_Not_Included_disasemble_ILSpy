using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	[Serialize]
	public int timesUsed;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = true;
		resetProgressOnStop = true;
		attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			RoomType roomType = roomOfGameObject.roomType;
			roomType.TriggerRoomEffects(GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		amountInstance.SetValue(0f);
		timesUsed++;
		Trigger(-350347868, worker);
		base.OnCompleteWork(worker);
	}
}
