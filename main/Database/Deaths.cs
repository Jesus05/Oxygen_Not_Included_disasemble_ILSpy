using STRINGS;

namespace Database
{
	public class Deaths : ResourceSet<Death>
	{
		public Death Generic;

		public Death Frozen;

		public Death Suffocation;

		public Death Starvation;

		public Death Slain;

		public Death Overheating;

		public Death Drowned;

		public Death Explosion;

		public Death FatalDisease;

		public Deaths(ResourceSet parent)
			: base("Deaths", parent)
		{
			Generic = new Death("Generic", this, DUPLICANTS.DEATHS.GENERIC.NAME, DUPLICANTS.DEATHS.GENERIC.DESCRIPTION, "dead_on_back", "dead_on_back");
			Frozen = new Death("Frozen", this, DUPLICANTS.DEATHS.FROZEN.NAME, DUPLICANTS.DEATHS.FROZEN.DESCRIPTION, "death_freeze_trans", "death_freeze_solid");
			Suffocation = new Death("Suffocation", this, DUPLICANTS.DEATHS.SUFFOCATION.NAME, DUPLICANTS.DEATHS.SUFFOCATION.DESCRIPTION, "death_suffocation", "dead_on_back");
			Starvation = new Death("Starvation", this, DUPLICANTS.DEATHS.STARVATION.NAME, DUPLICANTS.DEATHS.STARVATION.DESCRIPTION, "dead_on_back", "dead_on_back");
			Overheating = new Death("Overheating", this, DUPLICANTS.DEATHS.OVERHEATING.NAME, DUPLICANTS.DEATHS.OVERHEATING.DESCRIPTION, "dead_on_back", "dead_on_back");
			Drowned = new Death("Drowned", this, DUPLICANTS.DEATHS.DROWNED.NAME, DUPLICANTS.DEATHS.DROWNED.DESCRIPTION, "death_suffocation", "dead_on_back");
			Explosion = new Death("Explosion", this, DUPLICANTS.DEATHS.EXPLOSION.NAME, DUPLICANTS.DEATHS.EXPLOSION.DESCRIPTION, "dead_on_back", "dead_on_back");
			Slain = new Death("Combat", this, DUPLICANTS.DEATHS.COMBAT.NAME, DUPLICANTS.DEATHS.COMBAT.DESCRIPTION, "dead_on_back", "dead_on_back");
			FatalDisease = new Death("FatalDisease", this, DUPLICANTS.DEATHS.FATALDISEASE.NAME, DUPLICANTS.DEATHS.FATALDISEASE.DESCRIPTION, "dead_on_back", "dead_on_back");
		}
	}
}
