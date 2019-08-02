using System.Runtime.CompilerServices;

public class RoomMonitor : GameStateMachine<RoomMonitor, RoomMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Navigator navigator;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = GetComponent<Navigator>();
		}

		public Room GetCurrentRoomType()
		{
			return base.sm.currentRoom;
		}
	}

	public Room currentRoom;

	[CompilerGenerated]
	private static StateMachine<RoomMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventHandler(GameHashes.PathAdvanced, UpdateRoomType);
	}

	private static void UpdateRoomType(Instance smi)
	{
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(smi.master.gameObject);
		if (roomOfGameObject != smi.sm.currentRoom)
		{
			smi.sm.currentRoom = roomOfGameObject;
			roomOfGameObject?.cavity.OnEnter(smi.master.gameObject);
		}
	}
}
