using System;
using System.Collections;
using System.Collections.Generic;

public class Components
{
	public class Cmps<T> : ICollection, IEnumerable
	{
		private Dictionary<T, HandleVector<int>.Handle> table;

		private KCompactedVector<T> items;

		public List<T> Items => items.GetDataList();

		public int Count => items.Count;

		public T this[int idx]
		{
			get
			{
				return Items[idx];
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public event Action<T> OnAdd;

		public event Action<T> OnRemove;

		public Cmps()
		{
			App.OnPreLoadScene = (System.Action)Delegate.Combine(App.OnPreLoadScene, new System.Action(Clear));
			items = new KCompactedVector<T>(0);
			table = new Dictionary<T, HandleVector<int>.Handle>();
		}

		private void Clear()
		{
			items.Clear();
			table.Clear();
			this.OnAdd = null;
			this.OnRemove = null;
		}

		public void Add(T cmp)
		{
			HandleVector<int>.Handle value = items.Allocate(cmp);
			table[cmp] = value;
			if (this.OnAdd != null)
			{
				this.OnAdd(cmp);
			}
		}

		public void Remove(T cmp)
		{
			HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
			if (table.TryGetValue(cmp, out value))
			{
				table.Remove(cmp);
				items.Free(value);
				if (this.OnRemove != null)
				{
					this.OnRemove(cmp);
				}
			}
		}

		public void Register(Action<T> on_add, Action<T> on_remove)
		{
			OnAdd += on_add;
			OnRemove += on_remove;
			foreach (T item in Items)
			{
				this.OnAdd(item);
			}
		}

		public void Unregister(Action<T> on_add, Action<T> on_remove)
		{
			OnAdd -= on_add;
			OnRemove -= on_remove;
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}

	public static Cmps<MinionIdentity> LiveMinionIdentities = new Cmps<MinionIdentity>();

	public static Cmps<MinionIdentity> MinionIdentities = new Cmps<MinionIdentity>();

	public static Cmps<MinionIdentity> StoredMinionIdentities = new Cmps<MinionIdentity>();

	public static Cmps<Sleepable> Sleepables = new Cmps<Sleepable>();

	public static Cmps<IUsable> Toilets = new Cmps<IUsable>();

	public static Cmps<Pickupable> Pickupables = new Cmps<Pickupable>();

	public static Cmps<Brain> Brains = new Cmps<Brain>();

	public static Cmps<BuildingComplete> BuildingCompletes = new Cmps<BuildingComplete>();

	public static Cmps<Notifier> Notifiers = new Cmps<Notifier>();

	public static Cmps<Fabricator> Fabricators = new Cmps<Fabricator>();

	public static Cmps<Refinery> Refineries = new Cmps<Refinery>();

	public static Cmps<PlantablePlot> PlantablePlots = new Cmps<PlantablePlot>();

	public static Cmps<Ladder> Ladders = new Cmps<Ladder>();

	public static Cmps<ITravelTubePiece> ITravelTubePieces = new Cmps<ITravelTubePiece>();

	public static Cmps<CreatureFeeder> CreatureFeeders = new Cmps<CreatureFeeder>();

	public static Cmps<Light2D> Light2Ds = new Cmps<Light2D>();

	public static Cmps<Edible> Edibles = new Cmps<Edible>();

	public static Cmps<Diggable> Diggables = new Cmps<Diggable>();

	public static Cmps<ResearchCenter> ResearchCenters = new Cmps<ResearchCenter>();

	public static Cmps<Harvestable> Harvestables = new Cmps<Harvestable>();

	public static Cmps<Uprootable> Uprootables = new Cmps<Uprootable>();

	public static Cmps<Health> Health = new Cmps<Health>();

	public static Cmps<Equipment> Equipment = new Cmps<Equipment>();

	public static Cmps<FactionAlignment> FactionAlignments = new Cmps<FactionAlignment>();

	public static Cmps<Telepad> Telepads = new Cmps<Telepad>();

	public static Cmps<Generator> Generators = new Cmps<Generator>();

	public static Cmps<EnergyConsumer> EnergyConsumers = new Cmps<EnergyConsumer>();

	public static Cmps<Battery> Batteries = new Cmps<Battery>();

	public static Cmps<Breakable> Breakables = new Cmps<Breakable>();

	public static Cmps<Crop> Crops = new Cmps<Crop>();

	public static Cmps<Prioritizable> Prioritizables = new Cmps<Prioritizable>();

	public static Cmps<Clinic> Clinics = new Cmps<Clinic>();

	public static Cmps<HandSanitizer> HandSanitizers = new Cmps<HandSanitizer>();

	public static Cmps<BuildingCellVisualizer> BuildingCellVisualizers = new Cmps<BuildingCellVisualizer>();

	public static Cmps<RoleStation> RoleStations = new Cmps<RoleStation>();

	public static Cmps<Telescope> Telescopes = new Cmps<Telescope>();

	public static Cmps<Capturable> Capturables = new Cmps<Capturable>();

	public static Cmps<NotCapturable> NotCapturables = new Cmps<NotCapturable>();

	public static Cmps<DiseaseSourceVisualizer> DiseaseSourceVisualizers = new Cmps<DiseaseSourceVisualizer>();

	public static Cmps<DetectorNetwork.Instance> DetectorNetworks = new Cmps<DetectorNetwork.Instance>();

	public static Cmps<Grave> Graves = new Cmps<Grave>();

	public static Cmps<AttachableBuilding> AttachableBuildings = new Cmps<AttachableBuilding>();

	public static Cmps<BuildingAttachPoint> BuildingAttachPoints = new Cmps<BuildingAttachPoint>();
}
